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
            this.bikingPath = this.bikingPath.Concat(it2.bikingPath).ToList();
            this.pedestrianPath = this.pedestrianPath.Concat(it2.pedestrianPath).ToList();
        }

        public double getDuration()
        {
            double total = 0;

            foreach (JsonDocument doc in pedestrianPath)
            {

                total += getDurationJson(doc);
            }

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
                Console.WriteLine("[ORS] - getDuration error JSON");
                //Console.WriteLine(e);
                //Console.WriteLine(doc.RootElement.GetRawText());
                return 0;
            }
        }

    }
}
