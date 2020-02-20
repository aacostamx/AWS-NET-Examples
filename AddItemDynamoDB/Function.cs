using AddItemDynamoDB.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace AddItemDynamoDB
{
    public class Function
    {
        public async void FunctionHandler(Order order, ILambdaContext context)
        {
            await InsertOrderDocumentAsync(order, context);
            await InsertOrderDictionaryAsync(order, context);
        }

        /// <summary>
        /// Add item into DynamoDB Using a Document Amazon Class
        /// </summary>
        /// <param name="order"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<bool> InsertOrderDocumentAsync(Order order, ILambdaContext context)
        {
            bool success = false;

            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    var table = Table.LoadTable(client, "Orders");
                    var item = new Document
                    {
                        ["OrderId"] = new Random().Next(1000000),
                        ["CustomerId"] = order.CustomerId,
                        ["ShipMethod"] = order.ShipMethod,
                        ["Date"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        ["Status"] = order.Status
                    };

                    await table.PutItemAsync(item);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.ToString());
            }

            return success;
        }

        /// <summary>
        /// Add item into DynamoDB using a Dictionary
        /// </summary>
        /// <param name="order"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<bool> InsertOrderDictionaryAsync(Order order, ILambdaContext context)
        {
            bool sucess = false;

            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    var request = new PutItemRequest
                    {
                        TableName = "Orders",
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { "OrderId", new AttributeValue { N = new Random().Next(1000000).ToString() }},
                            { "CustomerId", new AttributeValue { S = order.CustomerId }},
                            { "ShipMethod", new AttributeValue { S = order.ShipMethod }},
                            { "Date", new AttributeValue { S = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") }},
                            { "Status", new AttributeValue { S = Convert.ToInt32(order.Status).ToString() }},
                        }
                    };

                    await client.PutItemAsync(request);
                    sucess = true;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.ToString());
            }

            return sucess;
        }

    }
}
