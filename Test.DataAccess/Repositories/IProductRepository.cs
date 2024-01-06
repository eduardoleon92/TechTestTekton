using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Core.Entities;

namespace Test.DataAccess.Repositories
{
    public interface IProductRepository
    {
        Dictionary<int, string> GetProductStatus();
        void AddProduct(string name, int stock, string description, decimal price);
        List<Product> GetAllProducts();
        Product GetById(int productId);
        void UpdateProduct(int productId, string name, int stock, string description, decimal price);
    }
}
