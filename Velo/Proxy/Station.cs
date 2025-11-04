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

   /*     public string address;

        public Position position;

        public bool banking;

        public bool bonus;

        public int bike_stands;

        public int available_bike_stands;

        public int available_bike;

        public string status;*/

        public Station(int number, string contract_name, string name) {

            this.number = number;
            this.contract_name = contract_name;
            this.name = name;
        }

        public Station() { }
    }
}


