using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Core;
using TeklifPanel.Entity;
using TeklifPanelWebUI.Models;

namespace TeklifPanelWebUI.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CategoryList()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var categoryList = await _categoryService.GetCategoriesAsync(companyId);
            return View(categoryList);
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryModel categoryModel)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            if (ModelState.IsValid)
            {
                var category = new Category()
                {
                    Name = categoryModel.Name,
                    Details = categoryModel.Details,
                    CompanyId = companyId,
                    Url = Jobs.MakeUrl(categoryModel.Name),
                    KDV = categoryModel.KDV,
                };
                var isCategoryAdd = await _categoryService.CreateAsync(category);
                if (isCategoryAdd)
                {
                    TempData["Message"] = $"'{category.Name}' adlı kategori eklendi";
                    return RedirectToAction("CategoryList");
                }
                TempData["Error"] = $"'{category.Name}' adlı kategori eklendi";
                return RedirectToAction("CategoryList");

            }
            TempData["Error"] = $"Eksik Bilgiler var!";
            return View();
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeletedCategory = await _categoryService.DeleteCategoryAsync(id);
            var status = isDeletedCategory ? 200 : 400;
            return Json(new { status = status });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            var categoryModel = new CategoryModel()
            {
                Id = id,
                Name = category.Name,
                Details = category.Details,
                KDV = category.KDV
            };
            return View(categoryModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(CategoryModel categoryModel)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;

            if (ModelState.IsValid)
            {
                var category = new Category()
                {
                    Name = categoryModel.Name,
                    Id = categoryModel.Id,
                    Details = categoryModel.Details,
                    Url = Jobs.MakeUrl(categoryModel.Name),
                    CompanyId = companyId,
                    KDV = categoryModel.KDV
                };
                var isUpdateCategory = await _categoryService.UpdateAsync(category);
                if (isUpdateCategory)
                {
                    TempData["Message"] = $"'{category.Name}' adlı ürün güncellendi";
                    return RedirectToAction("UpdateCategory", new { id = categoryModel.Id });
                }
                TempData["Error"] = $"'{category.Name}' adlı ürün güncellenemedi!";
                return RedirectToAction("UpdateCategory", new { id = categoryModel.Id });
            }
            TempData["Error"] = $"Eksik bilgileri doldurun!";
            return RedirectToAction("UpdateCategory", new {id=categoryModel.Id});

        }
    }
}
