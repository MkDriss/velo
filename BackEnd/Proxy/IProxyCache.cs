using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    [ServiceContract]
    interface IProxyCache
    {
        [OperationContract]
        Stations GetStations(string contractName);

        [OperationContract]
        string GetContract(string cityName);


        [OperationContract]
        Stations GetAllStations();

        [OperationContract]
        OrsResponse GetOrsResponse(string ors);

        [OperationContract]
        Address GetAddressCoordinates(string address);
        
    }
}
