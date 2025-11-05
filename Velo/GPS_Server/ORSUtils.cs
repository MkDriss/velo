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
            _httpClient.DefaultRequestHeaders.Add("Authorization", ApiKeys.ORS_API_KEY);
        }

        public async Task<JsonDocument> computeBestItinerary(List<Station> startStations, List<Station> endStations, Position startPos, Position endPos)
        {
            double bestTime = double.MaxValue;
            List<JsonDocument> pedestrian = new List<JsonDocument>();
            List<JsonDocument> bike = new List<JsonDocument>();

            try
            {
                foreach (Station start in startStations)
                {
                    foreach (Station end in endStations)
                    {
                        // -> Correction : routes entre (startPos -> start), (start -> end), (end -> endPos)
                        JsonDocument walkStartPath = await getRoute(startPos, start.position, "foot-walking").ConfigureAwait(false);
                        JsonDocument bikePath = await getRoute(start.position, end.position, "cycling-regular").ConfigureAwait(false);
                        JsonDocument walkEndPath = await getRoute(end.position, endPos, "foot-walking").ConfigureAwait(false);

                        double totalTime = getDuration(walkStartPath) + getDuration(bikePath) + getDuration(walkEndPath);

                        if (totalTime < bestTime)
                        {
                            bestTime = totalTime;
                            pedestrian = new List<JsonDocument> { walkStartPath, walkEndPath };
                            bike = new List<JsonDocument> { bikePath };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ORS] - Erreur computeBestItinerary: {ex}");
                throw;
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
            Console.WriteLine($"[ORS] - Get route for : {url}");

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

            HttpResponseMessage response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonDocument.Parse(responseString);
        }
    }
}
