using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Core;
using TeklifPanel.Entity;
using TeklifPanelWebUI.ViewModels;
using DinkToPdf;
using System.ComponentModel.Design;
using System.Security.Policy;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TeklifPanelWebUI.Models;
using System.Xml;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace TeklifPanelWebUI.Controllers
{
    [Authorize]
    public class OfferController : Controller
    {
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

        private IOptions<CurrenciesModel> DefaultData;
        private readonly IWritableOptions<CurrenciesModel> _writableOptions;
        private readonly IConfiguration _configuration;

        public OfferController(ICustomerService customerService, IProductService productService, ICategoryService categoryService, IContactPersonService contactPersonService, ICompanyService companyService, ICompanySettingsService companySettingsService, IOfferService offerService, IConverter pdfConverter, UserManager<User> userManager, IOptions<CurrenciesModel> defaultData, IWritableOptions<CurrenciesModel> writableOptions, IConfiguration configuration, IOfferTableService offerTableService)
        {
            _customerService = customerService;
            _productService = productService;
            _categoryService = categoryService;
            _contactPersonService = contactPersonService;
            _companyService = companyService;
            _companySettingsService = companySettingsService;
            _offerService = offerService;
            _pdfConverter = pdfConverter;
            _userManager = userManager;
            DefaultData = defaultData;
            _writableOptions = writableOptions;
            _configuration = configuration;
            _offerTableService = offerTableService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OfferList()
        {

            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var userId = HttpContext.Session.GetString("UserId") ?? default;

            var user = await _userManager.FindByIdAsync(userId);
            var offerList = await _offerService.GetCompanyOffersAsync(companyId);

            var offerModel = new OfferListViewModel()
            {
                Offers = offerList,
                User = user,
            };

            return View(offerModel);
        }
        public async Task<IActionResult> AddOffer(int offerId)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var customerList = await _customerService.GetCompanyByCustomersAsync(companyId);
            ViewBag.Company = await _companyService.GetCompanyByIdAsync(companyId);
            ViewBag.Categories = await _categoryService.GetCategoriesAsync(companyId);
            if (offerId > 0)
            {
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
                        CategoryName = productOffer.Product?.Category.Name,
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

                var customer = await _customerService.GetByIdAsync(offer.CustomerId);

                var contactPerson = await _contactPersonService.GetByIdAsync(offer.CustomerContactId);

                var offerViewModel = new OfferViewModel()
                {
                    CompanySettingsViewModel = companySettingsViewModel,
                    DateOfOffer = DateTime.Now.ToShortTimeString(),
                    ProductsViewModel = selectecProductList,
                    Company = company,
                    Customer = customer,
                    CustomerContact = contactPerson,
                    OfferTables = offerTables.ToList(),
                    OfferNumber = offer.OfferNumber,
                    CurrencyType = offer.CurrenyType,
                    Currency = offer.Currency
                };
                ViewBag.ProductOffer = offerViewModel;

                return View(customerList);
            }
            ViewBag.ProductOffer = null;
            return View(customerList);
        }

        [HttpGet]
        public async Task<IActionResult> ContactPersons(int customerId)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;

            var customerContactPersons = await _contactPersonService.GetCustomerContacts(companyId, customerId);
            return View(customerContactPersons);
        }

        public async Task<IActionResult> GetProducts(int categoryId, int customerId, string currency, string currencyType)
        {
            decimal katSayi = 1;

            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var userId = HttpContext.Session.GetString("UserId") ?? default;
            var user = await _userManager.FindByIdAsync(userId);

            var productList = await _productService.GetProductsByCategoryAsync(companyId, categoryId);
            var customer = await _customerService.GetByIdAsync(customerId);
            var category = await _categoryService.GetByIdAsync(categoryId);

            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.NumberDecimalSeparator = ".";

            string usdBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingUSD"];
            string eurBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingEUR"];
            string gbpBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingGBP"];

            var usdBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingUSD"];
            var eurBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingEUR"];
            var gbpBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingGBP"];

            var usdForexSelling = _configuration["CurrenciesModel:ForexSellingUSD"];
            var eurForexSelling = _configuration["CurrenciesModel:ForexSellingEUR"];
            var gbpForexSelling = _configuration["CurrenciesModel:ForexSellingGBP"];

            var usdForexBuying = _configuration["CurrenciesModel:ForexBuyingUSD"];
            var eurForexBuying = _configuration["CurrenciesModel:ForexBuyingEUR"];
            var gbpForexBuying = _configuration["CurrenciesModel:ForexBuyingGBP"];

            var productViewModel = new List<ProductViewModel>();
            var offers = await _offerService.GetCustomerOffersAsync(customerId);
            foreach (var product in productList.Where(x => x.IsActive))
            {
                var productOffers = offers
                    .Where(offer => offer.ProductOffers.Any(po => po.ProductId == product.Id))
                    .SelectMany(offer => offer.ProductOffers)
                    .Where(po => po.ProductId == product.Id)
                    .ToList();

                if (product.Currency == "usd")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(usdBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(usdBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(usdForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(usdForexBuying, numberFormat);
                }
                else if (product.Currency == "eu")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(eurBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(eurBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(eurForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(eurForexBuying, numberFormat);
                }
                else if (product.Currency == "gbp")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(gbpBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(gbpBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(gbpForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(gbpForexBuying, numberFormat);
                }

                productViewModel.Add(new ProductViewModel()
                {
                    Id = product.Id,
                    Code = product?.Code,
                    Name = product?.Name,
                    SellPrice = product?.SellPrice * (currencyType == "tl" ? katSayi : 1),
                    Detail = product?.Detail,
                    Stock = product.Stock,
                    Discount = customer?.Discount,
                    Images = product?.ProductImages.Select(p => p.Url).ToList(),
                    CompanyId = product.CompanyId,
                    CategoryName = category?.Name,
                    ProductOffers = productOffers,
                    User = user,
                    Currency = currencyType == "tl" ? "tl" : product?.Currency,
                });

            }

            return View(productViewModel);
        }

        public async Task<IActionResult> GetProductsMobil(int categoryId, int customerId, string currency, string currencyType)
        {

            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var userId = HttpContext.Session.GetString("UserId") ?? default;
            var user = await _userManager.FindByIdAsync(userId);

            var productList = await _productService.GetProductsByCategoryAsync(companyId, categoryId);
            var customer = await _customerService.GetByIdAsync(customerId);
            var category = await _categoryService.GetByIdAsync(categoryId);

            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.NumberGroupSeparator = ",";
            decimal katSayi = 1;

            string usdBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingUSD"];
            string eurBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingEUR"];
            string gbpBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingGBP"];

            var usdBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingUSD"];
            var eurBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingEUR"];
            var gbpBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingGBP"];

            var usdForexSelling = _configuration["CurrenciesModel:ForexSellingUSD"];
            var eurForexSelling = _configuration["CurrenciesModel:ForexSellingEUR"];
            var gbpForexSelling = _configuration["CurrenciesModel:ForexSellingGBP"];

            var usdForexBuying = _configuration["CurrenciesModel:ForexBuyingUSD"];
            var eurForexBuying = _configuration["CurrenciesModel:ForexBuyingEUR"];
            var gbpForexBuying = _configuration["CurrenciesModel:ForexBuyingGBP"];

            var productViewModel = new List<ProductViewModel>();
            var offers = await _offerService.GetCustomerOffersAsync(customerId);
            foreach (var product in productList.Where(x => x.IsActive))
            {
                var offerDiscount = offers
                    .Where(offer => offer.ProductOffers.Any(po => po.ProductId == product.Id))
                    .SelectMany(offer => offer.ProductOffers)
                    .Where(po => po.ProductId == product.Id)
                    .ToList();

                if (product.Currency == "usd")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(usdBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(usdBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(usdForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(usdForexBuying, numberFormat);
                }
                else if (product.Currency == "eu")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(eurBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(eurBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(eurForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(eurForexBuying, numberFormat);
                }
                else if (product.Currency == "gbp")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(gbpBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(gbpBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(gbpForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(gbpForexBuying, numberFormat);
                }

                productViewModel.Add(new ProductViewModel()
                {
                    Id = product.Id,
                    Code = product?.Code,
                    Name = product?.Name,
                    SellPrice = product?.SellPrice * (currencyType == "tl" ? katSayi : 1),
                    Detail = product?.Detail,
                    Stock = product.Stock,
                    Discount = customer?.Discount,
                    Images = product?.ProductImages.Select(p => p.Url).ToList(),
                    CompanyId = product.CompanyId,
                    CategoryName = category?.Name,
                    ProductOffers = offerDiscount,
                    User = user,
                    Currency = currencyType == "tl" ? "tl" : product?.Currency,
                });

            }

            return View(productViewModel);
        }


        public async Task<IActionResult> OfferPreview(List<int> Amount, List<decimal> Discount, int CustomerId, List<int> Id, int ContactPersonId, List<string> Deadline, string currency, string currencyType)
        {
            Random random = new Random();

            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var company = await _companyService.GetByIdAsync(companyId);

            var companySettings = await _companySettingsService.GetAllCompanySettingsAsync(companyId);

            var customer = await _customerService.GetByIdAsync(CustomerId);

            var contactPerson = await _contactPersonService.GetByIdAsync(ContactPersonId);

            var offerTable = await _offerTableService.GetManyAsync(x => x.CompanyId == companyId);

            var selectecProductList = new List<ProductViewModel>();

            decimal katSayi = 1;

            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.NumberDecimalSeparator = ".";

            string usdBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingUSD"];
            string eurBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingEUR"];
            string gbpBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingGBP"];

            var usdBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingUSD"];
            var eurBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingEUR"];
            var gbpBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingGBP"];

            var usdForexSelling = _configuration["CurrenciesModel:ForexSellingUSD"];
            var eurForexSelling = _configuration["CurrenciesModel:ForexSellingEUR"];
            var gbpForexSelling = _configuration["CurrenciesModel:ForexSellingGBP"];

            var usdForexBuying = _configuration["CurrenciesModel:ForexBuyingUSD"];
            var eurForexBuying = _configuration["CurrenciesModel:ForexBuyingEUR"];
            var gbpForexBuying = _configuration["CurrenciesModel:ForexBuyingGBP"];

            for (int i = 0; i < Id.Count(); i++)
            {
                var selectedProduct = await _productService.GetProductByIdAsync(Id[i]);

                if (selectedProduct.Currency == "usd")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(usdBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(usdBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(usdForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(usdForexBuying, numberFormat);
                }
                else if (selectedProduct.Currency == "eu")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(eurBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(eurBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(eurForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(eurForexBuying, numberFormat);
                }
                else if (selectedProduct.Currency == "gbp")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(gbpBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(gbpBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(gbpForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(gbpForexBuying, numberFormat);
                }

                var selectedCategory = await _categoryService.GetByIdAsync(selectedProduct.CategoryId);
                selectecProductList.Add(new ProductViewModel()
                {
                    Id = selectedProduct.Id,
                    Code = selectedProduct?.Code,
                    Name = selectedProduct?.Name,
                    SellPrice = selectedProduct?.SellPrice * (currencyType == "tl" ? katSayi : 1),
                    Amount = Amount[i],
                    Discount = Discount[i],
                    KDV = selectedCategory.KDV,
                    Unit = selectedProduct?.Unit,
                    Detail = selectedProduct?.Detail,
                    QRCode = selectedProduct?.QRCode,
                    Deadline = Deadline[i],
                    Currency = currencyType == "tl" ? "tl" : selectedProduct?.Currency,
                    Images = selectedProduct?.ProductImages.Select(p => p.Url).ToList(),
                    CompanyId = companyId,
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

            var offerViewModel = new OfferViewModel()
            {
                CompanySettingsViewModel = companySettingsViewModel,
                DateOfOffer = DateTime.Now.ToShortTimeString(),
                ProductsViewModel = selectecProductList,
                Company = company,
                Customer = customer,
                CustomerContact = contactPerson,
                OfferNumber = random.Next(11111, 99999),
                OfferTables = offerTable.ToList(),
                CurrencyType = currencyType,
                Currency = currency,
            };

            return View(offerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendMail(IFormFile pdfFile, List<int> Id, int CustomerId, int OfferNumber, int ContactPersonId, decimal Total, decimal Discount, string EditNote, List<string> CcEmail, List<string> ProductIds, string ProductDiscount, string MailBody, string Deadline, string Amounts, string CurrencyType, string Currency)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var userId = HttpContext.Session.GetString("UserId") ?? default;

            var user = await _userManager.FindByIdAsync(userId);
            var company = await _companyService.GetByIdAsync(companyId);

            var companySettings = await _companySettingsService.GetAllCompanySettingsAsync(companyId);

            var customer = await _customerService.GetByIdAsync(CustomerId);

            var contactPerson = await _contactPersonService.GetByIdAsync(ContactPersonId);

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
            };

            var sunucu = companySettingsViewModel.EmailServer;
            var port = companySettingsViewModel.EmailServerPort;
            var mail = companySettingsViewModel.EmailUsername;
            var sifre = companySettingsViewModel.EmailPassword;
            var aliciEmail = companySettingsViewModel.RecipientEmail;

            var fromAddress = new MailAddress(mail, "Teklif");
            var toAddress = new MailAddress(aliciEmail);

            var contactPersonEmail = new MailAddress(contactPerson.Email);

            SmtpClient smtp = new SmtpClient();
            try
            {
                smtp.Host = sunucu;
                smtp.Port = Convert.ToInt32(port);
                smtp.EnableSsl = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(mail, sifre);
            }
            catch
            {
                smtp.Host = sunucu;
                smtp.Port = Convert.ToInt32(port);
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(mail, sifre);
            }

            var message = new MailMessage(fromAddress, toAddress);
            message.Body = String.Format(@"Merhaba, <br/><b>" + customer.Name + "</b> adlı firmaya '" + user.FirstName + "' kişi teklif gönderdi");
            message.Subject = customer.Name;
            message.IsBodyHtml = true;


            var ccEmailSplit = CcEmail.FirstOrDefault()?.Split(",");
            if (ccEmailSplit != null)
            {
                foreach (var cc in ccEmailSplit)
                {
                    message.CC.Add(cc);
                }
            }

            var message2 = new MailMessage(fromAddress, contactPersonEmail);
            if (MailBody == null)
                message2.Body = String.Format(@"Merhaba, <br/><b>" + customer.Name + "</b> adlı firmadan teklif gönderildi");
            else
                message2.Body = MailBody; message2.Subject = company.Name;
            message2.IsBodyHtml = true;

            var pdf = Jobs.UploadPdf(pdfFile, customer.Name, companyId, company.Name);

            var pdfSplit = pdf.Split("\\");
            var pdfUrl = pdfSplit[pdfSplit.Length - 1];

            // PDF dosyasını ekleyin
            var attachment = new Attachment(pdf);

            message.Attachments.Add(attachment);
            message2.Attachments.Add(attachment);

            smtp.Send(message);
            smtp.Send(message2);

            var productIdSplit = ProductIds.FirstOrDefault()?.Split(",");
            var productDiscountSplit = ProductDiscount.Split(",");
            var productDeadlineSplit = Deadline?.Split(",");
            var productAmountSplit = Amounts?.Split(",");

            if (productIdSplit != null)
            {
                var productOffers = new List<ProductOffer>(); // ProductOffer sınıfınıza uygun bir koleksiyon türü kullanın

                decimal katSayi = 1;

                NumberFormatInfo numberFormat = new NumberFormatInfo();
                numberFormat.NumberDecimalSeparator = ".";

                string usdBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingUSD"];
                string eurBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingEUR"];
                string gbpBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingGBP"];

                var usdBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingUSD"];
                var eurBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingEUR"];
                var gbpBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingGBP"];

                var usdForexSelling = _configuration["CurrenciesModel:ForexSellingUSD"];
                var eurForexSelling = _configuration["CurrenciesModel:ForexSellingEUR"];
                var gbpForexSelling = _configuration["CurrenciesModel:ForexSellingGBP"];

                var usdForexBuying = _configuration["CurrenciesModel:ForexBuyingUSD"];
                var eurForexBuying = _configuration["CurrenciesModel:ForexBuyingEUR"];
                var gbpForexBuying = _configuration["CurrenciesModel:ForexBuyingGBP"];

                for (int i = 0; i < productIdSplit.Length; i++)
                {
                    var productId = int.Parse(productIdSplit[i]);
                    var selectedProduct = await _productService.GetProductByIdAsync(productId);

                    if (selectedProduct.Currency == "usd")
                    {
                        if (Currency == "BS")
                            katSayi = decimal.Parse(usdBanknoteSelling, numberFormat);
                        else if (Currency == "BB")
                            katSayi = decimal.Parse(usdBanknoteBuying, numberFormat);
                        else if (Currency == "FS")
                            katSayi = decimal.Parse(usdForexSelling, numberFormat);
                        else if (Currency == "FB")
                            katSayi = decimal.Parse(usdForexBuying, numberFormat);
                    }
                    else if (selectedProduct.Currency == "eu")
                    {
                        if (Currency == "BS")
                            katSayi = decimal.Parse(eurBanknoteSelling, numberFormat);
                        else if (Currency == "BB")
                            katSayi = decimal.Parse(eurBanknoteBuying, numberFormat);
                        else if (Currency == "FS")
                            katSayi = decimal.Parse(eurForexSelling, numberFormat);
                        else if (Currency == "FB")
                            katSayi = decimal.Parse(eurForexBuying, numberFormat);
                    }
                    else if (selectedProduct.Currency == "gbp")
                    {
                        if (Currency == "BS")
                            katSayi = decimal.Parse(gbpBanknoteSelling, numberFormat);
                        else if (Currency == "BB")
                            katSayi = decimal.Parse(gbpBanknoteBuying, numberFormat);
                        else if (Currency == "FS")
                            katSayi = decimal.Parse(gbpForexSelling, numberFormat);
                        else if (Currency == "FB")
                            katSayi = decimal.Parse(gbpForexBuying, numberFormat);
                    }


                    var productDiscount = decimal.Parse(productDiscountSplit[i].Replace(".", ","));
                    var productDeadline = productDeadlineSplit?[i];
                    var productAmount = int.TryParse(productAmountSplit[i], out var amount);
                    // productId'den gelen verilere göre yeni bir ProductOffer nesnesi oluşturun
                    var productOffer = new ProductOffer
                    {
                        ProductId = productId,
                        Discount = productDiscount,
                        ProductSellPrice = selectedProduct.SellPrice * (CurrencyType == "tl" ? katSayi : 1) * (1 - (productDiscount / 100)) ?? default,
                        Deadline = productDeadline ?? default,
                        Amount = amount,
                        Currency = CurrencyType == "tl" ? "tl" : selectedProduct.Currency
                    };

                    productOffers.Add(productOffer);
                }

                // ProductOffers koleksiyonunu Offer nesnesine ekleyin
                var offer = new Offer
                {
                    Pdf = pdfUrl,
                    Company = company,
                    CompanyId = companyId,
                    Customer = customer,
                    User = user,
                    DateOfOffer = DateTime.Now,
                    UserId = user.Id,
                    CustomerContact = contactPerson,
                    OfferNumber = OfferNumber,
                    TotalPrice = Total,
                    Discount = Discount,
                    ProductOffers = productOffers,
                    IsSend = true,
                    CurrenyType = CurrencyType,
                    Currency = Currency,
                };

                var result = await _offerService.CreateAsync(offer);
            }
            return View();
        }

        public async Task<IActionResult> Search(int customerId, string searchWord, string currency, string currencyType)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var productList = await _productService.GetSearchProduct(companyId, searchWord);
            var customer = await _customerService.GetByIdAsync(customerId);
            var userId = HttpContext.Session.GetString("UserId") ?? default;
            var user = await _userManager.FindByIdAsync(userId);

            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.NumberGroupSeparator = ",";
            decimal katSayi = 1;

            string usdBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingUSD"];
            string eurBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingEUR"];
            string gbpBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingGBP"];

            var usdBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingUSD"];
            var eurBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingEUR"];
            var gbpBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingGBP"];

            var usdForexSelling = _configuration["CurrenciesModel:ForexSellingUSD"];
            var eurForexSelling = _configuration["CurrenciesModel:ForexSellingEUR"];
            var gbpForexSelling = _configuration["CurrenciesModel:ForexSellingGBP"];

            var usdForexBuying = _configuration["CurrenciesModel:ForexBuyingUSD"];
            var eurForexBuying = _configuration["CurrenciesModel:ForexBuyingEUR"];
            var gbpForexBuying = _configuration["CurrenciesModel:ForexBuyingGBP"];

            var productViewModel = new List<ProductViewModel>();
            foreach (var product in productList)
            {
                var offers = await _offerService.GetCustomerOffersAsync(customerId);
                var offerDiscount = offers
                    .Where(offer => offer.ProductOffers.Any(po => po.ProductId == product.Id))
                    .SelectMany(offer => offer.ProductOffers)
                    .Where(po => po.ProductId == product.Id)
                    .ToList();

                if (product.Currency == "usd")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(usdBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(usdBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(usdForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(usdForexBuying, numberFormat);
                }
                else if (product.Currency == "eu")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(eurBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(eurBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(eurForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(eurForexBuying, numberFormat);
                }
                else if (product.Currency == "gbp")
                {
                    if (currency == "BS")
                        katSayi = decimal.Parse(gbpBanknoteSelling, numberFormat);
                    else if (currency == "BB")
                        katSayi = decimal.Parse(gbpBanknoteBuying, numberFormat);
                    else if (currency == "FS")
                        katSayi = decimal.Parse(gbpForexSelling, numberFormat);
                    else if (currency == "FB")
                        katSayi = decimal.Parse(gbpForexBuying, numberFormat);
                }

                productViewModel.Add(new ProductViewModel()
                {
                    Id = product.Id,
                    Code = product?.Code,
                    Name = product?.Name,
                    SellPrice = product?.SellPrice * (currencyType == "tl" ? katSayi : 1),
                    Detail = product?.Detail,
                    Stock = product.Stock,
                    Discount = customer?.Discount,
                    Images = product?.ProductImages.Select(p => p.Url).ToList(),
                    CompanyId = product.CompanyId,
                    CategoryName = product.Category?.Name,
                    ProductOffers = offerDiscount,
                    User = user,
                    Currency = currencyType == "tl" ? "tl" : product?.Currency,
                });

            }

            return View(productViewModel);
        }

        [HttpPost]
        public IActionResult DenemePdf()
        {

            var converter = new BasicConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        PagesCount=true,
                        HtmlContent = "<h1>Hello, PDF!</h1>",
                    }
                }
            };

            byte[] pdfBytes = converter.Convert(doc);

            var pdfFile = File(pdfBytes, "application/pdf", "example.pdf");


            return View();
        }

        [HttpPost]
        public IActionResult GeneratePdf(string htmlContent)
        {
            try
            {
                var globalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Sayfa [page] / [toPage]", Line = true },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Sayfa [page] / [toPage]" }
                };

                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                var pdfBytes = _pdfConverter.Convert(pdf);
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                // Hata işleme kodları burada yer alabilir.
                return Content("Hata oluştu: " + ex.Message);
            }
        }

        public async Task<IActionResult> SpecialNote(int customerId)
        {
            var customerSpecialNote = await _customerService.GetByIdAsync(customerId);
            return View(customerSpecialNote);
        }

        public async Task<IActionResult> SearchOffer(string searchWord)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var userId = HttpContext.Session.GetString("UserId") ?? default;
            var user = await _userManager.FindByIdAsync(userId);

            var offerList = await _offerService.GetSearchOfferAsync(companyId, searchWord);
            var offerModel = new OfferListViewModel()
            {
                Offers = offerList,
                User = user,
            };

            return View(offerModel);
        }

        public async Task<IActionResult> Approved(int offerId)
        {
            var offer = await _offerService.GetByIdAsync(offerId);
            offer.IsApproved = !offer.IsApproved;
            var result = await _offerService.UpdateAsync(offer);

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Note(int OfferId, string note)
        {
            var offer = await _offerService.GetByIdAsync(OfferId);
            if (offer != null)
            {
                offer.Note = note ?? default;
                var result = await _offerService.UpdateAsync(offer);
                return Json(result ? new { status = 200 } : new { status = 400 });
            }
            return Json(new { status = 400 });
        }

        [HttpPost]
        public async Task<IActionResult> Filter(int? customerId, string userId, DateTime dateStart, DateTime dateEnd)
        {
            userId = userId == "Gönderen Kişiyi Seçiniz" ? null : userId;
            var offerList = await _offerService.GetFilterOfferAsync(customerId, userId, dateStart, dateEnd);
            var user = await _userManager.FindByIdAsync(userId);

            var offerModel = new OfferListViewModel()
            {
                Offers = offerList,
                User = user,
            };

            return View(offerModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveOffer(IFormFile pdfFile, int CustomerId, int OfferNumber, int ContactPersonId, decimal Total, decimal Discount, List<string> CcEmail, List<string> ProductIds, string ProductDiscount, string MailBody, string Deadline, string Amounts, string CurrencyType, string Currency)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var userId = HttpContext.Session.GetString("UserId") ?? default;

            var user = await _userManager.FindByIdAsync(userId);
            var company = await _companyService.GetByIdAsync(companyId);

            var customer = await _customerService.GetByIdAsync(CustomerId);

            var contactPerson = await _contactPersonService.GetByIdAsync(ContactPersonId);

            var pdf = Jobs.UploadPdf(pdfFile, customer.Name, companyId, company.Name);

            var pdfSplit = pdf.Split("\\");
            var pdfUrl = pdfSplit[pdfSplit.Length - 1];

            var productIdSplit = ProductIds.FirstOrDefault()?.Split(",");
            var productDiscountSplit = ProductDiscount.Split(",");
            var productDeadlineSplit = Deadline?.Split(",");
            var productAmountSplit = Amounts?.Split(",");

            if (productIdSplit != null)
            {
                var productOffers = new List<ProductOffer>(); // ProductOffer sınıfınıza uygun bir koleksiyon türü kullanın

                decimal katSayi = 1;

                NumberFormatInfo numberFormat = new NumberFormatInfo();
                numberFormat.NumberDecimalSeparator = ".";

                string usdBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingUSD"];
                string eurBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingEUR"];
                string gbpBanknoteSelling = _configuration["CurrenciesModel:BanknoteSellingGBP"];

                var usdBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingUSD"];
                var eurBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingEUR"];
                var gbpBanknoteBuying = _configuration["CurrenciesModel:BanknoteBuyingGBP"];

                var usdForexSelling = _configuration["CurrenciesModel:ForexSellingUSD"];
                var eurForexSelling = _configuration["CurrenciesModel:ForexSellingEUR"];
                var gbpForexSelling = _configuration["CurrenciesModel:ForexSellingGBP"];

                var usdForexBuying = _configuration["CurrenciesModel:ForexBuyingUSD"];
                var eurForexBuying = _configuration["CurrenciesModel:ForexBuyingEUR"];
                var gbpForexBuying = _configuration["CurrenciesModel:ForexBuyingGBP"];

                for (int i = 0; i < productIdSplit.Length; i++)
                {
                    var productId = int.Parse(productIdSplit[i]);
                    var selectedProduct = await _productService.GetProductByIdAsync(productId);

                    if (selectedProduct.Currency == "usd")
                    {
                        if (Currency == "BS")
                            katSayi = decimal.Parse(usdBanknoteSelling, numberFormat);
                        else if (Currency == "BB")
                            katSayi = decimal.Parse(usdBanknoteBuying, numberFormat);
                        else if (Currency == "FS")
                            katSayi = decimal.Parse(usdForexSelling, numberFormat);
                        else if (Currency == "FB")
                            katSayi = decimal.Parse(usdForexBuying, numberFormat);
                    }
                    else if (selectedProduct.Currency == "eu")
                    {
                        if (Currency == "BS")
                            katSayi = decimal.Parse(eurBanknoteSelling, numberFormat);
                        else if (Currency == "BB")
                            katSayi = decimal.Parse(eurBanknoteBuying, numberFormat);
                        else if (Currency == "FS")
                            katSayi = decimal.Parse(eurForexSelling, numberFormat);
                        else if (Currency == "FB")
                            katSayi = decimal.Parse(eurForexBuying, numberFormat);
                    }
                    else if (selectedProduct.Currency == "gbp")
                    {
                        if (Currency == "BS")
                            katSayi = decimal.Parse(gbpBanknoteSelling, numberFormat);
                        else if (Currency == "BB")
                            katSayi = decimal.Parse(gbpBanknoteBuying, numberFormat);
                        else if (Currency == "FS")
                            katSayi = decimal.Parse(gbpForexSelling, numberFormat);
                        else if (Currency == "FB")
                            katSayi = decimal.Parse(gbpForexBuying, numberFormat);
                    }


                    var productDiscount = decimal.Parse(productDiscountSplit[i].Replace(".", ","));
                    var productDeadline = productDeadlineSplit?[i];
                    var productAmount = int.TryParse(productAmountSplit[i], out var amount);
                    // productId'den gelen verilere göre yeni bir ProductOffer nesnesi oluşturun
                    var productOffer = new ProductOffer
                    {
                        ProductId = productId,
                        Discount = productDiscount,
                        ProductSellPrice = selectedProduct.SellPrice * (CurrencyType == "tl" ? katSayi : 1) * (1 - (productDiscount / 100)) ?? default,
                        Deadline = productDeadline ?? default,
                        Amount = amount,
                        Currency = CurrencyType == "tl" ? "tl" : selectedProduct.Currency
                    };

                    productOffers.Add(productOffer);
                }

                // ProductOffers koleksiyonunu Offer nesnesine ekleyin
                var offer = new Offer
                {
                    Pdf = pdfUrl,
                    Company = company,
                    CompanyId = companyId,
                    Customer = customer,
                    User = user,
                    DateOfOffer = DateTime.Now,
                    UserId = user.Id,
                    CustomerContact = contactPerson,
                    OfferNumber = OfferNumber,
                    TotalPrice = Total,
                    Discount = Discount,
                    ProductOffers = productOffers,
                    IsSend = true,
                    CurrenyType = CurrencyType,
                    Currency = Currency,
                };

                var result = await _offerService.CreateAsync(offer);
                var status = result ? 200 : 400;
                return Json(new { status = status });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SendAgain(string pdf, int offerId)
        {
            ViewBag.Pdf = pdf.Replace("pdf", "png");
            ViewBag.OfferId = offerId;
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var customerList = await _customerService.GetCompanyByCustomersAsync(companyId);
            ViewBag.CompanyId = companyId;
            return View(customerList);
        }

        [HttpPost]
        public async Task<IActionResult> SendAgain(int customerId, int contactPersonId, List<string> CcEmail, string pdf, int offerId)
        {
            var pdfReplace = pdf.Replace("png", "pdf");
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var userId = HttpContext.Session.GetString("UserId") ?? default;
            var offer = await _offerService.GetByIdAsync(offerId);

            var user = await _userManager.FindByIdAsync(userId);
            var company = await _companyService.GetByIdAsync(companyId);

            var companySettings = await _companySettingsService.GetAllCompanySettingsAsync(companyId);

            var customer = await _customerService.GetByIdAsync(customerId);

            var contactPerson = await _contactPersonService.GetByIdAsync(contactPersonId);

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
            };

            var sunucu = companySettingsViewModel.EmailServer;
            var port = companySettingsViewModel.EmailServerPort;
            var mail = companySettingsViewModel.EmailUsername;
            var sifre = companySettingsViewModel.EmailPassword;
            var aliciEmail = companySettingsViewModel.RecipientEmail;

            var fromAddress = new MailAddress(mail, "Teklif");
            var toAddress = new MailAddress(aliciEmail);

            var contactPersonEmail = new MailAddress(contactPerson.Email);

            SmtpClient smtp = new SmtpClient();
            try
            {
                smtp.Host = sunucu;
                smtp.Port = Convert.ToInt32(port);
                smtp.EnableSsl = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(mail, sifre);
            }
            catch
            {
                smtp.Host = sunucu;
                smtp.Port = Convert.ToInt32(port);
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(mail, sifre);
            }

            var message = new MailMessage(fromAddress, toAddress);
            message.Body = String.Format(@"Merhaba, <br/><b>" + customer.Name + "</b> adlı firmadan teklif gönderildi");
            message.Subject = customer.Name;
            message.IsBodyHtml = true;

            var message2 = new MailMessage(fromAddress, contactPersonEmail);
            message2.Body = String.Format(@"Merhaba, <br/><b>" + company.Name + "</b> adlı firmadan teklif gönderildi");
            message2.Subject = company.Name;
            message2.IsBodyHtml = true;

            var ccEmailSplit = CcEmail.FirstOrDefault()?.Split(",");
            if (ccEmailSplit != null)
            {
                foreach (var cc in ccEmailSplit)
                {
                    message.CC.Add(cc);
                }
            }
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Content/pdfs/C" + companyId);
            var pdfPath = Path.Combine(folderPath, pdfReplace);

            // PDF dosyasını ekleyin
            var attachment = new Attachment(pdfPath);


            message.Attachments.Add(attachment);
            message2.Attachments.Add(attachment);

            message.IsBodyHtml = true;
            {
                smtp.Send(message);
            }
            message2.IsBodyHtml = true;
            {
                smtp.Send(message2);
            }
            if (offer != null)
            {
                offer.IsSend = true;
                await _offerService.UpdateAsync(offer);
            }

            return RedirectToAction("OfferList");
        }

        public async Task<IActionResult> OfferSoftDelete(int offerId)
        {
            var offer = await _offerService.GetByIdAsync(offerId);
            if (offer != null)
            {
                offer.IsDeleted = true;
                var result = await _offerService.UpdateAsync(offer);
                return Json(new { status = result ? 200 : 400 });
            }
            return Json(new { status = 300 });
        }

        public async Task<IActionResult> OfferRestore(int offerId)
        {
            var offer = await _offerService.GetByIdAsync(offerId);
            if (offer != null)
            {
                offer.IsDeleted = false;
                var result = await _offerService.UpdateAsync(offer);
                return Json(new { status = result ? 200 : 400 });
            }
            return Json(new { status = 300 });
        }

        public async Task<IActionResult> OfferHardDelete(int offerId)
        {
            var offer = await _offerService.GetByIdAsync(offerId);
            if (offer != null)
            {
                var result = await _offerService.GetOfferDeleteAsync(offer);
                return Json(new { status = result ? 200 : 400 });
            }
            return Json(new { status = 300 });
        }

        public async Task<IActionResult> OfferDeleteList()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var userId = HttpContext.Session.GetString("UserId") ?? default;

            var user = await _userManager.FindByIdAsync(userId);
            var offerList = await _offerService.GetCompanyOffersAsync(companyId);

            var offerModel = new OfferListViewModel()
            {
                Offers = offerList,
                User = user,
            };

            return View(offerModel);
        }



    }
}
