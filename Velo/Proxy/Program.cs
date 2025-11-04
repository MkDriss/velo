using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    internal class Program

    {

        public static void main()
        {

            // Adresse de base (doit correspondre à App.config)
            Uri baseAddress = new Uri("http://localhost:8733/proxyCache/");

            // Création de l'hôte 
            using (ServiceHost host = new ServiceHost(typeof(ProxyCache), baseAddress))
            {
                try
                {
                    host.Open(); // Démarre le service

                    Console.WriteLine("Service SOAP ProxyCache démarré !");
                    Console.WriteLine("Adresse : " + baseAddress);
                    Console.WriteLine("Appuyez sur une touche pour arrêter le service...");
                    Console.ReadKey();

                    host.Close(); // Arrête le service proprement
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors du démarrage du service : " + ex.Message);
                    host.Abort();
                }
            }

        }

    }
}
