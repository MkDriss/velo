using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    class ProxyCache : IProxyCache
    {

        private static GenericProxyCache stationsCache = new GenericProxyCache("stationsCache");
        private static GenericProxyCache contractCache = new GenericProxyCache("contractCache");
        private static GenericProxyCache orsCache = new GenericProxyCache("itineraryCache");
        private static GenericProxyCache addressCache = new GenericProxyCache("Address");

        public Stations GetStations(string contract)
        {
            if (string.IsNullOrEmpty(contract))
            {
                Console.WriteLine("[ProxyCache] Erreur : contract est null ou vide !");
                return null;
            }

            var stations = stationsCache.Get<Stations>(contract, 600.0); //10 minutes de délais
            if (stations == null)
            {
                Console.WriteLine($"[ProxyCache] Aucune station trouvée pour le contrat '{contract}'");
            }
            return stations;
        }

        public Stations GetAllStations()
        {

            var stations = stationsCache.GetAll<Stations>("ALL", 600.0); //10 minutes de délais
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

            var contract = contractCache.Get<Contract>(cityName, 86400.0); // 24h de délais
            if (contract == null || string.IsNullOrEmpty(contract.contractName))
                return null;

            return contract.contractName;
        }

        public OrsResponse GetOrsResponse(string ors)
        {

            
            if (string.IsNullOrEmpty(ors)) return null;

            return orsCache.Get<OrsResponse>(ors, 86400.0); // 24h de délais
            
        }

        public Address GetAddressCoordinates(string address)
        {
            if (string.IsNullOrEmpty(address)) return null;

            return addressCache.Get<Address>(address, DateTimeOffset.Now.AddYears(1));
        }
    }
}