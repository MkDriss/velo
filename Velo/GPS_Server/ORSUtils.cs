using GPS_Server.ProxyCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Proxy;

namespace GPS_Server
{
    class ORSUtils
    {

        private const double Epsilon = 1e-5;
        
        public ORSUtils(){}

        private bool PositionsEqual(Position a, Position b, double eps = Epsilon)
        {
            if (a == null || b == null) return false;
            return Math.Abs(a.latitude - b.latitude) < eps &&
                   Math.Abs(a.longitude - b.longitude) < eps;
        }
        public async Task<JsonDocument> computeComplexItinerary(List<Station> allStations, Position startPos, Position endPos)
        {
            Itinerary itinerary = new Itinerary();
            await recursiveItinerary(startPos, endPos, itinerary, allStations);
            Console.WriteLine($"TEST : {itinerary.pedestrianPath.Count}");
            return CreateBestRouteJson(itinerary);          
        }

        private async Task recursiveItinerary(Position startPosition, Position endPosition, Itinerary itinerary, List<Station> allStations)
        {

            if(PositionsEqual(startPosition, endPosition))
            {
                return;
            }

            JsonDocument walkGlobalRoute = getRoute(startPosition, endPosition, "foot-walking");
            Itinerary walkItinerary = new Itinerary(new List<JsonDocument>{ walkGlobalRoute });

            List<Station> closestStations = findHaversineClosestStation(startPosition, endPosition, allStations, 2);

            Station bestEndStation = null;
            Itinerary bestItinerary = null;
            double durationTime = double.MaxValue;

            foreach(Station closeStartStation in closestStations)
            {
                Stations sameContractStation = JCDecauxUtils.getStations(closeStartStation.contract_name);

                Station endCurrentStation = null;
                double bestDistance = double.MaxValue;
                
                foreach (Station closestEndStation in sameContractStation.stations)
                {
                    if(endCurrentStation == null)
                    {
                        endCurrentStation = closestEndStation;
                        bestDistance = GeoUtils.HaversineDistance(endCurrentStation.position, endPosition);
                        continue;
                    }

                    double distance = GeoUtils.HaversineDistance(closestEndStation.position, endPosition);

                    if (distance < bestDistance)    
                    {
                        endCurrentStation = closestEndStation;
                        bestDistance = distance;
                    }
                }

                JsonDocument walkRoute1 = getRoute(startPosition, closeStartStation.position, "foot-walking");
                JsonDocument bikeRoute = getRoute(closeStartStation.position, endCurrentStation.position, "cycling-regular");
                JsonDocument walkRoute2 = getRoute(endCurrentStation.position, endPosition, "foot-walking");

                double routeDuration = getDuration(walkRoute1) + getDuration(bikeRoute) + getDuration(walkRoute2);

                if(durationTime > routeDuration)
                {
                    durationTime = routeDuration;
                    bestEndStation = endCurrentStation;
                    bestItinerary = new Itinerary(new List<JsonDocument> { walkRoute1 }, new List<JsonDocument> { bikeRoute } );
   
                }
            }



            if(bestItinerary == null)
            {
                itinerary.add(walkItinerary);
                //await recursiveItinerary(endPosition, endPosition, itinerary, allStations);
                return;
            }


            if (bestItinerary.getDuration() < walkItinerary.getDuration())
            {
                itinerary.add(bestItinerary);

                allStations.RemoveAll(s => s.contract_name == bestEndStation.contract_name);

                await recursiveItinerary(bestEndStation.position, endPosition, itinerary , allStations);
                return;
            }

            else
            {
                itinerary.add(walkItinerary);

                allStations.RemoveAll(s => s.contract_name == bestEndStation.contract_name);

                // await recursiveItinerary(endPosition, endPosition, itinerary, allStations);
                return;
            }
        }


