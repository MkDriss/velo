using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPS_Server
{
    class Itinerary
    {

        public List<JsonDocument> bikingPath;
        public List<JsonDocument> pedestrianPath;

        public Itinerary(List<JsonDocument> pedestrianPath, List<JsonDocument> bikingPath)
        {
            this.bikingPath = bikingPath;
            this.pedestrianPath = pedestrianPath;
        }

        public Itinerary(List<JsonDocument> pedestrianPath)
        {
            this.pedestrianPath = pedestrianPath;
            this.bikingPath = new List<JsonDocument>();
        }

        public Itinerary()
        {
            this.pedestrianPath = new List<JsonDocument>();
            this.bikingPath = new List<JsonDocument>();
        }

        public void add(Itinerary it2)
        {
            this.bikingPath.Concat(it2.bikingPath);
            this.pedestrianPath.Concat(it2.pedestrianPath);
        }
        
        public double getDuration()
        {
            double total = 0;
            Console.WriteLine("1");

            foreach (JsonDocument doc in pedestrianPath)
            {
                Console.WriteLine("1");

                total += getDurationJson(doc);
            }

            Console.WriteLine("1");

            foreach (JsonDocument doc in bikingPath)
            {
                total += getDurationJson(doc);
            }

            return total;
        }

        private double getDurationJson(JsonDocument doc)
        {
            try
            {
                return doc.RootElement
                          .GetProperty("features")[0]
                          .GetProperty("properties")
                          .GetProperty("summary")
                          .GetProperty("duration")
                          .GetDouble();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ORS] - getDuration error ");
                Console.WriteLine(e);
                Console.WriteLine(doc.RootElement.GetRawText());
                return 0;
            }
        }

    }
}
