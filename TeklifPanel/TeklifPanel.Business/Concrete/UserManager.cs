using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Data.Abstract;
using TeklifPanel.Entity;

namespace TeklifPanel.Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Create(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetById(int id)
        {
            throw new NotImplementedException();

        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);

        }

        public Task<ICollection<User>> GetManyAsync(Expression<Func<User, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserAsync(string userId)
        {
            return await _userRepository.GetUserAsync(userId);
        }

        public async Task GetUserDeleteAsync(string userId)
        {
            await _userRepository.GetUserDeleteAsync(userId);
        }

        public async Task<List<User>> GetUsersByCompany(int companyId)
        {
            return await _userRepository.GetUsersByCompany(companyId);
        }

        public void Update(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }

        Task<ICollection<User>> IService<User>.GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