        public async Task<JsonDocument> computeThrowBikeSeine(List<Station> allStations, Position startPos)
        {
            Address seine = new Address("Quai de la Seine 75019 Paris");

            List<Station> closestStations = findHaversineClosestStation(startPos, seine.position, allStations, 2);

            Station closeStation = null;
            double closeDistance = double.MaxValue;
            Itinerary bestItin = null;

            foreach (Station s in closestStations)
            {
                JsonDocument sroute = await getRoute(startPos, s.position, "foot-walking");
                Itinerary sitin = new Itinerary(new List<JsonDocument> { sroute });
                if (closeStation == null)
                {
                    closeDistance = sitin.getDuration();
                    closeStation = s;
                    bestItin = sitin;
                }
                else
                {
                    if (sitin.getDuration() < closeDistance)
                    {
                        closeDistance = sitin.getDuration();
                        closeStation = s;
                        bestItin = sitin;
                    }
                }
            }


            JsonDocument goToStation = await getRoute(startPos, closeStation.position, "foot-walking");
            JsonDocument goToParis = await getRoute(closeStation.position, seine.position, "cycling-regular");

            Itinerary final = new Itinerary(new List<JsonDocument> { goToStation }, new List<JsonDocument> { goToParis });

            return CreateBestRouteJson(final);

        }
        private List<Station> findHaversineClosestStation(Position startPos, Position endPos, List<Station> allStations, int precision)
        {

            Console.WriteLine($"TEst : {allStations.Count}");
            var stationsWithDistance = new List<(Station station, double distance)>();

            foreach (Station s in allStations)
            {

                double startToStation = GeoUtils.HaversineDistance(startPos, s.position);

                stationsWithDistance.Add((s,startToStation));

            }

            var closestStations = stationsWithDistance
               .OrderBy(sd => sd.distance)
               .Take(precision) // Représente le nombre de station qu'on cherche autour de nous ( attention l'augmenter risque ban API ORS )
               .Select(sd => sd.station)
               .ToList();


            return closestStations;
        }


        public JsonDocument computeBestItinerary(List<Station> startStations, List<Station> endStations, Position startPos, Position endPos)
        {

            JsonDocument walkingPath = getRoute(startPos, endPos, "foot-walking");

            double bestTime = getDuration(walkingPath);
            List<JsonDocument> pedestrian = new List<JsonDocument> { walkingPath };
            List<JsonDocument> bike = new List<JsonDocument>();


            foreach (Station startStation in startStations)
            {
                foreach (Station endStation in endStations)
                {

                    if(startStation == endStation)
                    {
                        break;
                    }

                    JsonDocument walkStartPath = getRoute(startPos, startStation.position, "foot-walking");
                    JsonDocument bikePath = getRoute(startStation.position, endStation.position, "cycling-regular");
                    JsonDocument walkEndPath = getRoute(endStation.position, endPos, "foot-walking");

                    double totalTime = getDuration(walkStartPath) + getDuration(bikePath) + getDuration(walkEndPath);


                    if (totalTime < bestTime)
                    {
                        bestTime = totalTime;
                        pedestrian = new List<JsonDocument> { walkStartPath, walkEndPath };
                        bike = new List<JsonDocument> { bikePath };
                    }

                }
            }


            Itinerary itinerary = new Itinerary(pedestrian, bike);

            return CreateBestRouteJson(itinerary);
        }

        public JsonDocument CreateBestRouteJson(Itinerary itinerary)
        {
            List<JsonElement> ToElements(List<JsonDocument> docs)
            {
                var list = new List<JsonElement>();
                foreach (var doc in docs)
                {
                    list.Add(doc.RootElement.Clone());
                }
                return list;
            }

            var bestRouteObj = new
            {
                pedestrianPath = ToElements(itinerary.pedestrianPath),
                bikePath = ToElements(itinerary.bikingPath)
            };


            string jsonString = JsonSerializer.Serialize(bestRouteObj);
            Console.WriteLine("[ORS] - Best Route found !!");
            return JsonDocument.Parse(jsonString);
        }



        private double getDuration(JsonDocument doc)
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
            catch(Exception e)
            {
                Console.WriteLine("[ORS] - getDuration error ");
                Console.WriteLine(e);
                Console.WriteLine(doc.RootElement.GetRawText());
                return 0;
            }
        }

        public JsonDocument getRoute(Position startPosition, Position endPosition, string profile)
        {
            using (var client = new ProxyCacheClient())
            {

                var contextDto = new OrsContextDto
                {
                    StartPosition = new PositionDto
                    {
                        Longitude = startPosition.longitude,
                        Latitude = startPosition.latitude
                    },
                    EndPosition = new PositionDto
                    {
                        Longitude = endPosition.longitude,
                        Latitude = endPosition.latitude
                    }
                };

                string json = JsonSerializer.Serialize(contextDto);

                OrsResponse response = client.GetOrsResponse(json);

                Console.Write(response.value);

                return JsonDocument.Parse(response.value);

            }

        }

    }
}
