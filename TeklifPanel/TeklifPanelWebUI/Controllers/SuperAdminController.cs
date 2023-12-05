using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Core;
using TeklifPanel.Entity;
using TeklifPanelWebUI.Models;

namespace TeklifPanelWebUI.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ICompanyService _companyService;
        private readonly ICompanySettingsService _companySettingsServise;

        public SuperAdminController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ICompanyService companyService, ICompanySettingsService companySettingsServise)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _companyService = companyService;
            _companySettingsServise = companySettingsServise;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> RoleList()
        {
            var roleList = await _roleManager.Roles.ToListAsync();
            return View(roleList);
        }
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = roleModel.Name,
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
            }
            return View(roleModel);
        }
        [HttpGet]
        public IActionResult CompanyCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CompanyCreate(CompanyModel companyModel)
        {
            Random rnd = new Random();
            if (ModelState.IsValid)
            {
                var company = new Company()
                {
                    Name = companyModel.Name,
                    Email = companyModel.Email,
                    IsActive = true

                };

                var companyIsAdd = await _companyService.CreateAsync(company);
                if (!companyIsAdd)
                {
                    ViewBag.Error = "Firma ekleme aşamasında bir hata oluştu!";
                    return View();
                }

                var lastCompany = await _companyService.GetLastCompany();
                if (lastCompany == null)
                {
                    ViewBag.Error = "Eklenen firmanın id'sini alma aşamasında bir hata oluştu!";
                    return View();
                }

                User user = new User()
                {
                    FirstName = companyModel.Name,
                    Email = companyModel.Email,
                    IsOfferShow = true,
                };

                await _companySettingsServise.CreateCompanySettingsAsync(lastCompany.Id);

                user.CompanyId = lastCompany.Id;
                user.UserName = Jobs.MakeUserName(companyModel.Name) + "-" + rnd.Next(1111, 9999);

                var result = await _userManager.CreateAsync(user, companyModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    TempData["Message"] = $"Firma başarıyla eklendi Firma Adı: {companyModel.Name} - Email: {companyModel.Email} - Şifre: {companyModel.Password}";
                }
                else
                {
                    TempData["Error"] = "Firma 'Company' classına eklendi fakat User Listesine eklenemedi. Bilgileri kontrol edip tekrar deneyiniz! Ayrıca Eklenen firmayı listeden silebilirsini<";
                    return RedirectToAction("CompanyList");
                }
                return RedirectToAction("CompanyList");
            }
            ViewBag.Error = "Eksik ya da yanlış girilen bilgiler var. Bilgileri kontrol edip tekrar deneyiniz!";
            return View(companyModel);
        }
        public async Task<IActionResult> CompanyList()
        {
            var companList = await _companyService.GetAll();

            return View(companList);
        }

        public async Task<IActionResult> CompanyDelete(int id)
        {
            Company company = await _companyService.GetByIdAsync(id);
            if (company == null)
            {
                TempData["Error"] = "Firma bulunamadı!";
                return RedirectToAction("CompanyList");
            }
            company.IsActive = company.IsActive ? false : true;
            var updateCompany = await _companyService.UpdateAsync(company);
            if (updateCompany)
            {
                TempData["Message"] = $"Firma {company.Name} başarıyla silindi";
                return RedirectToAction("CompanyList");
            }
            TempData["Error"] = "Firma silinemedi!";
            return RedirectToAction("CompanyList");
        }

        public async Task<IActionResult> CompanyDeleteList()
        {
            var compantDeleteList = await _companyService.GetAll();
            return View(compantDeleteList);
        }

        public async Task<IActionResult> CompanyUnDelete(int id)
        {
            Company company = await _companyService.GetByIdAsync(id);
            if (company == null)
            {
                TempData["Error"] = "Firma bulunamadı!";
                return RedirectToAction("CompanyDeleteList");

            }
            company.IsActive = company.IsActive ? false : true;
            var updateCompany = await _companyService.UpdateAsync(company);
            if (updateCompany)
            {
                TempData["Message"] = $"Firma {company.Name} başarıyla geri yüklendi";
                return RedirectToAction("CompanyDeleteList");
            }
            TempData["Error"] = "Firma geri yüklenemedi";
            return RedirectToAction("CompanyDeleteList");
        }

        public async Task<IActionResult> CompanyPermanentlyDelete(int id)
        {
            Company company = await _companyService.GetByIdAsync(id);
            if (company == null)
            {
                TempData["Error"] = "Firma bulunamadı!";
                return RedirectToAction("CompanyDeleteList");
            }
            var deleteCompany = await _companyService.DeleteAsync(company);

            if (deleteCompany)
            {
                TempData["Message"] = $"Firma {company.Name} kalıcı olarak silindi";
                return RedirectToAction("CompanyDeleteList");
            }
            TempData["Error"] = "Firma silme aşamasında bir hata oluştu";
            return RedirectToAction("CompanyDeleteList");
        }

        public async Task<IActionResult> CompanyEdit(int id)
        {
            var company = await _companyService.GetByIdAsync(id);
            CompanyModel companyModel = new CompanyModel()
            {
                Id = id,
                Email = company.Email,
                Name = company.Name,
            };
            return View(companyModel);
        }

        [HttpPost]
        public async Task<IActionResult> CompanyEdit(CompanyModel companyModel)
        {
            if (companyModel == null)
            {
                TempData["Error"] = "Firma güncelleme aşamasında bir hata oluştu";
                return RedirectToAction("CompanyList");
            }
            Company company = new Company()
            {
                Id = companyModel.Id,
                Name = companyModel.Name,
                Email = companyModel.Email,
                IsActive = true
            };


            var userCompany = await _userManager.FindByEmailAsync(company.Email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(userCompany);

            var r = await _userManager.ResetPasswordAsync(userCompany, token,companyModel.Password);

            var companyUpdate = await _companyService.UpdateAsync(company);

            if (!companyUpdate)
            {
                TempData["Error"] = "Firma güncelleme aşamasında bir hata oluştu";
                return RedirectToAction("CompanyList");
            }

            TempData["Message"] = "Firma güncellendi";
            return RedirectToAction("CompanyList");
        }
    }
}
