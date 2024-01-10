using System.Collections.Generic;
using Test.Application.Handlers;
using Test.Application.Queries;
using Test.Core.Entities;
using Test.DataAccess.Repositories;

namespace Test.Application.QueryHandlers
{
    public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, List<Product>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IDiscountCommandHandler _discountCommandHandler;

        public GetAllProductsQueryHandler(IProductRepository productRepository, IDiscountCommandHandler discountCommandHandler)
        {
            _productRepository = productRepository;
            _discountCommandHandler = discountCommandHandler;
        }


        public List<Product> Handle(GetAllProductsQuery query)
        {
            var products = _productRepository.GetAllProducts();

            foreach (var product in products)
            {
                var discount = _discountCommandHandler.GetDiscount(product.ProductId);
                ApplyDiscountToProduct(product, discount);
            }

            return products;
        }
        private void ApplyDiscountToProduct(Product product, decimal discount)
        {
            product.Discount = discount;
            product.FinalPrice = CalculateFinalPrice(product.Price, discount);
        }

        private decimal CalculateFinalPrice(decimal price, decimal discount)
        {
            return price * (100 - discount) / 100;
        }
    }
}
