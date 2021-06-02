using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Net.Http;
using Topshelf;

namespace AgentService
{
    class Program
    {
        //private static readonly ILogger<Agent> _logger;
        //private static IHttpClientFactory _httpClientFactory;
        //private static readonly ISnmpData _printerSnmpData;
        //private static readonly IPingHost _pingHost;

        static void Main(string[] args)
        {
            //var builder = new ConfigurationBuilder();
            //BuildConfig(builder);

            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(builder.Build())
            //    .Enrich.FromLogContext()
            //    .WriteTo.Console()
            //    .CreateLogger();

            //Log.Logger.Information("Application Starting");

            //var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            //    .ConfigureServices((context, services) =>
            //    {
            //        services.AddTransient<IAgent, Agent>();
            //        services.AddHttpClient();
            //    })
            //    //.UseSerilog()
            //    .Build();

            //var svc = ActivatorUtilities.CreateInstance<Agent>(host.Services);
            //svc.StartAsync;



            // Topshelf
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<Agent>(s =>
                {
                    s.ConstructUsing(agent => new Agent());
                    s.WhenStarted(async agent => await agent.StartAsync());
                    s.WhenStopped(async agent => await agent.StopAsync());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("Agent");
                x.SetDisplayName("Agent");
                x.SetDescription("Printer agent that communicate with the devices on network via snmp protocol.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;

        }

        // 
        //static void BuildConfig(IConfigurationBuilder builder)
        //{
        //    builder.SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        //        .AddEnvironmentVariables();
        //}
    }
}

