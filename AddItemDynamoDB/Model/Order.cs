using System;

namespace AddItemDynamoDB.Model
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public string ShipMethod { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
    }
}
