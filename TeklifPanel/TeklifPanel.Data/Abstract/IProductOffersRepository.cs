using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Data.Abstract
{
    public interface IProductOffersRepository : IRepository<ProductOffer>
    {
        Task<List<ProductOffer>> GetByIdProductOffersAsync(int offerId);

    }
}
