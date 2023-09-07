﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Data.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetCompanyProductsAsync(int companyId);
        Task<List<Product>> GetProductsByCategoryAsync(int companyId, int categoryId);
        Task<Product> GetProductByIdAsync(int productId);
        Task<bool> DeleteProductAsync(int productId);

    }
}
