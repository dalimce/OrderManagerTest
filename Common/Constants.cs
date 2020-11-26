using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class Constants
    {
        public const string InternalHost = "host.docker.internal";
        public const string RabbitMQExchange = "order_manager";
        public const string RabbitMQOrderQueueName = "orders";
        public const string RabbitMQOrderRoutingKey = "order_request";
        public const string RabbitMQCustomerPrefix = "customer_";
        public const string RedisPass = "redis_pass";
        
    }
}
