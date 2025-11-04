using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Proxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    public interface IGenericProxyCache
    {
        T Get<T>(string CacheItemName);
        T Get<T>(string CacheItemName, double dt_seconds);
        T Get<T>(string CacheItemName, DateTimeOffset dt);

    }
}
