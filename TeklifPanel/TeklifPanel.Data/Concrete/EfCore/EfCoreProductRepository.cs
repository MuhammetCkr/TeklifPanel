using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Data.Abstract;
using TeklifPanel.Entity;

namespace TeklifPanel.Data.Concrete.EfCore
{
    public class EfCoreProductRepository : EfCoreGenericRepository<Product>, IProductRepository
    {
        public EfCoreProductRepository(TeklifPanelContext _dbContext) : base(_dbContext) { }
        private TeklifPanelContext context
        {
            get { return _dbContext as TeklifPanelContext; }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await context.Products
                .Include(p => p.ProductImages)
                .SingleOrDefaultAsync(p => p.Id == productId);

            context.ProductImages.RemoveRange(product.ProductImages);

            context.Remove(product);
            var result = await context.SaveChangesAsync();
            return result > 0 ? true : false;
        }

        public async Task<List<Product>> GetCompanyProductsAsync(int companyId)
        {
            var productList = await context.Products
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.ProductImages)
                .ToListAsync();
            return productList;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await context.Products
                .Where(p => p.Id == productId)
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .FirstOrDefaultAsync();
            return product;
        }

        public async Task<Product> GetProductByUrlAsync(string url)
        {
            var product =  await context.Products
                .Where(p => p.Url == url)
                .Include(p => p.ProductImages)
                .SingleOrDefaultAsync();
            return product;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int companyId, int categoryId)
        {
            var productList = await context.Products
                .Where(p => p.CompanyId == companyId && p.CategoryId == categoryId)
                .Include(p => p.ProductImages)
                .ToListAsync();
            return productList;
        }

        public async Task<List<Product>> GetSearchProduct(int? companyId, string searchWord)
        {
            if (companyId != null)
            {
                var productList = await context.Products
                    .Where(p => p.CompanyId == companyId && p.IsActive == true && (p.Name.Contains(searchWord) || p.Code.Contains(searchWord)))
                    .Include(p => p.ProductImages)
                    .Include(p => p.Category)
                    .Take(10)
                    .ToListAsync();
                return productList;

            }
            else
            {
                var productList = await context.Products
                    .Where(p => p.IsActive == true && p.Name.Contains(searchWord))
                    .Include(p => p.ProductImages)
                    .Include(p => p.Category)
                    .Take(10)
                    .ToListAsync();
                return productList;
            }
        }
    }
}
