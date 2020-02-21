using Amazon.DynamoDBv2.DataModel;
using System;

namespace ObjectPersistenceModel.Model
{
    public class Orders
    {
        [DynamoDBHashKey]
        public int OrderId { get; set; }
        [DynamoDBRangeKey]
        public string CustomerId { get; set; }
        public string ShipMethod { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
    }
}
