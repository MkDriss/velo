using GPS_Server.ProxyCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Server
{
    class JCDecauxUtils
    {
        public static string getContract(string cityName)
        {
            using (var client = new ProxyCacheClient())
            {
                try
                {
                    string contract = client.GetContract(cityName);
                    if (contract == null)
                    {
                        Console.WriteLine($"[GPS] - Aucun contract n'a été trouvé pour la ville : {cityName}");
                    }
                    else
                    {
                        Console.WriteLine($"[GPS] - Contract trouvé pour la ville : {cityName}");
                    }
                    return contract;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GPS] - Erreur lors de l’appel ProxyCacheClient : {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    return null;
                }
            }
        }

        public static Stations getStations(string contractName)
        {
            using (var client = new ProxyCacheClient())
            {

                try
                {
                    if (contractName != null)
                    {
                        Stations stations = client.GetStations(contractName);
                        Console.WriteLine($"[GPS] - Nombre de stations récupérées pour {contractName} : {stations.stations.Length}");
                        return stations;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GPS] - Erreur lors de l’appel ProxyCacheClient : {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    return null;
                }
            }

        }
    }
}
