using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    [DataContract]
    internal class Position
    {
        [DataMember] public double longitude;
        [DataMember] public double latitude;

        public Position(double longitude, double latitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;
        }

        public Position() { }
    }
}
