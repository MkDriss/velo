using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Proxy; // important : référence au projet Proxy

namespace GPS_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // --- Démarrer ProxyCache via ton wrapper ---
            Uri proxyAddress = new Uri("http://localhost:8733/proxyCache/");
            ServiceHost proxyHost = ProxyHostFactory.Create(proxyAddress);
            proxyHost.Open();
            Console.WriteLine("ProxyCache running at " + proxyAddress);

            // --- Démarrer GPSServer (inchangé) ---
            var gpsAddress = new Uri("http://localhost:8701/GPSServer/");
            using (var gpsHost = new WebServiceHost(typeof(GPS), gpsAddress))
            {
                var endpoint = gpsHost.AddServiceEndpoint(typeof(IGPS), new WebHttpBinding(), "");
                endpoint.Behaviors.Add(new WebHttpBehavior());

                gpsHost.Open();
                Console.WriteLine("GPSServer running at " + gpsAddress);
                Console.WriteLine("Press Enter to stop both services...");
                Console.ReadLine();
            }

            // -- Arrêter Proxy proprement --
            proxyHost.Close();
        }
    }
}
