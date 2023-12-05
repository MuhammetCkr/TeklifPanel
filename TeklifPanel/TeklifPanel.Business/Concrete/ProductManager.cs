using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Business.Abstract;
using TeklifPanel.Data.Abstract;
using TeklifPanel.Entity;

namespace TeklifPanel.Business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public void Create(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateAsync(Product entity)
        {
            return await _productRepository.CreateAsync(entity);
        }

        public void Delete(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(Product entity)
        {
            return await _productRepository.DeleteAsync(entity);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            return await _productRepository.DeleteProductAsync(productId);
        }

        public Task<ICollection<Product>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Product GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetCompanyProductsAsync(int companyId)
        {
            return await _productRepository.GetCompanyProductsAsync(companyId);
        }

        public Task<ICollection<Product>> GetManyAsync(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            try
            {

                return await _productRepository.GetProductByIdAsync(productId);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<Product> GetProductByUrlAsync(string url)
        {
            return await _productRepository.GetProductByUrlAsync(url);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int companyId, int categoryId)
        {
            return await _productRepository.GetProductsByCategoryAsync(companyId, categoryId);
        }

        public async Task<List<Product>> GetSearchProduct(int? companyId, string searchWord)
        {
            return await _productRepository.GetSearchProduct(companyId, searchWord);
        }

        public void Update(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(Product entity)
        {
            return await _productRepository.UpdateAsync(entity);
        }

        Task<Product> IService<Product>.GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
