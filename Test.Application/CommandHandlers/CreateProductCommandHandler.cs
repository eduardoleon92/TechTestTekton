using System;
using Test.Application.Commands;
using Test.Application.Handlers;
using Test.DataAccess.Repositories;
using LazyCache;

namespace Test.Application.CommandHandlers
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IAppCache _cache;

        public CreateProductCommandHandler(IProductRepository productRepository, IAppCache cache)
        {
            _productRepository = productRepository;
            _cache = cache;
        }

        public void Handle(CreateProductCommand command)
        {
            try
            {
                _productRepository.AddProduct(command.Name, command.Stock, command.Description, command.Price);
                _cache.Add("CreateProductCommand_Success", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el producto '{command.Name}': {ex.Message}");
                _cache.Add("CreateProductCommand_Success", false);
            }

            Console.WriteLine($"Producto '{command.Name}' creado con éxito.");
        }
    }
}
