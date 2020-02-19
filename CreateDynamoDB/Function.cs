using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using System;
using System.Collections.Generic;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace CreateDynamoDB
{
    public class Function
    {
        public async void FunctionHandler(ILambdaContext context)
        {
            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    if (!(await client.ListTablesAsync()).TableNames.Contains("Orders"))
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
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.ToString());
            }
        }
    }
}
