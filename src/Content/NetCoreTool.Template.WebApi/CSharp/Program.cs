#if (FrameworkNetCoreApp21)
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
#else
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
#endif
#if (HostingAzureSpringCloudOption && !FrameworkNetCoreApp21)
using Microsoft.Azure.SpringCloud.Client;
#endif
#if (DynamicLogging && FrameworkNetCoreApp21)
using Microsoft.Extensions.Logging;
#endif
#if (AnyHosting)
using Steeltoe.Common.Hosting;
#endif
#if (HostingCloudFoundryOption)
using Steeltoe.Extensions.Configuration.CloudFoundry;
#endif
#if (ConfigurationCloudConfigOption)
using Steeltoe.Extensions.Configuration.ConfigServer;
#endif
#if (ConfigurationPlaceholderOption)
#if (Steeltoe2)
using Steeltoe.Extensions.Configuration.PlaceholderCore;
#else
using Steeltoe.Extensions.Configuration.Placeholder;
#endif
#endif
#if (ConfigurationRandomValueOption)
using Steeltoe.Extensions.Configuration.RandomValue;
#endif
#if (DynamicLogging)
using Steeltoe.Extensions.Logging;
#endif
#if (MessagingRabbitMqOption)
#if (Steeltoe31)
#if (!FrameworkNetCoreApp21)
using Steeltoe.Messaging.RabbitMQ.Host;
#endif
#endif
#endif

namespace Company.WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if (FrameworkNetCoreApp21)
            CreateWebHostBuilder(args).Build().Run();
#else
            CreateHostBuilder(args).Build().Run();
#endif
        }

#if (FrameworkNetCoreApp21)
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
#if (ConfigurationCloudConfigOption)
                .AddConfigServer()
#endif
#if (ConfigurationPlaceholderOption)
                .AddPlaceholderResolver()
#endif
#if (ConfigurationRandomValueOption)
                .ConfigureAppConfiguration(b => b.AddRandomValueSource())
#endif
#if (AnyHosting)
                .UseCloudHosting(8080)
#endif
#if (HostingCloudFoundryOption)
                .AddCloudFoundryConfiguration()
#endif
#if (LoggingDynamicLoggerOption)
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    loggingBuilder.AddDynamicConsole();
                })
#endif
                .UseStartup<Startup>();
            return builder;
        }
#else
        public static IHostBuilder CreateHostBuilder(string[] args) =>
#if (MessagingRabbitMqOption)
#if (Steeltoe31)
#if (!FrameworkNetCoreApp21)
            RabbitHost.CreateDefaultBuilder()
#endif
#else
            Host.CreateDefaultBuilder(args)
#endif
#else
            Host.CreateDefaultBuilder(args)
#endif
#if (ConfigurationPlaceholderOption)
                .AddPlaceholderResolver()
#endif
#if (HostingAzureSpringCloudOption)
                .UseAzureSpringCloudService()
#endif
#if (AnyHosting)
                .UseCloudHosting(8080)
#endif
#if (HostingCloudFoundryOption)
                .AddCloudFoundryConfiguration()
#endif
#if (ConfigurationCloudConfigOption)
                .AddConfigServer()
#endif
#if (ConfigurationRandomValueOption)
                .ConfigureAppConfiguration(b => b.AddRandomValueSource())
#endif
#if (DynamicLogging)
                .ConfigureLogging((context, builder) => builder.AddDynamicConsole())
#endif
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
#endif
    }
}
