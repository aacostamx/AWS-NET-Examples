using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace LambdaDynamoDB
{
    public class Function
    {
        public async void FunctionHandler(ILambdaContext context)
        {
            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    ListTablesResponse tableResponse = await client.ListTablesAsync();
                    if (!tableResponse.TableNames.Contains("Orders"))
                    {
                        var request = new CreateTableRequest
                        {
                            TableName = "Orders",
                            AttributeDefinitions = new List<AttributeDefinition>
                        {
                            new AttributeDefinition
                            {
                                AttributeName = "OrderId",
                                AttributeType = "N"
                            }
                        },
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement
                            {
                                AttributeName = "OrderId",
                                KeyType = "HASH"
                            }
                        },
                            ProvisionedThroughput = new ProvisionedThroughput
                            {
                                ReadCapacityUnits = 3,
                                WriteCapacityUnits = 1
                            },
                        };

                        CreateTableResponse response = await client.CreateTableAsync(request);
                        Console.WriteLine("Table created with request ID: " + response.ResponseMetadata.RequestId);
                    }
                }

                await InsertOrderDictionaryAsync();
                await InsertOrderDocumentAsync();
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.ToString());
            }
        }

        private static async Task InsertOrderDocumentAsync()
        {
            string[] Ships = { "FedEx", "UPS", "DHL", "USPS" };

            using (var client = new AmazonDynamoDBClient())
            {
                var random = new Random();

                var table = Table.LoadTable(client, "Orders");
                var item = new Document
                {
                    ["OrderId"] = random.Next(1000000),
                    ["CustomerId"] = Guid.NewGuid().ToString(),
                    ["ShipMethod"] = Ships[random.Next(Ships.Length)],
                    ["Date"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ["Status"] = random.Next(0, 2)
                };

                await table.PutItemAsync(item);
            }
        }

        private static async Task InsertOrderDictionaryAsync()
        {
            string[] Ships = { "FedEx", "UPS", "DHL", "USPS" };

            using (var client = new AmazonDynamoDBClient())
            {
                var random = new Random();

                var request = new PutItemRequest
                {
                    TableName = "Orders",
                    Item = new Dictionary<string, AttributeValue>
                    {
                        { "OrderId", new AttributeValue { N = random.Next(1000000).ToString() }},
                        { "CustomerId", new AttributeValue { S = Guid.NewGuid().ToString() }},
                        { "ShipMethod", new AttributeValue { S = Ships[random.Next(Ships.Length)] }},
                        { "Date", new AttributeValue { S = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") }},
                        { "Status", new AttributeValue { S = random.Next(0, 2).ToString() }},
                    }
                };

                await client.PutItemAsync(request);
            }
        }
    }
}
