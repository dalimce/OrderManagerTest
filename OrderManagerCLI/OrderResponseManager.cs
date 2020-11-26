using Common;
using Newtonsoft.Json;
using OrderManagerCLI.CacheManagers;
using OrderManagerCLI.ContextModels;
using OrderManagerCLI.Contexts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OrderManagerCLI
{
    public class OrderResponseManager
    {
        private ConnectionFactory factory;
        private RedisCacheManager redis;
        public OrderResponseManager()
        {
            redis = new RedisCacheManager();
            factory = new ConnectionFactory() { HostName = Common.Constants.InternalHost };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();


            channel.ExchangeDeclare(exchange: Common.Constants.RabbitMQExchange, type: "direct");

            channel.QueueDeclare(queue: Common.Constants.RabbitMQOrderQueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: Common.Constants.RabbitMQOrderQueueName,
                exchange: Common.Constants.RabbitMQExchange,
                routingKey: Common.Constants.RabbitMQOrderRoutingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string orderRequestStr = Encoding.UTF8.GetString(body);
                OrderRequestModel orderRequest = JsonConvert.DeserializeObject<OrderRequestModel>(orderRequestStr);
                while (!redis.SetKey(orderRequest.ProductId, "lock", TimeSpan.FromSeconds(3)))
                {
                    Console.WriteLine("Waiting For Access To => {0}", orderRequest.ProductId);
                    Thread.Sleep(100);
                }
                using (ECommerce2Context db = new ECommerce2Context())
                {
                    Product p = db.Products.FirstOrDefault(x => x.ProductId == orderRequest.ProductId);
                    if (p != null)
                    {
                        OrderResponseModel orderResponse = new OrderResponseModel()
                        {
                            IsError = true,
                            Quantity = orderRequest.Quantity,
                            ProductId = orderRequest.ProductId,
                            ProductName = p.ProductName
                        };
                        if ((p.Quantity - orderRequest.Quantity) > 0)
                        {
                            Console.WriteLine("{0} x '{1}' Product Sold For (Customer Id = '{2}')", orderRequest.Quantity, p.ProductName,orderRequest.RequestCustomerId);
                            p.Quantity -= orderRequest.Quantity;
                            db.SaveChanges();
                            orderResponse.IsError = false;
                        }
                        else
                        {
                            Console.WriteLine("{0} x '{1}' Product Could't Sold (Customer Id = '{2}') - SQ: {3}", orderRequest.Quantity, p.ProductName,orderRequest.RequestCustomerId, p.Quantity);
                        }

                        string orderResponseStr = JsonConvert.SerializeObject(orderResponse);
                        byte[] requestBody = Encoding.UTF8.GetBytes(orderResponseStr);
                        try
                        {
                            channel.BasicPublish(exchange: Common.Constants.RabbitMQExchange, routingKey: Common.Constants.RabbitMQCustomerPrefix + orderRequest.RequestCustomerId, basicProperties: null, body: requestBody);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Customer Gone Away => {0}", orderRequest.RequestCustomerId);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Product Cannot Found => {0} ", orderRequest.ProductId);
                    }
                    
                }
                redis.RemoveLockKey(orderRequest.ProductId);//RELASE LOCK
            };
            channel.BasicConsume(queue: Common.Constants.RabbitMQOrderQueueName,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}
