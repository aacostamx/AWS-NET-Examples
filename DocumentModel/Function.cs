using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using DocumentModel.Model;
using System;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace DocumentModel
{
    public class Function
    {
        public async Task<Order> FunctionHandler(Order input, ILambdaContext context)
        {
            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    var orderTable = Table.LoadTable(client, "Orders");

                    int orderId = new Random().Next(1000000);
                    string customerId = Guid.NewGuid().ToString();

                    var order = new Document
                    {
                        ["OrderId"] = orderId,
                        ["CustomerId"] = customerId,
                        ["ShipMethod"] = input.ShipMethod,
                        ["Date"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        ["Status"] = input.Status,
                    };

                    await orderTable.PutItemAsync(order);

                    order = await orderTable.GetItemAsync(orderId, customerId);

                    input.OrderId = int.Parse(order["OrderId"]);
                    input.CustomerId = order["CustomerId"];
                    input.ShipMethod = order["ShipMethod"];
                    input.Date = DateTime.Parse(order["Date"]);
                    input.Status = Convert.ToBoolean(int.Parse(order["Status"]));
                }
            }
            catch (Exception ex)
            {
                context.Logger.Log(ex.ToString());
            }

            return input;
        }
    }
}
