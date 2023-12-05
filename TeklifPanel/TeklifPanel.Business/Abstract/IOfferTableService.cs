using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Business.Abstract
{
    public interface IOfferTableService : IService<OfferTable>
    {
        Task<bool> CreateCompanyOfferTableAsync(int companyId);

    }
}
