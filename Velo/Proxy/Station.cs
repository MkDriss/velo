using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    internal class Station
    {
        private int number;

        private string contract_name;

        private string name;

        private string address;

        private Position position;

        private bool banking;

        private bool bonus;

        private int bike_stands;

        private int available_bike_stands;


        private int available_bike;


        private string status;

        public Station()
        {
            position = new Position(1, 2);
        }

    }
}
