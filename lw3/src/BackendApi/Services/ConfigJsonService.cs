using Microsoft.Extensions.Configuration;
using System.IO;
public class ConfigJsonService : IConfig 
{

    public IConfigurationRoot Config
    {
        get { 
            return new ConfigurationBuilder()  
                .SetBasePath(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName + "/config")  
                .AddJsonFile("config.json", optional: false)  
                .Build();
            }
    }
}