using CloudNauticalAPI.Models.Domain;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Security.Cryptography.Xml;
using System.Security.Principal;
using System.Text;

namespace CloudNauticalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryDetailsController : ControllerBase
    {
        

        [HttpPost]
        public IActionResult GetUserDeliveryDetails(string user, string customerID)
        {
            if(user == null || customerID == null) 
            {
                return BadRequest();
            }

            // Models to create response
            var customer = new Customers();
            var order = new Orders();
            var orderItems = new List<OrderItems>();

            // Flag to check if it is a gift
            int isGift = 0;

            // PLEASE CHANGE CONNECTION STRING ACCORDINGLY
            using(SqlConnection connection = new SqlConnection("Server = localhost; Database = master; Trusted_Connection = True;"))
            {
                
                // Writing SQL Query
                StringBuilder sb = new StringBuilder();
                sb.Append("select FIRSTNAME, LASTNAME, HOUSENO, STREET, TOWN, POSTCODE from customers where email = @user and customerid = @customerID");

                SqlCommand cm = new SqlCommand(sb.ToString(),  connection);
                cm.Parameters.AddWithValue("@user", user);
                cm.Parameters.AddWithValue("@customerid", customerID);

                // Opening Connection  
                connection.Open();

                // Executing the SQL query  
                SqlDataReader sdr = cm.ExecuteReader();

                // Storing address for later
                StringBuilder address = new StringBuilder();
                
                // Parsing Data  
                while (sdr.Read())
                {
                    customer.firstName = (string)sdr["FIRSTNAME"];
                    customer.lastName = (string)sdr["LASTNAME"];
                    
                    address.Append((string)sdr["HOUSENO"] + " ");
                    address.Append((string)sdr["STREET"] + " ");
                    address.Append((string)sdr["TOWN"] + " ");
                    address.Append((string)sdr["POSTCODE"]);
                }

                sdr.Close();

                // Check if email and customer ID are a match before proceeding
                if (customer.firstName != null)
                {

                    sb.Clear();
                    sb.Append("select top 1 * from orders where CUSTOMERID = @customerID order by ORDERDATE desc");

                    cm = new SqlCommand(sb.ToString(), connection);
                    cm.Parameters.AddWithValue("@customerid", customerID);

                    sdr = cm.ExecuteReader();

                    //Parsing data
                    while (sdr.Read())
                    {
                        order.orderID = sdr["ORDERID"].ToString();
                        order.orderDate = sdr["ORDERDATE"].ToString();
                        order.deliveryAddress = address.ToString();
                        order.deliveryExpected = sdr["DELIVERYEXPECTED"].ToString();
                        isGift = Convert.ToInt32(sdr["CONTAINSGIFT"]);
                    }

                    sdr.Close();
                    sb.Clear();

                    // Check if there are any orders before proceeding
                    if (order.orderID != null)
                    {
                        sb.Append("select PRODUCTNAME, QUANTITY, PRICE from ORDERITEMS o , PRODUCTS p where orderid = @orderID and o.PRODUCTID = p.PRODUCTID;");

                        cm = new SqlCommand(sb.ToString(), connection);
                        cm.Parameters.AddWithValue("@orderID", order.orderID);

                        sdr = cm.ExecuteReader();

                        // Parsing data
                        while (sdr.Read())
                        {
                            var item = new OrderItems
                            {
                                Product = isGift == 1 ? "Gift" : (string)sdr["PRODUCTNAME"],
                                Quantity = (int)sdr["QUANTITY"],
                                PriceEach = (decimal)sdr["PRICE"]
                            };
                            orderItems.Add(item);
                        }
                    }
                    order.orderItems = orderItems;
                }
                else return BadRequest();
            }

            // Populating response model
            var response = new Response
            {
                Customer = customer,
                Order = order
            };

            return new JsonResult(response);
        }

    }
}
