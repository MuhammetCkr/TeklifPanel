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
    public class EfCoreProductOffersRepository : EfCoreGenericRepository<ProductOffer>, IProductOffersRepository
    {
        public EfCoreProductOffersRepository(TeklifPanelContext _dbContext) : base(_dbContext) { }
        private TeklifPanelContext context
        {
            get { return _dbContext as TeklifPanelContext; }
        }

        public async Task<List<ProductOffer>> GetByIdProductOffersAsync(int offerId)
        {
            var productOffers = await context.ProductOffers.Where(x => x.OfferId == offerId)
                .Include(o => o.Offer)
                .Include(o => o.Product)
                .ToListAsync();
            return productOffers;
        }
    }
}
