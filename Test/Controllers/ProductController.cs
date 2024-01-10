using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Test.API.ActionFilter;
using Test.Application.Commands;
using Test.Application.Queries;
using Test.Application.Services;
using Test.Core.Entities;
using Test.Application.CommandHandlers;
using Test.Application.Handlers;

namespace Test.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly ICommandHandler<CreateProductCommand> _createProductCommandHandler;
        private readonly ICommandHandler<UpdateProductCommand> _updateProductCommandHandler;
        private readonly IQueryHandler<GetAllProductsQuery, List<Product>> _getAllProductsQueryHandler;
        private readonly IQueryHandler<GetProductByIdQuery, Product> _getProductByIdQueryHandler;
        private readonly IQueryHandler<GetProductStatusQuery, Dictionary<int, string>> _getProductStatusQueryHandler;
        public ProductController(
            ICommandHandler<CreateProductCommand> createProductCommandHandler,
            ICommandHandler<UpdateProductCommand> updateProductCommandHandler,
            IQueryHandler<GetAllProductsQuery, List<Product>> getAllProductsQueryHandler,
            IQueryHandler<GetProductByIdQuery, Product> getProductByIdQueryHandler,
            IQueryHandler<GetProductStatusQuery, Dictionary<int, string>> getProductStatusQueryHandler)
        {
            _createProductCommandHandler = createProductCommandHandler ?? throw new ArgumentNullException(nameof(createProductCommandHandler));
            _updateProductCommandHandler = updateProductCommandHandler ?? throw new ArgumentNullException(nameof(updateProductCommandHandler));
            _getAllProductsQueryHandler = getAllProductsQueryHandler ?? throw new ArgumentNullException(nameof(getAllProductsQueryHandler));
            _getProductByIdQueryHandler = getProductByIdQueryHandler ?? throw new ArgumentNullException(nameof(getProductByIdQueryHandler));
            _getProductStatusQueryHandler = getProductStatusQueryHandler ?? throw new ArgumentNullException(nameof(getProductStatusQueryHandler));
        }


        [HttpGet("status")]
        [ServiceFilter(typeof(LogResponseTimeFilter))]
        public ActionResult<Dictionary<int, string>> GetProductStatus()
        {
            try
            {
                var productStatus = _getProductStatusQueryHandler.Handle(new GetProductStatusQuery());
                return Ok(productStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("add")]
        [ServiceFilter(typeof(LogResponseTimeFilter))]
        public ActionResult AddProduct([FromBody] CreateProductCommand command)
        {
            try
            {
                _createProductCommandHandler.Handle(command);
                return Ok("Producto agregado exitosamente");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("all")]
        [ServiceFilter(typeof(LogResponseTimeFilter))]
        public ActionResult<List<Product>> GetAllProducts()
        {
            try
            {
                var products = _getAllProductsQueryHandler.Handle(new GetAllProductsQuery());
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("get/{productId}")]
        [ServiceFilter(typeof(LogResponseTimeFilter))]
        public ActionResult<Product> GetById(int productId)
        {
            try
            {
                var query = new GetProductByIdQuery { ProductId = productId };
                var product = _getProductByIdQueryHandler.Handle(query);

                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update/{productId}")]
        [ServiceFilter(typeof(LogResponseTimeFilter))]
        public ActionResult UpdateProduct(int productId, [FromBody] UpdateProductCommand command)
        {
            try
            {
                command.ProductId = productId;
                _updateProductCommandHandler.Handle(command);

                return Ok("Producto actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }
}
