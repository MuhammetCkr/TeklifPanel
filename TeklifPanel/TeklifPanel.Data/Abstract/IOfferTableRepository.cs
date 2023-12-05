using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Data.Abstract
{
    public interface IOfferTableRepository : IRepository<OfferTable>
    {
        Task<bool> CreateCompanyOfferTableAsync(int companyId);

    }
}
