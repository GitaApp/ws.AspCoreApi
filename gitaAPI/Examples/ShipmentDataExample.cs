// ShipmentDataExample.cs fájl az Examples mappában
using gitaAPI.Entities;
using Swashbuckle.AspNetCore.Filters;
namespace gitaAPI.Examples
{
    public class ShipmentDataExample : IExamplesProvider<ShipmentResponse>
    {
        public ShipmentResponse GetExamples()
        {
            return new ShipmentResponse
            {
                ok = 123,
                err = "Example Carrier",
                // További példa adatok
            };
        }
    }
}