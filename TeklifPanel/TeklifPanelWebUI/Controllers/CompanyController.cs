using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Core;
using TeklifPanel.Entity;
using TeklifPanelWebUI.Models;
using TeklifPanelWebUI.ViewModels;

namespace TeklifPanelWebUI.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanySettingsService _companySettingsService;
        private readonly IIbanService _ibanService;
        private readonly IOfferTableService _offerTableService;

        public CompanyController(ICompanyService companyService, ICompanySettingsService companySettingsService, IIbanService ibanService, IOfferTableService offerTableService)
        {
            _companyService = companyService;
            _companySettingsService = companySettingsService;
            _ibanService = ibanService;
            _offerTableService = offerTableService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Settings()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            if (companyId != default)
            {
                var companySettings = await _companySettingsService.GetAllCompanySettingsAsync(companyId);
                var ibans = await _ibanService.GetIbansByCompanyAsync(companyId);
                var offerTables = await _offerTableService.GetManyAsync(o => o.CompanyId == companyId);
                if (companySettings != null && ibans != null)
                {
                    var company = await _companyService.GetByIdAsync(companyId);
                    CompanySettingsViewModel model = new CompanySettingsViewModel()
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
                        OfferTables = offerTables.ToList(),
                    };
                    if (ibans.Count() > 0)
                        model.Ibans = ibans;
                    else
                        model.Ibans = new List<Iban>();
                    return View(model);
                }
                TempData["Error"] = "Ayarlar sayfası açılırken hata oluştu!";
                return Redirect("/Home/Index");
            }
            TempData["Error"] = "Ayarlar sayfası açılırken hata oluştu!";
            return Redirect("/Home/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Settings(CompanySettingsViewModel settingsViewModel, IFormFile logo, IFormFile logo2, List<string> IbanTitle, List<int> menuId, List<int> isShow)
        {

            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var company = await _companyService.GetCompanyByIdAsync(companyId);
            var companySettings = await _companySettingsService.GetAllCompanySettingsAsync(companyId);
            var companyMenu = await _offerTableService.GetManyAsync(x => x.CompanyId == companyId);
            var companyMenuList = companyMenu.ToList();

            var url = Jobs.MakeUrl(companyId + company.Name);

            company.Id = companyId;
            company.Email = settingsViewModel.EmailAddress;

            if (company.Ibans.Count() == IbanTitle.Count())
            {
                for (var i = 0; i < IbanTitle.Count(); i++)
                {
                    if (!String.IsNullOrEmpty(IbanTitle[i]) && IbanTitle[i].Contains('&'))
                    {
                        var title = IbanTitle[i].Split('&')[0];
                        var ibanNumber = IbanTitle[i].Split('&')[1];
                        if (String.IsNullOrWhiteSpace(title) || String.IsNullOrWhiteSpace(ibanNumber))
                        {
                            TempData["Error"] = "'Banka Adı' veya 'IBAN' boş bırakılamaz";
                            return RedirectToAction("Settings");
                        }

                        company.Ibans[i].IbanNumber = ibanNumber;
                        company.Ibans[i].Title = title;
                    }
                }
            }
            else
            {
                for (int i = company.Ibans.Count(); i < IbanTitle.Count(); i++)
                {
                    if (!String.IsNullOrEmpty(IbanTitle[i]) && IbanTitle[i].Contains('&'))
                    {
                        var title = IbanTitle[i].Split('&')[0];
                        var ibanNumber = IbanTitle[i].Split('&')[1];
                        if (String.IsNullOrWhiteSpace(title) || String.IsNullOrWhiteSpace(ibanNumber))
                        {
                            TempData["Error"] = "'Banka Adı' veya 'IBAN' boş bırakılamaz";
                            return RedirectToAction("Settings");
                        }

                        company.Ibans.Add(new Iban
                        {
                            CompanyId = companyId,
                            IbanNumber = ibanNumber,
                            Title = title
                        });
                    }
                }

            }

            await _companyService.UpdateAsync(company);

            companySettings.Where(c => c.Parameter == "AliciEmail").FirstOrDefault().Value = settingsViewModel?.RecipientEmail;
            companySettings.Where(c => c.Parameter == "EmailSunucuPort").FirstOrDefault().Value = settingsViewModel?.EmailServerPort;
            companySettings.Where(c => c.Parameter == "EmailSunucu").FirstOrDefault().Value = settingsViewModel?.EmailServer;
            companySettings.Where(c => c.Parameter == "EmailKullaniciAdi").FirstOrDefault().Value = settingsViewModel?.EmailUsername;
            companySettings.Where(c => c.Parameter == "EmailParola").FirstOrDefault().Value = settingsViewModel?.EmailPassword;
            if (logo != null)
                companySettings.Where(c => c.Parameter == "Logo").FirstOrDefault().Value = Jobs.UploadImage(logo, url, companyId);
            if (logo2 != null)
                companySettings.Where(c => c.Parameter == "Logo2").FirstOrDefault().Value = Jobs.UploadImage(logo2, url, companyId);
            companySettings.Where(c => c.Parameter == "TelNo").FirstOrDefault().Value = settingsViewModel?.PhoneNumber;
            companySettings.Where(c => c.Parameter == "FaxNo").FirstOrDefault().Value = settingsViewModel?.FaxNumber;
            companySettings.Where(c => c.Parameter == "Adres").FirstOrDefault().Value = settingsViewModel?.Address;
            companySettings.Where(c => c.Parameter == "Not").FirstOrDefault().Value = settingsViewModel?.Note;

            for (int i = 0; i < companyMenu.Count(); i++)
            {
                //companyMenuList[menuId[i] - 1].SiraNo = i;
                //companyMenuList[i].SiraNo = menuId.Contains(companyMenuList[i].Id) ? i : default;
                companyMenuList[i].IsShow = isShow.Contains(companyMenuList[i].Id) ? true : false;
                await _offerTableService.UpdateAsync(companyMenuList[i]);
            }

            var index = 0;
            foreach (var menuItem in menuId)
            {
                var r = companyMenuList.Where(x => x.Id == menuItem).FirstOrDefault();
                r.SiraNo = index;
                index++;
                await _offerTableService.UpdateAsync(r);
            }

            var result = await _companySettingsService.UpdateCompanySettingAsync(companySettings);
            if (result)
            {
                TempData["Message"] = "Ayarlar güncellendi";
                return RedirectToAction("Settings");
            }

            TempData["Error"] = "Ayarlar güncellenmedi";
            return RedirectToAction("Settings");
        }

        public async Task<IActionResult> IbanDelete(int id)
        {
            var iban = await _ibanService.GetByIdAsync(id);
            if (iban == null)
                return RedirectToAction("Settings");
            var result = await _ibanService.DeleteAsync(iban);
            if (result)
            {
                TempData["Message"] = "IBAN silindi";
                return RedirectToAction("Settings");
            }

            TempData["Error"] = "Iban silinemedi";
            return RedirectToAction("Settings");

        }
    }
}
