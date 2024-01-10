using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Application.Commands;
using Test.Application.Handlers;
using Test.DataAccess.Repositories;

namespace Test.Application.CommandHandlers
{
    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void Handle(UpdateProductCommand command)
        {
            _productRepository.UpdateProduct(command.ProductId, command.Name, command.Stock, command.Description, command.Price);
            Console.WriteLine($"Producto con ID {command.ProductId} actualizado con éxito.");
        }
    }
}
