using System;
using System.IO;
using System.Text;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using System.Linq;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Subscriber
{
    public class SubscriberService
    {
        
        private IConfigurationRoot config;

        public string getRedisValue(string id)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect($"localhost:{config.GetValue<int>("Redis:port")}");
            IDatabase db = redis.GetDatabase();
            return db.StringGet(id);
        }

        public void WriteLog(string id)
        {
            string redisValue = getRedisValue(id);
            Console.WriteLine(id + " : " + redisValue);
        }

        public void Run(IConnection connection)
        {
            config = new ConfigurationBuilder()  
                .SetBasePath(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName + "/config")  
                .AddJsonFile("config.json", optional: false)  
                .Build();
            var greetings = connection.Observe(config.GetValue<string>("NATS:eventName"))
                    .Select(m => Encoding.Default.GetString(m.Data));

            greetings.Subscribe(msg => WriteLog(msg));
        }    
    }
}