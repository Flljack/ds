using System;
using NATS.Client;
using Subscriber;

namespace JobLogger
{
    class Program
    {
        static void Main(string[] args)
        {
           var subscriberService = new SubscriberService();


            using(IConnection connection = new ConnectionFactory().CreateConnection(ConnectionFactory.GetDefaultOptions())){
                subscriberService.Run(connection);
                Console.WriteLine("Events listening started. Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
