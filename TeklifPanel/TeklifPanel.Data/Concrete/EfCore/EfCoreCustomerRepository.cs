using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Data.Abstract;
using TeklifPanel.Entity;

namespace TeklifPanel.Data.Concrete.EfCore
{
    public class EfCoreCustomerRepository : EfCoreGenericRepository<Customer>, ICustomerRepository
    {
        public EfCoreCustomerRepository(TeklifPanelContext _dbContext) : base(_dbContext) { }
        private TeklifPanelContext context
        {
            get { return _dbContext as TeklifPanelContext; }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customer = await context.Customers
                .Include(c => c.CustomerContacts)
                .SingleOrDefaultAsync(c => c.Id == customerId);
            context.CustomerContacts.RemoveRange(customer.CustomerContacts);
            context.Remove(customer);
            var result = await context.SaveChangesAsync();
            return result > 0 ? true : false;
        }

        public async Task<ICollection<Customer>> GetCompanyByCustomersAsync(int companyId)
        {
            var customerList = await context.Customers
                .Where(c => c.CompanyId ==  companyId)
                .Include(c => c.CustomerContacts)
                .ToListAsync();
            return customerList;
        }

        public async Task<Customer> GetCustomerAsync(int customerId)
        {
            var customer = await context.Customers
                .Where(c => c.Id == customerId)
                .Include(c => c.CustomerContacts)
                .Include(c => c.Company)
                .FirstOrDefaultAsync();
            return customer;
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            var updateCustomer = await context.Customers
                .Include(c => c.CustomerContacts)
                .SingleOrDefaultAsync(c => c.Id == customer.Id);
            var result = context.SaveChanges();
            return result > 0 ? true : false;
        }
    }
}
