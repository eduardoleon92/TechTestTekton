using LazyCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Core.Entities;

namespace Test.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IAppCache _cache;
        private static List<Product> _products = new List<Product>();
        public ProductRepository(IAppCache cache)
        {
            _cache = cache;
        }

        public Dictionary<int, string> GetProductStatus()
        {
            var cacheKey = "ProductStatus";
            var cachedData = _cache.GetOrAdd(cacheKey, () => LoadProductStatus(), TimeSpan.FromMinutes(5));

            return cachedData;
        }

        private Dictionary<int, string> LoadProductStatus()
        {
            Console.WriteLine("Cargando datos de estados de productos...");

            var productStatus = new Dictionary<int, string>
            {
                { 1, "Active" },
                { 0, "Inactive" }
            };

            return productStatus;
        }

        public void AddProduct(string name, int stock, string description, decimal price)
        {
            var defaultStatus = GetDefaultStatusFromCache();

            var product = new Product
            {
                ProductId = _products.Count + 1,
                Name = name,
                Stock = stock,
                Description = description,
                Price = price,
                Status = defaultStatus
            };

            SetStatusNameInCache(defaultStatus);

            _products.Add(product);
        }

        private void SetStatusNameInCache(int status)
        {
            var statusCacheKey = $"Status_{status}";
            var statusName = _cache.GetOrAdd(statusCacheKey, () => LoadProductStatus()[status], TimeSpan.FromMinutes(10));
            _cache.Add(statusCacheKey, statusName, TimeSpan.FromMinutes(10));
        }


        private int GetDefaultStatusFromCache()
        {
            var cacheKey = "ProductStatus";
            var cachedValue = _cache.Get<Dictionary<int, string>>(cacheKey);

            if (cachedValue != null && cachedValue.TryGetValue(1, out var statusName))
            {
                Console.WriteLine($"Valor de caché obtenido correctamente: {statusName}");
                return 1;
            }
            else
            {
                Console.WriteLine("Valor de caché no encontrado. Usando valor predeterminado.");
                return 0;
            }
        }


        public List<Product> GetAllProducts()
        {
            return _products.ToList();
        }

        public virtual Product GetById(int productId)
        {
            return _products.FirstOrDefault(p => p.ProductId == productId);
        }

        public void UpdateProduct(int productId, string name, int stock, string description, decimal price)
        {
            var existingProduct = _products.FirstOrDefault(p => p.ProductId == productId);
            var defaultStatus = GetDefaultStatusFromCache();

            if (existingProduct != null)
            {
                existingProduct.Name = name;
                existingProduct.Stock = stock;
                existingProduct.Description = description;
                existingProduct.Price = price;
                existingProduct.Status = defaultStatus;
            }
        }

    }
}
