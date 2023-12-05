using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TeklifPanel.Entity;
using TeklifPanelWebUI.Models;

namespace TeklifPanelWebUI.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;

        private readonly IConfiguration _configuration;

        public NavBarViewComponent(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");

            string usdExchangeRate = _configuration["CurrenciesModel:BanknoteSellingUSD"];
            string euExchangeRate = _configuration["CurrenciesModel:BanknoteSellingEUR"];
            string gbpExchangeRate = _configuration["CurrenciesModel:BanknoteSellingGBP"];
            string bultenExchangeRate = _configuration["CurrenciesModel:Bulten"];
            var user = await _userManager.FindByIdAsync(userId);
            var navbarModel = new NavBarModel()
            {
                UserName = user?.FirstName + " " + user?.LastName,
                USD = usdExchangeRate,
                EURO = euExchangeRate,
                GBP= gbpExchangeRate,
                Date = bultenExchangeRate
            };
            return View(navbarModel);
        }
    }
}
