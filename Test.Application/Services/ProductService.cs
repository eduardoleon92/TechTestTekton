using LazyCache;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Application.Commands;
using Test.Core.Entities;
using Test.DataAccess.Repositories;

namespace Test.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IAppCache _cache;

        public ProductService(IProductRepository productRepository, IAppCache cache)
        {
            _productRepository = productRepository;
            _cache = cache;
        }

        public Dictionary<int, string> GetProductStatus()
        {
            return _productRepository.GetProductStatus();
        }

        public void AddProduct(CreateProductCommand command)
        {
            _productRepository.AddProduct(command.Name, command.Stock, command.Description, command.Price);
        }

        public List<Product> GetAllProducts()
        {
            var allProducts = _productRepository.GetAllProducts();

            foreach (var product in allProducts)
            {
                product.Status = _cache.GetOrAdd($"StatusName_{product.Status}", () => GetStatusName(product.Status));
                product.Discount = GetDiscountFromExternalService(product.ProductId);
                product.FinalPrice = CalculateFinalPrice(product.Price, product.Discount);
            }

            return allProducts;
        }

        public Product GetById(int productId)
        {
            var product = _productRepository.GetById(productId);

            if (product != null)
            {
                Console.WriteLine("Intentando obtener StatusName desde LazyCache...");
                product.Status = _cache.GetOrAdd($"StatusName_{product.Status}", () => GetStatusName(product.Status));
                product.Discount = GetDiscountFromExternalService(productId);
                product.FinalPrice = CalculateFinalPrice(product.Price, product.Discount);
            }

            return product;
        }

        private int GetStatusName(int status)
        {
            return status == 1 ? 1 : 0; 
        }


        private decimal GetDiscountFromExternalService(int productId)
        {
            var mockApiUrl = "https://659763c5668d248edf22cfe6.mockapi.io/api/v1/discount";
            try
            {
                var httpClient = new HttpClient();
                var response = httpClient.GetStringAsync(mockApiUrl).Result;
                var discounts = JsonConvert.DeserializeObject<List<DiscountResponse>>(response);
                var discountObject = discounts.FirstOrDefault(d => d.Id == productId.ToString());

                if (discountObject != null)
                {
                    return discountObject.Discount;
                }
                    
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener descuento del servicio externo: {ex.Message}");
            }
            return 0;
        }

        private class DiscountResponse
        {
            public string Id { get; set; }
            public decimal Discount { get; set; }
        }

        private decimal CalculateFinalPrice(decimal price, decimal discount)
        {
            return price * (100 - discount) / 100;
        }

        public void UpdateProduct(UpdateProductCommand command)
        {
            _productRepository.UpdateProduct(command.ProductId, command.Name, command.Stock, command.Description, command.Price);
        }


    }
}
