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
    public class OfferManager : IOfferService
    {
        private readonly IOfferRepository _offerRepository;

        public OfferManager(IOfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public void Create(Offer entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateAsync(Offer entity)
        {
            return await _offerRepository.CreateAsync(entity);
        }

        public void Delete(Offer entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Offer entity)
        {
            throw new NotImplementedException();
        }

        public Offer GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Offer> GetByIdAsync(int id)
        {
            return await _offerRepository.GetByIdAsync(id);
        }

        public async Task<List<Offer>> GetCompanyOffersAsync(int companyId)
        {
            return await _offerRepository.GetCompanyOffersAsync(companyId);
        }

        public async Task<List<Offer>> GetCustomerOffersAsync(int customerId)
        {
            return await _offerRepository.GetCustomerOffersAsync(customerId);
        }

        public async Task<List<Offer>> GetFilterOfferAsync(int? customerId, string userId, DateTime? startDate, DateTime? endDate)
        {
            return await _offerRepository.GetFilterOfferAsync(customerId, userId, startDate, endDate);
        }

        public Task<ICollection<Offer>> GetManyAsync(Expression<Func<Offer, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<Offer> GetOfferAsync(int offerId)
        {
            return await _offerRepository.GetOfferAsync(offerId);
        }

        public async Task<bool> GetOfferDeleteAsync(Offer offer)
        {
            return await _offerRepository.GetOfferDeleteAsync(offer);
        }

        public async Task<List<Offer>> GetSearchOfferAsync(int? companyId, string searchWord)
        {
            return await _offerRepository.GetSearchOfferAsync(companyId, searchWord);
        }

        public void Update(Offer entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(Offer entity)
        {
            return await _offerRepository.UpdateAsync(entity);
        }

        Task<ICollection<Offer>> IService<Offer>.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<Offer> IService<Offer>.GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
