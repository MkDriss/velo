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

            // --- Démarrer GPSServer ---
            var gpsAddress = new Uri("http://localhost:8701/GPSServer/");
            using (ServiceHost gpsHost = new ServiceHost(typeof(GPS), gpsAddress))
            {
                gpsHost.Open();
                Console.WriteLine("GPSServer running...");
                Console.WriteLine("SOAP WSDL: " + gpsAddress + "?wsdl");
                Console.WriteLine("REST endpoint: " + gpsAddress + "rest/");
                Console.WriteLine("Press Enter to stop...");
                Console.ReadLine();
            }

            // -- Arrêter Proxy proprement --
            proxyHost.Close();
        }
    }
}
