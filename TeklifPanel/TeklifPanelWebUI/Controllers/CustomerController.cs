using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Entity;
using TeklifPanelWebUI.ViewModels;

namespace TeklifPanelWebUI.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IContactPersonService _contactPersonService;

        public CustomerController(ICustomerService customerService, IContactPersonService contactPersonService)
        {
            _customerService = customerService;
            _contactPersonService = contactPersonService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CustomerList()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var customerList = await _customerService.GetCompanyByCustomersAsync(companyId);
            return View(customerList);
        }
        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerViewModel customerViewModel)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var customer = new Customer()
            {
                CompanyId = companyId,
                Discount = customerViewModel.Discount ?? default,
                Name = customerViewModel?.Name,
                Note = customerViewModel?.Note,
                Phone = customerViewModel?.Phone,
                TaxNumber = customerViewModel.TaxNumber,
                TaxOffice = customerViewModel?.TaxOffice,
                OpenAddress = customerViewModel?.OpenAddress,
                Email = customerViewModel?.Email,
                SpecialNote = customerViewModel?.SpecialNote,
            };

            var isAddCustomer = await _customerService.CreateAsync(customer);
            if (isAddCustomer)
            {
                var getCustomer = await _customerService.GetByIdAsync(customer.Id);

                for (int i = 0; i < customerViewModel.ContactName.Count(); i++)
                {
                    customer.CustomerContacts.Add(new CustomerContact()
                    {
                        CustomerId = getCustomer.Id,
                        Department = customerViewModel?.Department[i],
                        Email = customerViewModel?.ContactEmail[i],
                        Name = customerViewModel?.ContactName[i],
                        Phone = customerViewModel?.ContactPhone[i],
                        CompanyId = companyId
                    });
                }

                var isAddContactPerson = await _customerService.UpdateAsync(customer);

                if (isAddContactPerson)
                {
                    TempData["Message"] = "Müşteri Eklendi";
                    return RedirectToAction("CustomerList");
                }
                TempData["Error"] = "Müşteri eklendi fakat Kontakt kişileri eklenemedi!";
                return RedirectToAction("CustomerList");
            }
            TempData["Error"] = "Müşteri eklenemedi!";
            return RedirectToAction("CustomerList");
        }
        public async Task<IActionResult> UpdateCustomer(int id)
        {
            var customer = await _customerService.GetCustomerAsync(id);
            if (customer == null)
            {
                TempData["Error"] = "Müşteri bulunamadı!";
                return RedirectToAction("CustomerList");
            }
            var customerViewModel = new CustomerViewModel()
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                OpenAddress = customer.OpenAddress,
                Discount = customer.Discount,
                TaxNumber = customer.TaxNumber,
                TaxOffice = customer.TaxOffice,
                Email = customer.Email,
                Note = customer.Note,
                CompanyId = customer.CompanyId,
                SpecialNote = customer.SpecialNote,
            };
            customerViewModel.CustomerContacts = customer.CustomerContacts;
            return View(customerViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(CustomerViewModel customerViewModel)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId") ?? default;
            var getCustomer = await _customerService.GetCustomerAsync(customerViewModel.Id);

            if (getCustomer == null)
            {
                TempData["Error"] = "Müşteri bulunamadı!";
                return RedirectToAction("CustomerList");
            }
            getCustomer.TaxNumber = customerViewModel.TaxNumber;
            getCustomer.TaxOffice = customerViewModel?.TaxOffice;
            getCustomer.Name = customerViewModel?.Name;
            getCustomer.Discount = customerViewModel?.Discount;
            getCustomer.Email = customerViewModel?.Email;
            getCustomer.Phone = customerViewModel?.Phone;
            getCustomer.OpenAddress = customerViewModel?.OpenAddress;
            getCustomer.Note = customerViewModel?.Note;
            getCustomer.SpecialNote = customerViewModel?.SpecialNote;

            if (customerViewModel.ContactName != null)
            {
                if (customerViewModel.ContactName.Count() == getCustomer.CustomerContacts.Count())
                {
                    for (int i = 0; i < customerViewModel.ContactName.Count(); i++)
                    {
                        getCustomer.CustomerContacts[i].Id = customerViewModel.ContactId[i];
                        getCustomer.CustomerContacts[i].CustomerId = getCustomer.Id;
                        getCustomer.CustomerContacts[i].Department = customerViewModel?.Department[i];
                        getCustomer.CustomerContacts[i].Email = customerViewModel?.ContactEmail[i];
                        getCustomer.CustomerContacts[i].Name = customerViewModel?.ContactName[i];
                        getCustomer.CustomerContacts[i].Phone = customerViewModel?.ContactPhone[i];
                    }
                }
                else
                {
                    for (int i = getCustomer.CustomerContacts.Count(); i < customerViewModel.ContactName.Count(); i++)
                    {
                        getCustomer.CustomerContacts.Add(new CustomerContact()
                        {
                            CustomerId = getCustomer.Id,
                            Department = customerViewModel?.Department[i],
                            Email = customerViewModel?.ContactEmail[i],
                            Name = customerViewModel?.ContactName[i],
                            Phone = customerViewModel?.ContactPhone[i],
                            CompanyId = companyId
                        });
                    }
                }
            }
            var isAddContactPerson = await _customerService.UpdateAsync(getCustomer);

            if (isAddContactPerson)
            {
                TempData["Message"] = "Müşteri Güncellendi";
                return RedirectToAction("CustomerList");
            }
            TempData["Message"] = "Müşteri Güncellenemedi";
            return RedirectToAction("CustomerList");
        }

        public async Task<IActionResult> DeleteCustomer(int id)
         {
            var isCustomerDelete = await _customerService.DeleteCustomerAsync(id);
            if (isCustomerDelete)
            {
                return Json(new { status = 200 });
            }
            return Json(new { status = 400 });

        }

        public async Task<IActionResult> ContactPersonDelete(int id)
        {
            var contactPerson = await _contactPersonService.GetByIdAsync(id);
            var isDelete = await _contactPersonService.DeleteAsync(contactPerson);
            if (isDelete)
            {
                return Json(new { status = 200 });

            }
            return Json(new { status = 400 });
        }

        public async Task<IActionResult> CustomerDetail(int id)
        {
            var customer = await _customerService.GetCustomerAsync(id);
            
            if (customer == null)
            {
                TempData["Error"] = "Müşteri bulunamadı!";
                return RedirectToAction("CustomerList");
            }
            var customerViewModel = new CustomerViewModel()
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                OpenAddress = customer.OpenAddress,
                Discount = customer.Discount,
                TaxNumber = customer.TaxNumber,
                TaxOffice = customer.TaxOffice,
                Email = customer.Email,
                Note = customer.Note,
                CompanyId = customer.CompanyId,
                SpecialNote = customer.SpecialNote,
                Company = customer.Company,
                CustomerContacts = customer.CustomerContacts,
            };

            return View(customerViewModel);
        }
    }
}
