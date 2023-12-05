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
    public class ProductOffersManager : IProductOffersService
    {
        private readonly IProductOffersRepository _productOffersRepository;

        public ProductOffersManager(IProductOffersRepository productOffersRepository)
        {
            _productOffersRepository = productOffersRepository;
        }

        public void Create(ProductOffer entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(ProductOffer entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(ProductOffer entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ProductOffer entity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ProductOffer>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ProductOffer> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductOffer> GetByIdAsync(int id)
        {
            return await _productOffersRepository.GetByIdAsync(id);
        }

        public async Task<List<ProductOffer>> GetByIdProductOffersAsync(int offerId)
        {
            return await _productOffersRepository.GetByIdProductOffersAsync(offerId);
        }

        public async Task<ICollection<ProductOffer>> GetManyAsync(Expression<Func<ProductOffer, bool>> expression)
        {
            return await _productOffersRepository.GetManyAsync(expression);
        }

        public void Update(ProductOffer entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ProductOffer entity)
        {
            throw new NotImplementedException();
        }
    }
}
