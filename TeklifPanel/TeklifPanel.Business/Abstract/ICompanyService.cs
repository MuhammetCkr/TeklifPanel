﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Business.Abstract
{
    public interface ICompanyService : IService<Company>
    {
        Task<Company> GetLastCompany();
        Task<Company> GetCompanyByIdAsync(int id);
    }
}
