using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Business.Abstract
{
    public interface IProductService : IService<Product>
    {
        Task<List<Product>> GetCompanyProductsAsync(int companyId);
        Task<List<Product>> GetProductsByCategoryAsync(int companyId, int categoryId);
        Task<Product> GetProductByIdAsync(int productId);
        Task<bool> DeleteProductAsync(int productId);
        Task<List<Product>> GetSearchProduct(int? companyId,string searchWord);
        Task<Product> GetProductByUrlAsync(string url);
    }
}
