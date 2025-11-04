using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Proxy;
using System.Runtime.Serialization;

namespace Proxy
{

    [DataContract]
    internal class Station
    {

        [DataMember] public int number;

        [DataMember] public string contract_name;

        [DataMember] public string name;

        [DataMember] public Position position;


        public Station(int number, string contract_name, string name, Position position) {
            this.number = number;
            this.contract_name = contract_name;
            this.name = name;
            this.position = position;
        }

        public Station() { }
    }
}


