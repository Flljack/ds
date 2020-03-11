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
    public class IndexModel : PageModel
    {

        public string TaskId { get; set; }

        public bool ShowTaskId => !string.IsNullOrEmpty(TaskId);

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string description)
        {   
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var config = new ConfigurationBuilder()  
                        .SetBasePath(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName + "/config")  
                        .AddJsonFile("config.json", optional: false)  
                        .Build(); 
            using var channel = GrpcChannel.ForAddress($"http://localhost:{config.GetValue<int>("BackendAPI:port")}");
            var client = new Job.JobClient(channel);
            var reply = await client.RegisterAsync(
                              new RegisterRequest { Description = description });
            TaskId =  reply.Id;
            return Page();
        }
    }
}
