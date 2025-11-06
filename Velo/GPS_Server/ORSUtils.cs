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

        public async Task<JsonDocument> computeBestItinerary(List<Station> startStations, List<Station> endStations, Position startPos, Position endPos)
        {

            double bestTime = double.MaxValue;
            List<JsonDocument> pedestrian = new List<JsonDocument>();
            List<JsonDocument> bike = new List<JsonDocument>();


            foreach (Station start in startStations)
            {
                foreach (Station end in endStations)
                {

                    // Temps à pieds du départ jusqu'a la station start
                    JsonDocument walkStartPath = await getRoute(startPos, endPos, "foot-walking");
                    JsonDocument bikePath = await getRoute(startPos, endPos, "cycling-regular");
                    JsonDocument walkEndPath = await getRoute(startPos, endPos, "foot-walking");

                    double totalTime = getDuration(walkStartPath) + getDuration(bikePath) + getDuration(walkEndPath);

                    if(totalTime < bestTime)
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
            return JsonDocument.Parse(jsonString);
        }



        private double getDuration(JsonDocument doc)
        {
           
          return doc.RootElement
                    .GetProperty("features")[0]
                    .GetProperty("properties")
                    .GetProperty("summary")
                    .GetProperty("duration")
                    .GetDouble();
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



        public double GetWalkingDistance(double lon1, double lat1, double lon2, double lat2)
        {
            string url = "https://api.openrouteservice.org/v2/directions/foot-walking";

            double[][] coordinates = new double[][]
            {
                new double[] { lon1, lat1 },
                new double[] { lon2, lat2 }
            };

            var requestBody = new { coordinates };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", ApiKeys.ORS_API_KEY);

                var content = new StringContent(JsonSerializer.Serialize(requestBody));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                // Appel async de manière synchrone (C# 7.3)
                var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                using (JsonDocument json = JsonDocument.Parse(responseBody))
                {
                    double distance = json.RootElement
                        .GetProperty("features")[0]
                        .GetProperty("properties")
                        .GetProperty("summary")
                        .GetProperty("distance")
                        .GetDouble();

                    return distance;
                }
            }
        }
    }
}
