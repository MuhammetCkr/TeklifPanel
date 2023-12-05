using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Business.Abstract
{
    public interface IProductOffersService : IService<ProductOffer>
    {
        Task<List<ProductOffer>> GetByIdProductOffersAsync(int offerId);
    }
}
