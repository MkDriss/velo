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

            Position currentPosition = startPos;
            List<JsonDocument> finalPedestrian = new List<JsonDocument>();
            List<JsonDocument> finalBiking = new List<JsonDocument>();

            while(currentPosition != endPos) {

                var stationsWithDistance = new List<(Station station, double distance)>();

                foreach (Station station in allStations)
                {
                    double startToStation = GeoUtils.HaversineDistance(currentPosition, station.position);
                    double stationToEnd = GeoUtils.HaversineDistance(station.position, endPos);
                    double totalDistance = startToStation + stationToEnd;

                    stationsWithDistance.Add((station, totalDistance));
                }

                var closestStations = stationsWithDistance
                   .OrderBy(sd => sd.distance)
                   .Take(2) // Représente le nombre de station qu'on cherche autour de nous ( attention l'augmenter risque ban API ORS )
                   .Select(sd => sd.station)
                   .ToList();


                List<JsonDocument> pedestrian = null;
                List<JsonDocument> bike = null;
                double bestItineraryTime = double.MaxValue;
                Station endItineraryStation = null;

                foreach (Station station in closestStations)
                {
                    Stations currentCityStations = JCDecauxUtils.getStations(station.contract_name);

                    Station endCurrentStation = null;
                    double bestDistance = double.MaxValue;

                    foreach (Station endStation in currentCityStations.stations)
                    {
                        double distance = GeoUtils.HaversineDistance(endStation.position, endPos);

                        if(distance < bestDistance)
                        {
                            endCurrentStation = endStation;
                            bestDistance = distance;
                        }
                    }

                    JsonDocument walkRoute1 = await getRoute(currentPosition, station.position, "foot-walking");
                    JsonDocument bikeRoute = await getRoute(station.position, endCurrentStation.position, "cycling-regular");
                    JsonDocument walkRoute2 = await getRoute(endCurrentStation.position, endPos, "foot-walking");

                    double routeDuration = getDuration(walkRoute1) + getDuration(bikeRoute) + getDuration(walkRoute2);
                    if (routeDuration < bestItineraryTime)
                    {
                        pedestrian.Add(walkRoute1);
                        pedestrian.Add(walkRoute2);
                        bike.Add(bikeRoute);
                        endItineraryStation = endCurrentStation;
                        bestItineraryTime = routeDuration;
                    }

                }

                JsonDocument walkRoute = await getRoute(currentPosition, endPos, "foot-walking");


                if(getDuration(walkRoute) > bestItineraryTime)
                {
                    currentPosition = endItineraryStation.position;
                    finalBiking.Concat(bike);
                    finalPedestrian.Concat(pedestrian);
                }
                else
                {
                    currentPosition = endPos;
                    finalPedestrian.Add(walkRoute);
                }
                
            }

            return CreateBestRouteJson(finalPedestrian, finalBiking);
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


            return CreateBestRouteJson(pedestrian, bike);

        }

        public JsonDocument CreateBestRouteJson(List<JsonDocument> pedestrian, List<JsonDocument> bike)
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
                pedestrianPath = ToElements(pedestrian),
                bikePath = ToElements(bike)
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
