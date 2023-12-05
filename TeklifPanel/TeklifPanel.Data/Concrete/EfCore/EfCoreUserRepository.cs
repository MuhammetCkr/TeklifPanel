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
    public class EfCoreUserRepository : EfCoreGenericRepository<User>, IUserRepository
    {
        public EfCoreUserRepository(TeklifPanelContext _dbContext) : base(_dbContext) { }
        private TeklifPanelContext context
        {
            get { return _dbContext as TeklifPanelContext; }
        }

        public async Task<User> GetUserAsync(string userId)
        {
            var user = await context.Users.Where(u => u.Id == userId).Include(u => u.Offers).FirstOrDefaultAsync();
            return user;
        }

        public Task GetUserDeleteAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetUsersByCompany(int companyId)
        {
            var userList = await context.Users.Where(u => u.CompanyId == companyId).Include(u => u.Offers).ToListAsync();
            return userList;
        }

        //public async Task GetUserDeleteAsync(string userId)
        //{
        //    var user =  context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        //    await context.Remove(user);
        //}
    }
}
