using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Wecollect_Kategorisierung_Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("WecollectKategorisierung wird gestartet.");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var log = new wecollect.Tools.EreignisLog();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Wasserstand kommt NICHT aus EMail_Postfach, sondern aus KI_Zusammenfassung:
                    // Kategorisierung greift ausschließlich auf diese Tabelle zu, nie auf
                    // EMail_Postfach. Alle SQL-Operationen laufen über die DLL-Klassen
                    // KI_Kategorisierung/KI_KategorisierungPersistenz, hier nur Methodenaufrufe.
                    int aktuelleMaxID = wecollect.KI.KI_Zusammenfassung.LiesMaxEMailID();
                    int letzteKategorisierungID = wecollect.KI.KI_Kategorisierung.LiesMaxEMailID();

                    if (aktuelleMaxID > letzteKategorisierungID)
                    {
                        wecollect.KI.KI_KategorisierungPersistenz.Synchronisiere(letzteKategorisierungID, aktuelleMaxID);
                    }
                }
                catch (Exception ex)
                {
                    log.Event_Log_Schreiben(
                        "Service-Log: Fehler bei KI_KategorisierungPersistenz.Synchronisiere: " + ex.Message,
                        System.Diagnostics.EventLogEntryType.Error);
                }

                await Task.Delay(10000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("WecollectKategorisierung wird gestoppt.");
            return base.StopAsync(cancellationToken);
        }
    }
}
