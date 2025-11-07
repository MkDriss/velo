using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy
{
    [DataContract]
    public class OrsResponse
    {
        

        [DataMember] public string value;
        public OrsResponse() {}
        public OrsResponse(string data) { 
            OrsContextDto context = JsonSerializer.Deserialize<OrsContextDto>(data);
            requestOrsResponse(context).Wait();
        }

        async Task requestOrsResponse(OrsContextDto context)
        {

            string url = $"https://api.openrouteservice.org/v2/directions/{context.Profile}/geojson";

            var body = new
            {
                coordinates = new[]
                {
                    new[] { context.StartPosition.Longitude, context.StartPosition.Latitude},
                    new[] { context.EndPosition.Longitude, context.EndPosition.Latitude }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"[ORS] - POST vers {url}");

            HttpClient _httpClient = new HttpClient();

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

            HttpResponseMessage response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync();


            response.EnsureSuccessStatusCode();

            value = await response.Content.ReadAsStringAsync();
        }
    }
}
