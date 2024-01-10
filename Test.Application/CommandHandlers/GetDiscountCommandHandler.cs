using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Application.Commands;
using Test.Application.Handlers;

// Implementación de la interfaz en GetDiscountCommandHandler.cs
namespace Test.Application.CommandHandlers
{
    public class GetDiscountCommandHandler : IDiscountCommandHandler
    {
        public decimal GetDiscount(int productId)
        {
            var mockApiUrl = "https://659763c5668d248edf22cfe6.mockapi.io/api/v1/discount";
            try
            {
                var httpClient = new HttpClient();
                var response = httpClient.GetStringAsync(mockApiUrl).Result;
                var discounts = JsonConvert.DeserializeObject<List<DiscountResponse>>(response);
                var discountObject = discounts.FirstOrDefault(d => d.Id == productId.ToString());

                if (discountObject != null)
                {
                    return discountObject.Discount;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener descuento del servicio externo: {ex.Message}");
            }

            return 0;
        }

        private class DiscountResponse
        {
            public string Id { get; set; }
            public decimal Discount { get; set; }
        }
    }
}

