using Microsoft.Extensions.Configuration;
public interface IConfig
{
    IConfigurationRoot Config { get; }
}