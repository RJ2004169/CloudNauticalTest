using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography.Xml;
using System.Security.Principal;

namespace CloudNauticalAPI.Models.Domain
{
    public class OrderItems
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal PriceEach { get; set; }
    }
}
