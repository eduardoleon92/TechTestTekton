using System;
using System.Collections.Generic;
using Test.Application.Handlers;
using Test.Application.Queries;
using Test.Core.Entities;
using Test.DataAccess.Repositories;
using static Test.Application.CommandHandlers.GetDiscountCommandHandler;

namespace Test.Application.QueryHandlers
{
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, Product>
    {
        private readonly IProductRepository _productRepository;
        private readonly IDiscountCommandHandler _discountCommandHandler;

        public GetProductByIdQueryHandler(IProductRepository productRepository, IDiscountCommandHandler discountCommandHandler)
        {
            _productRepository = productRepository;
            _discountCommandHandler = discountCommandHandler;
        }

        public Product Handle(GetProductByIdQuery query)
        {
            var product = _productRepository.GetById(query.ProductId);
            var discount = _discountCommandHandler.GetDiscount(query.ProductId);
            ApplyDiscountToProduct(product, discount);

            return product;
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
