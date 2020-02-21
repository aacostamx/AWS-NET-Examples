using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Low_level.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace Low_level
{
    public class Function
    {
        public async Task<Order> FunctionHandler(Order order, ILambdaContext context)
        {
            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    int orderId = new Random().Next(1000000);
                    string customerId = Guid.NewGuid().ToString();
                    var request = new PutItemRequest
                    {
                        TableName = "Orders",
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { "OrderId", new AttributeValue { N = orderId.ToString()}},
                            { "CustomerId", new AttributeValue { S =  customerId}},
                            { "ShipMethod", new AttributeValue { S = order.ShipMethod }},
                            { "Date", new AttributeValue { S = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") }},
                            { "Status", new AttributeValue { S = Convert.ToInt32(order.Status).ToString() }},
                        }
                    };

                    await client.PutItemAsync(request);

                    Dictionary<string, AttributeValue> item = client.GetItemAsync(new GetItemRequest
                    {
                        TableName = "Orders",
                        Key = new Dictionary<string, AttributeValue>
                    {
                        { "OrderId", new AttributeValue { N = orderId.ToString() } },
                        { "OrderId", new AttributeValue { N = customerId } }
                    }
                    }).Result.Item;

                    order.OrderId = int.Parse(item["OrderId"].S);
                    order.CustomerId = item["CustomerId"].S;
                    order.ShipMethod = item["ShipMethod"].S;
                    order.Date = DateTime.Parse(item["Date"].S);
                    order.Status = bool.Parse(item["Status"].N);
                }
            }
            catch (Exception ex)
            {
                context.Logger.Log(ex.ToString());
            }
            return order;
        }
    }
}
