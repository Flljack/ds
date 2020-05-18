using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using NATS.Client;

namespace BackendApi.Services
{
    public class JobService : Job.JobBase
    {
        private readonly static Dictionary<string, string> _jobs = new Dictionary<string, string>();
        
        private readonly ILogger<JobService> _logger;

        private IConfigurationRoot config;


        private const int DELAY = 2000;
        private const int REQUEST_RETRY = 5;

        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
        }

        public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            config = new ConfigurationBuilder()  
                .SetBasePath(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName + "/config")  
                .AddJsonFile("config.json", optional: false)  
                .Build();

            string id = Guid.NewGuid().ToString();
            Console.WriteLine(request);
            saveToRedis(id + "_description", request.Description);
            saveToRedis(id + "_data", request.Data);
            publishEvent(id);
            var resp = new RegisterResponse
            {
                Id = id
            };
            _jobs[id] = request.Description;

            return Task.FromResult(resp);
        }

        public void publishEvent(string id) 
        {
            IConnection connection = new ConnectionFactory().CreateConnection(ConnectionFactory.GetDefaultOptions());
            byte[] payload = Encoding.Default.GetBytes(id);
            connection.Publish(config.GetValue<string>("NATS:eventName"), payload);
        }

        public void saveToRedis(string id, string value)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect($"localhost:{config.GetValue<int>("Redis:port")}");
            IDatabase db = redis.GetDatabase();
            db.StringSet(id, value);
        }
        private string getFromRedis(string id)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect($"localhost:{config.GetValue<int>("Redis:port")}");
            IDatabase db = redis.GetDatabase();
            return db.StringGet(id);
        }

        public override Task<ProcessingResult> GetProcessingResult(RegisterResponse registerResponse, ServerCallContext context)
        {
            config = new ConfigurationBuilder()  
                .SetBasePath(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName + "/config")  
                .AddJsonFile("config.json", optional: false)  
                .Build();
            string idResult = registerResponse.Id + "_rating";
            var result = new ProcessingResult {Status = ProcessingStatus.Processing, Response = ""};
            
            for (int i = 0; i < REQUEST_RETRY; i++)
            {
                string response = getFromRedis(idResult);
                if (response != null) {
                    result.Status = ProcessingStatus.Completed;
                    result.Response = response;
                    break;
                }
                Thread.Sleep(DELAY);
            }
            return Task.FromResult(result);
        }
    }
}