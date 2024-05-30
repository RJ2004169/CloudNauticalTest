using System.Security.Principal;

namespace CloudNauticalAPI.Models.Domain
{
    public class Orders
    {
        //        ORDERID INT IDENTITY PRIMARY KEY,
        //CUSTOMERID NVARCHAR(10),
        //ORDERDATE DATE,
        //DELIVERYEXPECTED DATE,
        //CONTAINSGIFT BIT,

        public string orderID { get; set; }
        public string orderDate { get; set; }
        public string deliveryAddress { get; set; }
        public List<OrderItems> orderItems { get; set; }
        public string deliveryExpected { get; set; }

    }
}
