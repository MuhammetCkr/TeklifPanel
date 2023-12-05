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
    public class OfferTableManager : IOfferTableService
    {
        private readonly IOfferTableRepository _offerTableRepository;

        public OfferTableManager(IOfferTableRepository offerTableRepository)
        {
            _offerTableRepository = offerTableRepository;
        }

        public void Create(OfferTable entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(OfferTable entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateCompanyOfferTableAsync(int companyId)
        {
            return await _offerTableRepository.CreateCompanyOfferTableAsync(companyId);
        }

        public void Delete(OfferTable entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(OfferTable entity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<OfferTable>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<OfferTable> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OfferTable> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<OfferTable>> GetManyAsync(Expression<Func<OfferTable, bool>> expression)
        {
            return await _offerTableRepository.GetManyAsync(expression);
        }

        public void Update(OfferTable entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(OfferTable entity)
        {
            return await _offerTableRepository.UpdateAsync(entity);
        }
    }
}
