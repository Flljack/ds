using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using BackendApi;
using Grpc.Net.Client;

namespace aspnetcoreapp.Pages
{
    public class TextDetailsModel : PageModel
    {

        public string Rating { get; set; }

        public bool ShowTaskDetails => !string.IsNullOrEmpty(Rating);

        private readonly ILogger<TextDetailsModel> _logger;

        public TextDetailsModel(ILogger<TextDetailsModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
           AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var config = new ConfigurationBuilder()  
                        .SetBasePath(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName + "/config")  
                        .AddJsonFile("config.json", optional: false)  
                        .Build(); 
            using var channel = GrpcChannel.ForAddress($"http://localhost:{config.GetValue<int>("BackendAPI:port")}");
            string taskId = HttpContext.Request.Query["jobId"].ToString();
            var client = new Job.JobClient(channel);
            var reply = await client.GetProcessingResultAsync(new RegisterResponse { Id = taskId });
            Rating = reply.Response;
            return Page();
        }
    }
}
