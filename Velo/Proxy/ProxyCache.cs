using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    class ProxyCache : IProxyCache
    {

        private static GenericProxyCache stationsCache = new GenericProxyCache("stationsCache");
        private static GenericProxyCache contractCache = new GenericProxyCache("contractCache");
        private static GenericProxyCache orsCache = new GenericProxyCache("itineraryCache");

        public Stations GetStations(string contract)
        {
            if (string.IsNullOrEmpty(contract))
            {
                Console.WriteLine("[ProxyCache] Erreur : contract est null ou vide !");
                return null;
            }

            var stations = stationsCache.Get<Stations>(contract);
            if (stations == null)
            {
                Console.WriteLine($"[ProxyCache] Aucune station trouvée pour le contrat '{contract}'");
            }
            return stations;
        }

        public Stations GetAllStations()
        {

            var stations = stationsCache.GetAll<Stations>("ALL");
            if (stations == null)
            {
                Console.WriteLine($"[ProxyCache] Aucune station trouvée pour le GetAllStations");
            }
            return stations;
        }

        public string GetContract(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
                return null;

            var contract = contractCache.Get<Contract>(cityName);
            if (contract == null || string.IsNullOrEmpty(contract.contractName))
                return null;

            return contract.contractName;
        }

        public OrsResponse GetOrsResponse(string ors)
        {

            Console.WriteLine("[PROXY_CACHE] - ", ors);
            if (string.IsNullOrEmpty(ors)) return null;

            return orsCache.Get<OrsResponse>(ors);
            
        }
    }
}