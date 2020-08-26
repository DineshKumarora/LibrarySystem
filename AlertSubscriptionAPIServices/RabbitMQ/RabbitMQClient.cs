using System;
using System.Text;
using AlertSubscriptionService.Models;
using RabbitMQ.Client;

namespace AlertSubscriptionService.RabbitMQ
{
    /// <summary>
    /// Rabbit MQ Client
    /// </summary>
    public class RabbitMQClient
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;

        private const string _exchangeName = "Topic_Exchange";
        private static string _alertQueueName = "Alert_Queue";

        /// <summary>
        /// RabbitMQ Client
        /// </summary>
        /// <param name="subscriber"></param>
        public RabbitMQClient(Subscription subscriber = null)
        {
            CreateConnection(subscriber);
        }

        /// <summary>
        /// Create Connection
        /// </summary>
        /// <param name="subscriber"></param>
        private static void CreateConnection(Subscription subscriber)
        {
            try
            {

                _factory = new ConnectionFactory
                {
                    HostName = "localhost", UserName = "guest", Password = "guest"
                };

                _connection = _factory.CreateConnection();
                _model = _connection.CreateModel();
                _model.ExchangeDeclare(_exchangeName, "topic");

                _alertQueueName = $"Alert_Queue_{subscriber.BookId}";
                _model.QueueDeclare(_alertQueueName, true, false, false, null);

                _model.QueueBind(_alertQueueName, _exchangeName, string.Empty);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Close Connection
        /// </summary>
        public void Close()
        {
            _model.Close();
            _connection.Close();
        }

        /// <summary>
        /// Add Alert
        /// </summary>
        /// <param name="subscriber"></param>
        public void AddAlert(Subscription subscriber)
        {
            SendMessage(subscriber.Serialize(), string.Empty);
        }

        /// <summary>
        /// Send Alert
        /// </summary>
        public void SendAlert()
        {
            StringBuilder str =  new StringBuilder();
            uint count = _model.MessageCount(_alertQueueName);
            for (int M = 0; M < count; M++)
            {
                BasicGetResult result = _model.BasicGet(_alertQueueName, false);
                var body = result.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                str.Append(message);

            }

            _model.QueueDelete(_alertQueueName, false, false);
        }

        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="routingKey">routing key</param>
        public void SendMessage(byte[] message, string routingKey)
        {
            _model.BasicPublish(_exchangeName, routingKey, null, message);
        }
    }  
}
