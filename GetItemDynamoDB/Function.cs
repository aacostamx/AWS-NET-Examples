using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using GetItemDynamoDB.Model;
using System;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace GetItemDynamoDB
{
    public class Function
    {
        public async Task<Order> FunctionHandler(int orderId, ILambdaContext context)
        {
            var order = new Order();

            try
            {
                order = await GetItemFromTable(orderId);
                order = await InstanceGetItemfromTable(orderId);
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.ToString());
            }

            return order;
        }

        /// <summary>
        /// Getting an Item from a Table
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task<Order> GetItemFromTable(int orderId)
        {
            var order = new Order();

            using (var client = new AmazonDynamoDBClient())
            {
                if (!(await client.ListTablesAsync()).TableNames.Contains("Orders"))
                {
                    var table = Table.LoadTable(client, "Orders");
                    Document item = await table.GetItemAsync(orderId);
                    order.OrderId = item["OrderId"].AsInt();
                    order.CustomerId = item["CustomerId"];
                    order.ShipMethod = item["ShipMethod"];
                    order.Date = item["Date"].AsDateTime();
                    order.Status = item["Status"].AsBoolean();
                }
            }

            return order;
        }

        /// <summary>
        /// Using an Instance of a .NET Object to Get an Item from a Table
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private async Task<Order> InstanceGetItemfromTable(int orderId)
        {
            var order = new Order();

            using (var client = new AmazonDynamoDBClient())
            {
                if (!(await client.ListTablesAsync()).TableNames.Contains("Orders"))
                {

                    var context = new DynamoDBContext(client);
                    order = await context.LoadAsync<Order>(orderId);
                }
            }

            return order;
        }
    }
}
