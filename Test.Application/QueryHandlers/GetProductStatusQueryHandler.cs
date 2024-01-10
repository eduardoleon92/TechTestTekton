using System;
using System.Collections.Generic;
using Test.Application.Handlers;
using Test.Application.Queries;
using LazyCache;

namespace Test.Application.QueryHandlers
{
    public class GetProductStatusQueryHandler : IQueryHandler<GetProductStatusQuery, Dictionary<int, string>>
    {
        private readonly IAppCache _cache;

        public GetProductStatusQueryHandler(IAppCache cache)
        {
            _cache = cache;
        }

        public Dictionary<int, string> Handle(GetProductStatusQuery query)
        {
            string cacheKey = "ProductStatus";
            return _cache.GetOrAdd(cacheKey, () => LoadProductStatus(), TimeSpan.FromMinutes(5));
        }

        private Dictionary<int, string> LoadProductStatus()
        {
            Console.WriteLine("Cargando datos de estados de productos...");

            var productStatus = new Dictionary<int, string>
            {
                { 1, "Active" },
                { 0, "Inactive" }
            };

            return productStatus;
        }
    }
}
