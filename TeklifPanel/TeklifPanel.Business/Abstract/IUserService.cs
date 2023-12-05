using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Business.Abstract
{
    public interface IUserService : IService<User>
    {
        Task<User> GetUserAsync(string userId);
        Task GetUserDeleteAsync(string userId);
        Task<List<User>> GetUsersByCompany (int companyId);
    }
}
