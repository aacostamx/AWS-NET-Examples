using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using ObjectPersistenceModel.Model;
using System;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace ObjectPersistenceModel
{
    public class Function
    {
        public async Task<Orders> FunctionHandler(Orders input, ILambdaContext context)
        {
            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    using (var dbContext = new DynamoDBContext(client))
                    {
                        int orderId = new Random().Next(1000000);
                        string customerId = Guid.NewGuid().ToString();

                        var order = new Orders
                        {
                            OrderId = orderId,
                            CustomerId = customerId,
                            ShipMethod = input.ShipMethod,
                            Date = DateTime.UtcNow,
                            Status = input.Status
                        };

                        await dbContext.SaveAsync(order);

                        input = await dbContext.LoadAsync<Orders>(orderId, customerId);
                    }
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
