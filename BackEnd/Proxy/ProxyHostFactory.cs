using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public static class ProxyHostFactory
    {
        public static ServiceHost Create(Uri baseAddress)
        {
            return new ServiceHost(typeof(ProxyCache), baseAddress);
        }
    }
}
