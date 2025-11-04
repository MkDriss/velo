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


        public Stations GetStations(string contract)
        {
            var stations = stationsCache.Get<Stations>(contract);
            return stations;
        }

        public string GetContract(string cityName)
        {
            var contract = contractCache.Get<Contract>(cityName);
            return contract.contractName;
        }



    }
}
