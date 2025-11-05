using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
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

        [DataMember] public int available_bikes;

        [DataMember] public int available_bikes_stands;


        public Station(int number, string contract_name, string name, Position position, int available_bikes, int available_bikes_stands) {
            this.number = number;
            this.contract_name = contract_name;
            this.name = name;
            this.position = position;
            this.available_bikes = available_bikes;
            this.available_bikes_stands = available_bikes_stands;
        }

        public Station() { }
    }
}


