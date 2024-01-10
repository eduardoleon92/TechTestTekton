using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Test.API.ActionFilter;
using Test.Application.Services;
using Test.Core.Entities;

namespace Test.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet("get")]
        [ServiceFilter(typeof(LogResponseTimeFilter))]
        public ActionResult<Dictionary<int, string>> GetProductStatus()
        {
            try
            {
                var productStatus = _productService.GetProductStatus();
                return Ok(productStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("add")]
        [ServiceFilter(typeof(LogResponseTimeFilter))]
        public ActionResult AddProduct(string name, int stock, string description, decimal price)
        {
            try
            {
                _productService.AddProduct(name, stock, description, price);
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
                var products = _productService.GetAllProducts();
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
                var product = _productService.GetById(productId);
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
        public ActionResult UpdateProduct(int productId, [FromBody] ProductUpdateModel updateModel)
        {
            try
            {
                _productService.UpdateProduct(productId, updateModel.Name, updateModel.Stock, updateModel.Description, updateModel.Price);

                return Ok("Producto actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
