using System;

namespace LambdaDynamoDB.Model
{
    public class Order
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }
}
