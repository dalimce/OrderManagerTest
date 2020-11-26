using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace CustomerCLI
{
    public class OrderRequestManager
    {
        #region Properties
        private static ConnectionFactory factory;
        private string customerId = "";
        private string customerTag = "";
        private string[] Products;
        private IConnection connection;
        private IModel channel;

        #endregion
        #region Constructor
        public OrderRequestManager(string _customerGUID, string[] _products)
        {
            Products = _products;
            customerId = _customerGUID;
            customerTag = Common.Constants.RabbitMQCustomerPrefix + _customerGUID;
            factory = new ConnectionFactory() { HostName = Common.Constants.InternalHost };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            ReceiveResponse();
        }
        #endregion
        #region Send Receive Actions
        public void ReceiveResponse()
        {
           channel.ExchangeDeclare(exchange: Common.Constants.RabbitMQExchange, type: "direct");

            channel.QueueDeclare(queue: customerTag,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: true,
                                 arguments: null);
            channel.QueueBind(queue: customerTag,
                exchange: Common.Constants.RabbitMQExchange,
                routingKey: customerTag);
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                string responseStr = Encoding.UTF8.GetString(body);
                OrderResponseModel orderResponse = JsonConvert.DeserializeObject<OrderResponseModel>(responseStr);
                Console.WriteLine("{0} x '{1}' => {2} (Client ID = '{3}')", orderResponse.Quantity, orderResponse.ProductName, orderResponse.IsError ? "Couldn't Purchased" : "Purchased", customerId);
            };
            channel.BasicConsume(queue: customerTag,
                                     autoAck: true,
                                     consumer: consumer);
        }
        public void SendRequest(Object o)
        {
            Random r = new Random();
            int randQuantity = r.Next(1, 10);
            int randProductSelect = r.Next(0, Products.Length);

            var sampleOrderRequest = new OrderRequestModel()
            {
                ProductId = Products[randProductSelect],
                Quantity = randQuantity,
                RequestCustomerId = customerId
            };
            channel.ExchangeDeclare(exchange: Common.Constants.RabbitMQExchange, type: "direct");
            var message = JsonConvert.SerializeObject(sampleOrderRequest);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: Common.Constants.RabbitMQExchange, routingKey: Common.Constants.RabbitMQOrderRoutingKey, basicProperties: null, body: body);
            Console.WriteLine("Order Request Sended", message);
            GC.Collect();
        }

        #endregion
    }
}
