using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Data.Abstract
{
    public interface IOfferRepository : IRepository<Offer>
    {
        Task<List<Offer>> GetCompanyOffersAsync(int companyId);
        Task<List<Offer>> GetCustomerOffersAsync(int customerId);
        Task<List<Offer>> GetSearchOfferAsync(int? companyId, string searchWord);
        Task<List<Offer>> GetFilterOfferAsync(int? customerId, string? userId, DateTime? startDate, DateTime? endDate);
        Task<bool> GetOfferDeleteAsync(Offer offer);
        Task<Offer> GetOfferAsync(int offerId);

    }
}
