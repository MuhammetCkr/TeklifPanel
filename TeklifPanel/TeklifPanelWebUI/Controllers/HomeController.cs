using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Data.Concrete.EfCore;
using TeklifPanel.Entity;
using TeklifPanelWebUI.Models;
using TeklifPanelWebUI.ViewModels;

namespace TeklifPanelWebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly UserManager<User> _userManager;
        private readonly IOfferService _offerService;
        private readonly ILogService _logService;
        private readonly ICompanyService _companyService;

        private IOptions<CurrenciesModel> _defaultData;
        private readonly IWritableOptions<CurrenciesModel> _writableOptions;
        private readonly IConfiguration _configuration;

        public HomeController(ICustomerService customerService, UserManager<User> userManager, IOfferService offerService, ILogService logService, ICompanyService companyService, IOptions<CurrenciesModel> defaultData, IWritableOptions<CurrenciesModel> writableOptions, IConfiguration configuration)
        {
            _customerService = customerService;
            _userManager = userManager;
            _offerService = offerService;
            _logService = logService;
            _companyService = companyService;
            _defaultData = defaultData;
            _writableOptions = writableOptions;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            Currency currency = new Currency(_defaultData, _writableOptions, _configuration);
            currency.UpdateCurrencies();

            if (User.Identity.IsAuthenticated)
            {
                var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
                var userId = HttpContext.Session.GetString("UserId");
                var offerList = await _offerService.GetCompanyOffersAsync(companyId) ?? new List<Offer>();
                var logList = await _logService.GetCompanyLogsAsync(companyId);
                var customerList = await _customerService.GetCompanyByCustomersAsync(companyId) ?? new List<Customer>();
                var user = await _userManager.FindByIdAsync(userId);

                var usdTotal = offerList.SelectMany(x => x.ProductOffers.Where(x => x.Currency == "usd").Select(x => x.ProductSellPrice * x.Amount)).Sum();
                var euTotal = offerList.SelectMany(x => x.ProductOffers.Where(x => x.Currency == "eu").Select(x => x.ProductSellPrice * x.Amount)).Sum();
                var gbpTotal = offerList.SelectMany(x => x.ProductOffers.Where(x => x.Currency == "gbp").Select(x => x.ProductSellPrice * x.Amount)).Sum();
                var tlTotal = offerList.SelectMany(x => x.ProductOffers.Where(x => x.Currency == "tl").Select(x => x.ProductSellPrice * x.Amount)).Sum();

                var usdTotalUser = offerList.Where(x => x.UserId == userId).SelectMany(x => x.ProductOffers.Where(x => x.Currency == "usd").Select(x => x.ProductSellPrice * x.Amount)).Sum();
                var euTotalUser = offerList.Where(x => x.UserId == userId).SelectMany(x => x.ProductOffers.Where(x => x.Currency == "eu").Select(x => x.ProductSellPrice * x.Amount)).Sum();
                var gbpTotalUser = offerList.Where(x => x.UserId == userId).SelectMany(x => x.ProductOffers.Where(x => x.Currency == "gbp").Select(x => x.ProductSellPrice * x.Amount)).Sum();
                var tlTotalUser = offerList.Where(x => x.UserId == userId).SelectMany(x => x.ProductOffers.Where(x => x.Currency == "tl").Select(x => x.ProductSellPrice * x.Amount)).Sum();

                if (user != null)
                {
                    var homeViewModel = new HomeViewModel()
                    {
                        Offers = offerList,
                        Logs = logList,
                        CustomerList = customerList.ToList(),
                        User = user,
                        USD = usdTotal,
                        EURO = euTotal,
                        GBP = gbpTotal,
                        TL = tlTotal,
                        USDUser = usdTotalUser,
                        EUROUser = euTotalUser,
                        GBPUser = gbpTotalUser,
                        TLUser = tlTotalUser,
                    };
                    return View(homeViewModel);
                }
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}