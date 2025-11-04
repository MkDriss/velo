using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GPS_Server.ServiceReference1;

namespace GPS_Server
{
	// REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
	public class GPS : IGPS
	{
        public double GetItinerary(string address1, string address2)
        {

            Address start = new Address(address1);
            Address end = new Address(address2);

            using (var client = new ProxyCacheClient())
            {

                try
                {
                    string contract = client.GetContract(address1);
                    Stations stations = client.GetStations(contract);
                    Console.WriteLine($"[GPS] - Nombre de stations récupérées pour {address1} : {stations.stations}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GPS] - Erreur lors de l’appel ProxyCacheClient : {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            }


            Console.WriteLine($"[GPS] - Calcul d'itinérarire entre {address1} et {address2}");
            return 1234.5;

        }
    }
}
