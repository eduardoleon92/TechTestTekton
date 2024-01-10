using LazyCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Application.Services;
using Test.Core.Entities;
using Test.DataAccess.Repositories;
using Moq;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using Test.Application.Handlers;
using Test.Application.Queries;
using Test.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Test.Application.Commands;

namespace Test.Application.UnitTest.Services
{
    [TestClass]
    public class ProductServiceTests
    {

        [TestMethod]
        public void GetAllProducts_ReturnsOkResult()
        {
            // Arrange
            var mockHandler = new Mock<IQueryHandler<GetAllProductsQuery, List<Product>>>();
            mockHandler.Setup(handler => handler.Handle(It.IsAny<GetAllProductsQuery>()))
                       .Returns(new List<Product> { new Product { ProductId = 1, Name = "Test Product" } });

            var controller = new ProductController(
                createProductCommandHandler: Mock.Of<ICommandHandler<CreateProductCommand>>(),  // Puedes ajustar esto según tus necesidades
                updateProductCommandHandler: Mock.Of<ICommandHandler<UpdateProductCommand>>(),  // Puedes ajustar esto según tus necesidades
                getAllProductsQueryHandler: mockHandler.Object,
                getProductByIdQueryHandler: Mock.Of<IQueryHandler<GetProductByIdQuery, Product>>(),  // Puedes ajustar esto según tus necesidades
                getProductStatusQueryHandler: Mock.Of<IQueryHandler<GetProductStatusQuery, Dictionary<int, string>>>()  // Puedes ajustar esto según tus necesidades
            );

            // Act
            var result = controller.GetAllProducts();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(List<Product>));
            var products = (List<Product>)okResult.Value;
            Assert.AreEqual(1, products.Count);
        }

        [TestMethod]
        public void GetById_ReturnsOkResult_WhenProductExists()
        {
            // Arrange
            var mockHandler = new Mock<IQueryHandler<GetProductByIdQuery, Product>>();
            mockHandler.Setup(handler => handler.Handle(It.IsAny<GetProductByIdQuery>()))
                       .Returns(new Product { ProductId = 1, Name = "Test Product" });

            var controller = new ProductController(
                createProductCommandHandler: Mock.Of<ICommandHandler<CreateProductCommand>>(),
                updateProductCommandHandler: Mock.Of<ICommandHandler<UpdateProductCommand>>(),
                getAllProductsQueryHandler: Mock.Of<IQueryHandler<GetAllProductsQuery, List<Product>>>(),
                getProductByIdQueryHandler: mockHandler.Object,
                getProductStatusQueryHandler: Mock.Of<IQueryHandler<GetProductStatusQuery, Dictionary<int, string>>>()
            );

            // Act
            var result = controller.GetById(productId: 1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(Product));
            var product = (Product)okResult.Value;
            Assert.AreEqual(1, product.ProductId);
        }

        [TestMethod]
        public void GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var mockHandler = new Mock<IQueryHandler<GetProductByIdQuery, Product>>();
            mockHandler.Setup(handler => handler.Handle(It.IsAny<GetProductByIdQuery>()))
                       .Returns((Product)null);

            var controller = new ProductController(
                createProductCommandHandler: Mock.Of<ICommandHandler<CreateProductCommand>>(),
                updateProductCommandHandler: Mock.Of<ICommandHandler<UpdateProductCommand>>(),
                getAllProductsQueryHandler: Mock.Of<IQueryHandler<GetAllProductsQuery, List<Product>>>(),
                getProductByIdQueryHandler: mockHandler.Object,
                getProductStatusQueryHandler: Mock.Of<IQueryHandler<GetProductStatusQuery, Dictionary<int, string>>>()
            );

            // Act
            var result = controller.GetById(productId: 1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetById_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var mockHandler = new Mock<IQueryHandler<GetProductByIdQuery, Product>>();
            mockHandler.Setup(handler => handler.Handle(It.IsAny<GetProductByIdQuery>()))
                       .Throws(new Exception("Test exception"));

            var controller = new ProductController(
                createProductCommandHandler: Mock.Of<ICommandHandler<CreateProductCommand>>(),
                updateProductCommandHandler: Mock.Of<ICommandHandler<UpdateProductCommand>>(),
                getAllProductsQueryHandler: Mock.Of<IQueryHandler<GetAllProductsQuery, List<Product>>>(),
                getProductByIdQueryHandler: mockHandler.Object,
                getProductStatusQueryHandler: Mock.Of<IQueryHandler<GetProductStatusQuery, Dictionary<int, string>>>()
            );

            // Act
            var result = controller.GetById(productId: 1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result.Result;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [TestMethod]
        public void UpdateProduct_ReturnsOkResult_WhenUpdateSucceeds()
        {
            // Arrange
            var mockHandler = new Mock<ICommandHandler<UpdateProductCommand>>();
            var controller = new ProductController(
                createProductCommandHandler: Mock.Of<ICommandHandler<CreateProductCommand>>(),
                updateProductCommandHandler: mockHandler.Object,
                getAllProductsQueryHandler: Mock.Of<IQueryHandler<GetAllProductsQuery, List<Product>>>(),
                getProductByIdQueryHandler: Mock.Of<IQueryHandler<GetProductByIdQuery, Product>>(),
                getProductStatusQueryHandler: Mock.Of<IQueryHandler<GetProductStatusQuery, Dictionary<int, string>>>()
            );

            // Act
            var result = controller.UpdateProduct(productId: 1, command: new UpdateProductCommand());

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual("Producto actualizado exitosamente", okResult.Value);
            mockHandler.Verify(handler => handler.Handle(It.IsAny<UpdateProductCommand>()), Times.Once);
        }

        [TestMethod]
        public void UpdateProduct_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var mockHandler = new Mock<ICommandHandler<UpdateProductCommand>>();
            mockHandler.Setup(handler => handler.Handle(It.IsAny<UpdateProductCommand>()))
                       .Throws(new Exception("Test exception"));
            var controller = new ProductController(
                createProductCommandHandler: Mock.Of<ICommandHandler<CreateProductCommand>>(),
                updateProductCommandHandler: mockHandler.Object,
                getAllProductsQueryHandler: Mock.Of<IQueryHandler<GetAllProductsQuery, List<Product>>>(),
                getProductByIdQueryHandler: Mock.Of<IQueryHandler<GetProductByIdQuery, Product>>(),
                getProductStatusQueryHandler: Mock.Of<IQueryHandler<GetProductStatusQuery, Dictionary<int, string>>>()
            );

            // Act
            var result = controller.UpdateProduct(productId: 1, command: new UpdateProductCommand());

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("Test exception", objectResult.Value);
            mockHandler.Verify(handler => handler.Handle(It.IsAny<UpdateProductCommand>()), Times.Once);
        }



    }
}
