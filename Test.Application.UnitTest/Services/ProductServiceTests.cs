using LazyCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Application.Services;
using Test.Core.Entities;
using Test.DataAccess.Repositories;
using Moq;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Test.Application.UnitTest.Services
{
    [TestClass]
    public class ProductServiceTests
    {

        [TestMethod]
        public void GetAllProducts_ShouldReturnListOfProducts()
        {
            var cacheMock = new Mock<IAppCache>();
            var productRepositoryMock = new Mock<ProductRepository>(cacheMock.Object) { CallBase = true };
            var productService = new ProductService(productRepositoryMock.Object, cacheMock.Object);

            var result = productService.GetAllProducts();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Product>));
        }



        [TestMethod] 
        public void AddProduct_ShouldAddProductCorrectly()
        {
            var cacheMock = new Mock<IAppCache>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var productService = new ProductService(productRepositoryMock.Object, cacheMock.Object);

            productService.AddProduct("TestProduct", 10, "TestDescription", 100.0m);

            productRepositoryMock.Verify(repo => repo.AddProduct(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Once);
        }

        [TestMethod]
        public void UpdateProduct_ShouldUpdateProductCorrectly()
        {
            var cacheMock = new Mock<IAppCache>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var productService = new ProductService(productRepositoryMock.Object, cacheMock.Object);

            var existingProduct = new Product
            {
                ProductId = 1,
                Name = "ExistingProduct",
                Stock = 10,
                Description = "ExistingDescription",
                Price = 100.0m,
                Status = 1
            };

            productRepositoryMock.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(existingProduct);

            productService.UpdateProduct(1, "UpdatedProduct", 20, "UpdatedDescription", 150.0m);

            productRepositoryMock.Verify(repo => repo.UpdateProduct(1, "UpdatedProduct", 20, "UpdatedDescription", 150.0m), Times.Once);
        }

    }
}
