using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Wecollect_Kategorisierung_Service
{
    public static class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            var guard = new SingleInstanceGuard("WecollectKategorisierung");
            if (!guard.IstEinzigeInstanz)
            {
                Console.WriteLine("Es läuft bereits eine Instanz. Beende...");
                new wecollect.Tools.EreignisLog().Event_Log_Schreiben(
                    "WecollectKategorisierung: Start abgebrochen, es läuft bereits eine andere Instanz.",
                    System.Diagnostics.EventLogEntryType.Warning);
                return;
            }

            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "WecollectKategorisierung";
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
