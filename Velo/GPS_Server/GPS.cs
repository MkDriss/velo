using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace GPS_Server
{
	// REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
	public class GPS : IGPS
	{
        public double GetItinerary(string address1, string address2)
        {


            Console.WriteLine($"[GPS] - Calcul d'itinérarire entre {address1} et {address2}");
            return 1234.5;

        }
    }
}
