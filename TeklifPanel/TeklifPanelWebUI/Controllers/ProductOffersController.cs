using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.Design;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Entity;
using TeklifPanelWebUI.ViewModels;

namespace TeklifPanelWebUI.Controllers
{
    public class ProductOffersController : Controller
    {
        private readonly IProductOffersService _productOffersService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IContactPersonService _contactPersonService;
        private readonly ICompanyService _companyService;
        private readonly ICompanySettingsService _companySettingsService;
        private readonly IOfferService _offerService;
        private readonly IConverter _pdfConverter;
        private readonly UserManager<User> _userManager;
        private readonly IOfferTableService _offerTableService;

        public ProductOffersController(IProductOffersService productOffersService, ICustomerService customerService, IProductService productService, ICategoryService categoryService, IContactPersonService contactPersonService, ICompanyService companyService, ICompanySettingsService companySettingsService, IOfferService offerService, IConverter pdfConverter, UserManager<User> userManager, IOfferTableService offerTableService)
        {
            _productOffersService = productOffersService;
            _customerService = customerService;
            _productService = productService;
            _categoryService = categoryService;
            _contactPersonService = contactPersonService;
            _companyService = companyService;
            _companySettingsService = companySettingsService;
            _offerService = offerService;
            _pdfConverter = pdfConverter;
            _userManager = userManager;
            _offerTableService = offerTableService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOffer(int offerId)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var company = await _companyService.GetByIdAsync(companyId);
            var companySettings = await _companySettingsService.GetAllCompanySettingsAsync(companyId);

            var offer = await _offerService.GetOfferAsync(offerId);
            var offerTables = await _offerTableService.GetManyAsync(o => o.CompanyId == companyId);

            var selectecProductList = new List<ProductViewModel>();

            foreach (var productOffer in offer.ProductOffers)
            {
                selectecProductList.Add(new ProductViewModel()
                {
                    Id = productOffer.Product.Id,
                    Code = productOffer.Product?.Code,
                    Name = productOffer.Product?.Name,
                    SellPrice = productOffer.Product?.SellPrice,
                    Amount = productOffer.Amount,
                    Discount = productOffer.Discount,
                    KDV = productOffer.Product.Category.KDV,
                    Unit = productOffer.Product?.Unit,
                    Detail = productOffer.Product?.Detail,
                    QRCode = productOffer.Product?.QRCode,
                    Deadline = productOffer.Deadline,
                    Currency = productOffer.Product?.Currency,
                });
            }

            CompanySettingsViewModel companySettingsViewModel = new CompanySettingsViewModel()
            {
                Id = companyId,
                EmailAddress = company?.Email,
                RecipientEmail = companySettings.Where(c => c.Parameter == "AliciEmail")?.FirstOrDefault().Value,
                EmailServerPort = companySettings.Where(c => c.Parameter == "EmailSunucuPort")?.FirstOrDefault().Value,
                EmailServer = companySettings.Where(c => c.Parameter == "EmailSunucu")?.FirstOrDefault().Value,
                EmailUsername = companySettings.Where(c => c.Parameter == "EmailKullaniciAdi")?.FirstOrDefault().Value,
                EmailPassword = companySettings.Where(c => c.Parameter == "EmailParola")?.FirstOrDefault().Value,
                Logo = companySettings.Where(c => c.Parameter == "Logo")?.FirstOrDefault().Value,
                Logo2 = companySettings.Where(c => c.Parameter == "Logo2")?.FirstOrDefault().Value,
                PhoneNumber = companySettings.Where(c => c.Parameter == "TelNo")?.FirstOrDefault().Value,
                FaxNumber = companySettings.Where(c => c.Parameter == "FaxNo")?.FirstOrDefault().Value,
                Address = companySettings.Where(c => c.Parameter == "Adres")?.FirstOrDefault().Value,
                Note = companySettings.Where(c => c.Parameter == "Not")?.FirstOrDefault().Value,

            };

            var customer = await _customerService.GetByIdAsync(19);

            var contactPerson = await _contactPersonService.GetByIdAsync(4);

            var offerViewModel = new OfferViewModel()
            {
                CompanySettingsViewModel = companySettingsViewModel,
                DateOfOffer = DateTime.Now.ToShortTimeString(),
                ProductsViewModel = selectecProductList,
                Company = company,
                Customer = customer,
                CustomerContact = contactPerson,
                OfferTables = offerTables.ToList(),
                OfferNumber = offer.OfferNumber
            };
            TempData["ProductOffer"] = offerViewModel;
            var r = TempData["ProductOffer"];
            return Json(new {status = 200});
        }
    }
}
