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
        private readonly HttpClient _httpClient;
        public ORSUtils()
        {
            _httpClient = new HttpClient();
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", ApiKeys.ORS_API_KEY); // On fait ça pour skip les verifications qui bloque l'ajout, étrange mais en forcant ça passe
                _httpClient.Timeout = TimeSpan.FromSeconds(15);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ORS] - Erreur en ajoutant le header : {ex}");
            }
        }

        public async Task<JsonDocument> computeComplexItinerary(List<Station> allStations, Position startPos, Position endPos)
        {
            Itinerary itinerary = new Itinerary();
            await recursiveItinerary(startPos, endPos, itinerary, allStations);

            return CreateBestRouteJson(itinerary);          
        }

        private async Task<Itinerary> recursiveItinerary(Position startPosition, Position endPosition, Itinerary itinerary, List<Station> allStations)
        {
            if(startPosition == endPosition)
            {
                return new Itinerary();
            }

            JsonDocument walkGlobalRoute = await getRoute(startPosition, endPosition, "foot-walking");
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
                    double distance = GeoUtils.HaversineDistance(endCurrentStation.position, endPosition);

                    if (distance < bestDistance)
                    {
                        endCurrentStation = closestEndStation;
                        bestDistance = distance;
                    }
                }

                JsonDocument walkRoute1 = await getRoute(startPosition, closeStartStation.position, "foot-walking");
                JsonDocument bikeRoute = await getRoute(closeStartStation.position, endCurrentStation.position, "cycling-regular");
                JsonDocument walkRoute2 = await getRoute(endCurrentStation.position, endPosition, "foot-walking");

                double routeDuration = getDuration(walkRoute1) + getDuration(bikeRoute) + getDuration(walkRoute2);

                if(durationTime > routeDuration)
                {
                    durationTime = routeDuration;
                    bestEndStation = endCurrentStation;
                    bestItinerary = new Itinerary(new List<JsonDocument> { walkRoute1, walkRoute2 }, new List<JsonDocument> { bikeRoute } );
   
                }
            }


            Console.WriteLine(itinerary.pedestrianPath.Count);


            if (bestItinerary.getDuration() < walkItinerary.getDuration())
            {
                Console.WriteLine("1");
                itinerary.add(bestItinerary);
                Console.WriteLine("2");

                return await recursiveItinerary(bestEndStation.position, endPosition, itinerary , allStations);

            }
            else
            {
                Console.WriteLine("4");

                itinerary.add(walkItinerary);
                Console.WriteLine("3");

                return await recursiveItinerary(endPosition, endPosition, itinerary, allStations);
            }
        }


        private List<Station> findHaversineClosestStation(Position startPos, Position endPos, List<Station> allStations, int precision)
        {

            var stationsWithDistance = new List<(Station station, double distance)>();

            foreach (Station s in allStations)
            {

                double startToStation = GeoUtils.HaversineDistance(startPos, s.position);
                double stationToEnd = GeoUtils.HaversineDistance(s.position, endPos);
                double totalDistance = startToStation + stationToEnd;

                stationsWithDistance.Add((s, totalDistance));
            }

            var closestStations = stationsWithDistance
               .OrderBy(sd => sd.distance)
               .Take(precision) // Représente le nombre de station qu'on cherche autour de nous ( attention l'augmenter risque ban API ORS )
               .Select(sd => sd.station)
               .ToList();

            return closestStations;
        }


        public async Task<JsonDocument> computeBestItinerary(List<Station> startStations, List<Station> endStations, Position startPos, Position endPos)
        {

            JsonDocument walkingPath = await getRoute(startPos, endPos, "foot-walking");

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

                    JsonDocument walkStartPath = await getRoute(startPos, startStation.position, "foot-walking");
                    JsonDocument bikePath = await getRoute(startStation.position, endStation.position, "cycling-regular");
                    JsonDocument walkEndPath = await getRoute(endStation.position, endPos, "foot-walking");

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

        public async Task<JsonDocument> getRoute(Position startPosition, Position endPosition, string profile)
        {

            string url = $"https://api.openrouteservice.org/v2/directions/{profile}/geojson";


            // Le corps de la requête doit contenir les coordonnées [lon, lat]
            var body = new
            {
                coordinates = new[]
                {
                new[] { startPosition.longitude, startPosition.latitude },
                new[] { endPosition.longitude, endPosition.latitude }
            }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"[ORS] - POST vers {url}");
            HttpResponseMessage response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(responseString);
        }

    }
}
