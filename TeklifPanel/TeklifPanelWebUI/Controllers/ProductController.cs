using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QRCoder;
using System;
using System.ComponentModel.Design;
using System.Drawing.Imaging;
using System.Drawing;
using System.Xml;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Core;
using TeklifPanel.Entity;
using TeklifPanelWebUI.Models;
using TeklifPanelWebUI.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace TeklifPanelWebUI.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly ICategoryService _categoryService;
        private readonly IImageService _imageService;

        private IOptions<CurrenciesModel> DefaultData;
        private readonly IWritableOptions<CurrenciesModel> _writableOptions;
        private readonly IConfiguration _configuration;

        public ProductController(IProductService productService, ICompanyService companyService, ICategoryService categoryService, IImageService imageService, IOptions<CurrenciesModel> defaultData, IWritableOptions<CurrenciesModel> writableOptions, IConfiguration configuration)
        {
            _productService = productService;
            _companyService = companyService;
            _categoryService = categoryService;
            _imageService = imageService;
            DefaultData = defaultData;
            _writableOptions = writableOptions;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProductList(int id)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            ViewBag.Categories = await _categoryService.GetCategoriesAsync(companyId);

            if (id == 0)
            {
                var productList = await _productService.GetCompanyProductsAsync(companyId);
                return View(productList);
            }

            var productsByCategory = await _productService.GetProductsByCategoryAsync(companyId, id);
            return View(productsByCategory);
        }
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            ViewBag.Categories = await _categoryService.GetCategoriesAsync(companyId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductViewModel productViewModel, List<IFormFile> images)
        {
            string panelUrl = _configuration["Url:PanelUrl"];

            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var productList = await _productService.GetCompanyProductsAsync(companyId);

            ViewBag.Categories = await _categoryService.GetCategoriesAsync(companyId);
            var url = Jobs.MakeUrl(productViewModel.Name);

            if (ModelState.IsValid)
            {
                var product = new Product()
                {
                    BuyPrice = productViewModel?.BuyPrice,
                    Code = productViewModel?.Code,
                    Detail = productViewModel?.Detail,
                    IsActive = productViewModel.IsActive,
                    Name = productViewModel?.Name,
                    SellPrice = productViewModel?.SellPrice,
                    Stock = productViewModel.Stock,
                    CompanyId = companyId,
                    CategoryId = productViewModel.CategoryId,
                    Url = url,
                    Unit = productViewModel?.Unit,
                    Currency = productViewModel?.Currency,
                };

                var isCreateProduct = await _productService.CreateAsync(product);
                var qrCode = Jobs.GenerateQrCodeFromUrl(panelUrl + "/Product/ProductCard/" + product.Url);
                //var qrCode = Jobs.GenerateQrCodeFromUrl("http://teklif.ttr.com.tr/Product/ProductCard/" + product.Id);

                product.QRCode = qrCode;

                if (isCreateProduct)
                {
                    foreach (var image in images)
                    {
                        product.ProductImages.Add(new ProductImage()
                        {
                            Name = productViewModel.Name,
                            Url = Jobs.UploadImage(image, url, companyId),
                            ProductId = product.Id
                        });
                    }
                    var result = await _productService.UpdateAsync(product);
                    TempData["Message"] = $"'{product.Name}' adlı ürün eklendi";
                    return RedirectToAction("ProductList");
                }
                TempData["Error"] = "Ürün Eklenemedi!";
                return RedirectToAction("ProductList");
            }
            TempData["Error"] = "Eksik bilgi var!";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            ViewBag.Categories = await _categoryService.GetCategoriesAsync(companyId);
            var product = await _productService.GetProductByIdAsync(id);

            var productViewModel = new ProductViewModel()
            {
                BuyPrice = product.BuyPrice,
                CategoryId = product.CategoryId,
                Code = product.Code,
                Detail = product.Detail,
                IsActive = product.IsActive,
                Name = product.Name,
                SellPrice = product.SellPrice,
                Stock = product.Stock,
                Id = id,
                ProductImages = product.ProductImages,
                CompanyId = companyId,
                Unit = product.Unit,
                Currency = product.Currency,
            };
            return View(productViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductViewModel productViewModel, List<IFormFile> images)
        {
            string panelUrl = _configuration["Url:PanelUrl"];
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            ViewBag.Categories = await _categoryService.GetCategoriesAsync(companyId);
            var url = Jobs.MakeUrl(productViewModel.Name);
            if (ModelState.IsValid)
            {
                var product = new Product()
                {
                    CompanyId = companyId,
                    Id = productViewModel.Id,
                    BuyPrice = productViewModel.BuyPrice,
                    SellPrice = productViewModel.SellPrice,
                    Stock = productViewModel.Stock,
                    Code = productViewModel.Code,
                    Detail = productViewModel.Detail,
                    IsActive = productViewModel.IsActive,
                    CategoryId = productViewModel.CategoryId,
                    Name = productViewModel.Name,
                    Unit = productViewModel?.Unit,
                    Currency = productViewModel?.Currency,
                    Url = url,
                    QRCode = Jobs.GenerateQrCodeFromUrl(panelUrl + "/Product/ProductCard/" + url)
            };

                if (images != null && images.Count() > 0)
                {
                    foreach (var image in images)
                    {
                        product.ProductImages.Add(new ProductImage()
                        {
                            Name = productViewModel.Name,
                            Url = Jobs.UploadImage(image, url, companyId),
                            ProductId = product.Id
                        });
                    }
                }
                var updateProduct = await _productService.UpdateAsync(product);
                if (updateProduct)
                {
                    TempData["Message"] = $"'{product.Name}' adlı ürün güncelledi";
                    return RedirectToAction("UpdateProduct");
                }
                TempData["Error"] = $"'{product.Name}' adlı ürün güncellenirken bir hata oluştu";
                return View(productViewModel);
            }
            TempData["Error"] = "Eksik bilgi var!";
            return View(productViewModel);
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var isDeleteProduct = await _productService.DeleteProductAsync(id);
            var status = isDeleteProduct ? 200 : 400;
            return Json(new { status = status });
        }

        public async Task<IActionResult> DeleteProductImage(int id)
        {
            var image = await _imageService.GetByIdAsync(id);
            if (image == null)
            {
                return View();
            }
            var isDeletedImage = await _imageService.DeleteAsync(image);
            if (isDeletedImage)
            {
                TempData["Message"] = "Resim silindi";
                return RedirectToAction("ProductList");
            }
            TempData["Error"] = "Resim silinemedi";
            return RedirectToAction("ProductList");
        }

        public async Task<IActionResult> Search(string searchWord)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var search = await _productService.GetSearchProduct(companyId, searchWord);

            return View(search);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ProductCard(string url)
        {
            var product = await _productService.GetProductByUrlAsync(url);
            return View(product);
        }
        public ActionResult GenerateQRCode(string data)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, ImageFormat.Png);
                byte[] imageBytes = stream.ToArray();
                ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
            }

            return View();
        }

        public ActionResult AddProductFromExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddProductFromExcel(IFormFile excel)
        {
            string panelUrl = _configuration["Url:PanelUrl"];

            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var result = Jobs.UploadExcel(excel, excel.Name, companyId);
            var categoryList = await _categoryService.GetCategoriesAsync(companyId);

            FileInfo fileInfo = new FileInfo(result);
            ExcelPackage excelPackage = new ExcelPackage(fileInfo);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorkbook workbook = excelPackage.Workbook;
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];
            var productList = await _productService.GetCompanyProductsAsync(companyId);
            for (int i = 2; i <= workbook.Worksheets.Count(); i++)
            {

                var isProduct = productList.Where(p => p.Name == worksheet.Cells[i, 3].Value.ToString()).SingleOrDefault();
                if (isProduct != null)
                    continue;

                var category = categoryList.Where(c => c.Name == worksheet.Cells[i, 1].Value.ToString()).SingleOrDefault();
                var categoryModel = new Category();
                if (category == null)
                {
                    categoryModel.Name = worksheet.Cells[i, 1].Value.ToString();
                    categoryModel.KDV = decimal.TryParse(worksheet.Cells[i, 2].Value.ToString(), out decimal decimalKdv) ? decimalKdv : default;
                    categoryModel.CompanyId = companyId;
                    categoryModel.Url = Jobs.MakeUrl(worksheet.Cells[i, 1].Value.ToString());
                    await _categoryService.CreateAsync(categoryModel);
                }
                var product = new Product();

                product.CategoryId = category == null ? categoryModel.Id : category.Id;
                product.Name = worksheet.Cells[i, 3].Value.ToString() ?? default;
                product.Code = worksheet.Cells[i, 4].Value.ToString() ?? default;
                product.BuyPrice = decimal.TryParse(worksheet.Cells[i, 5].Value.ToString(), out decimal decimalBuy) ? decimalBuy : default;
                product.SellPrice = decimal.TryParse(worksheet.Cells[i, 6].Value.ToString(), out decimal decimalSell) ? decimalSell : default;
                product.Stock = int.TryParse(worksheet.Cells[i, 7].Value.ToString(), out int intStock) ? intStock : default;
                product.Unit = worksheet.Cells[i, 8].Value.ToString();
                product.Currency = worksheet.Cells[i, 9].Value.ToString();
                product.Detail = worksheet.Cells[i, 10].Value.ToString();
                product.IsActive = worksheet.Cells[i, 12].Value.ToString().Trim() == "Evet" ? true : false;
                product.CompanyId = companyId;
                product.Url = Jobs.MakeUrl(worksheet.Cells[i, 3].Value.ToString());

                await _productService.CreateAsync(product);
                var qrCode = Jobs.GenerateQrCodeFromUrl(panelUrl + "/Product/ProductCard/" + product.Id);
                product.QRCode = qrCode;

                var productImage = new ProductImage();
                productImage.Name = product.Name;
                productImage.Url = worksheet.Cells[i, 11].Value.ToString() ?? default;
                productImage.ProductId = product.Id;

                product.ProductImages.Add(productImage);

                await _productService.UpdateAsync(product);
            }

            return View();
        }

        [HttpPost]
        public IActionResult AddProductImageForm()
        {
            var images = Request.Form.Files;
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            foreach (var image in images)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Content/images/C" + companyId);
                Directory.CreateDirectory(folderPath);
                var path = Path.Combine(folderPath, image.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
            }
            return Json(new { status = 200 });
        }

        [HttpPost]
        public int DeleteFile(string fileName)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Content/images/C" + companyId);

            try
            {
                if (System.IO.File.Exists(Path.Combine(path, fileName)))
                    System.IO.File.Delete(Path.Combine(path, fileName));
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return View(product);
        }
    }
}

