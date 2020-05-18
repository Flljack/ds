using System;
using System.IO;
using System.Text;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using System.Linq;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.RegularExpressions;

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

        public void setRedisValue(string id, string value)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect($"localhost:{config.GetValue<int>("Redis:port")}");
            IDatabase db = redis.GetDatabase();
            db.StringSet(id, value);
        }

        private float calcMessageRating(string message)
        {
            int countVowels = Regex.Matches(message, @"[aeiouауоыиэяюёе]", RegexOptions.IgnoreCase).Count;
            int countConsonants = Regex.Matches(message, @"[bcdfghjklmnpqrstvwxyzбвгджзйклмнпрстфхцчшщ]", RegexOptions.IgnoreCase).Count;
            return countConsonants > 0 ? countVowels / countConsonants : 0;
        }

        public void ProcessingTask(string id)
        {
            string redisValue = getRedisValue(id + "_data");
            float rating = calcMessageRating(redisValue);
            id += "_rating";
            setRedisValue(id, rating.ToString());
        }

        public void Run(IConnection connection)
        {
            config = new ConfigurationBuilder()  
                .SetBasePath(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName + "/config")  
                .AddJsonFile("config.json", optional: false)  
                .Build();
            var greetings = connection.Observe(config.GetValue<string>("NATS:eventName"))
                    .Select(m => Encoding.Default.GetString(m.Data));

            greetings.Subscribe(msg => ProcessingTask(msg));
        }    
    }
}