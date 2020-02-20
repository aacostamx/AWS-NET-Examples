using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using AddItemDynamoDB;
using AddItemDynamoDB.Model;
using Amazon.DynamoDBv2.DocumentModel;

namespace AddItemDynamoDB.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task Add_Item_DynamoDBAsync()
        {
            var function = new Function();
            var context = new TestLambdaContext();
            var random = new Random();
            string[] Ships = { "FedEx", "UPS", "DHL", "USPS" };

            var order = new Order
            {
                OrderId = random.Next(1000000),
                CustomerId = Guid.NewGuid().ToString(),
                ShipMethod = Ships[random.Next(Ships.Length)],
                Status = Convert.ToBoolean(random.Next(0, 2))
            };
            bool result = await function.FunctionHandler(order, context);

            Assert.True(result);
        }
    }
}
