using System.Threading;

namespace Wecollect_Kategorisierung_Service
{
    /// <summary>
    /// Verhindert Doppel-Instanzen dieses Dienstes (gleiches Risiko wie bei
    /// Wecollect-EMailWatcher-Service: zwei gleichzeitige Instanzen würden dieselbe
    /// EMailID doppelt kategorisieren, siehe KI_KategorisierungPersistenz.Synchronisiere).
    /// </summary>
    public class SingleInstanceGuard
    {
        // Referenz MUSS gehalten werden, sonst könnte der Garbage Collector den Mutex
        // vorzeitig freigeben, während der Prozess noch läuft.
        private readonly Mutex _mutex;

        public bool IstEinzigeInstanz { get; }

        public SingleInstanceGuard(string dienstName)
        {
            _mutex = new Mutex(initiallyOwned: true, name: "Global\\" + dienstName + "_SingleInstance", createdNew: out bool neu);
            IstEinzigeInstanz = neu;
        }
    }
}
