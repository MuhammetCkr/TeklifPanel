using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Data.Abstract;
using TeklifPanel.Entity;

namespace TeklifPanel.Data.Concrete.EfCore
{
    public class EfCoreOfferRepository : EfCoreGenericRepository<Offer>, IOfferRepository
    {
        public EfCoreOfferRepository(TeklifPanelContext _dbContext) : base(_dbContext) { }
        private TeklifPanelContext context
        {
            get { return _dbContext as TeklifPanelContext; }
        }

        public async Task<List<Offer>> GetCompanyOffersAsync(int companyId)
        {
            try
            {
                var offerList = await context.Offers
                    .Where(o => o.CompanyId == companyId)
                    .Include(o => o.Company)
                    .Include(o => o.CustomerContact)
                    .Include(o => o.Customer)
                    .Include(o => o.User)
                    .Include(o => o.ProductOffers).ThenInclude(o => o.Product).ThenInclude(o => o.Category)
                    .ToListAsync();
                return offerList;

            }
            catch (Exception e)
            {

                throw;
            }

        }

        public async Task<List<Offer>> GetCustomerOffersAsync(int customerId)
        {
            try
            {
                var offerList = await context.Offers
                    .Where(o => o.CustomerId == customerId)
                    .Include(o => o.Company)
                    .Include(o => o.CustomerContact)
                    .Include(o => o.Customer)
                    .Include(o => o.User)
                    .Include(o => o.ProductOffers).ThenInclude(x => x.Product)
                    .ToListAsync();
                return offerList;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<List<Offer>> GetFilterOfferAsync(int? customerId, string userId, DateTime? startDate, DateTime? endDate)
        {

            var filter = await context.Offers
                .Where(f =>
                 (customerId == null || f.CustomerId == customerId)
                 && (string.IsNullOrWhiteSpace(userId) || f.UserId == userId)
                 && (startDate == new DateTime() || f.DateOfOffer >= startDate)
                 && (endDate == new DateTime() || f.DateOfOffer <= endDate))
                .Include(x => x.Customer)
                .Include(x => x.Company)
                .Include(x => x.User)
                .ToListAsync();
            return filter;
        }

        public async Task<Offer> GetOfferAsync(int offerId)
        {
            var offer = await context.Offers
                .Where(o => o.Id == offerId)
                .Include(o => o.Company)
                .Include(o => o.CustomerContact)
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.ProductOffers).ThenInclude(o => o.Product).ThenInclude(o => o.Category)
                .SingleOrDefaultAsync();
            return offer;
        }

        public async Task<bool> GetOfferDeleteAsync(Offer offer)
        {
            context.Offers.Remove(offer);
            var result = await context.SaveChangesAsync();
            return result > 0 ? true : false;
        }

        public async Task<List<Offer>> GetSearchOfferAsync(int? companyId, string searchWord)
        {
            var searchOffer = await context.Offers.Where(x => x.CompanyId == companyId && x.OfferNumber.ToString().Contains(searchWord))
                .Include(x => x.Customer)
                .Include(x => x.Company)
                .Include(x => x.User)
                .Take(10)
                .ToListAsync() ?? new List<Offer>();
            return searchOffer;
        }
    }
}
