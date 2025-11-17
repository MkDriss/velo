using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy
{
    [DataContract]
    class Address
    {
        [DataMember]
        public string City;        // Exemple : Marseille

        [DataMember]
        public int PostalCode;    // Exemple : 13000

        [DataMember]
        public string StreetAddress; // Exemple : 2 rue des lilas

        [DataMember]
        public Position position;


        private static readonly HttpClient client = new HttpClient();

        public Address(string addressInput)
        {
            Console.WriteLine("===== Création d'une adresse =====");
            string[] parts = addressInput.Split(' ');

            string codePostal = parts[parts.Length - 2];
            string ville = parts[parts.Length - 1];
            string adresse = string.Join(" ", parts.Take(parts.Length - 2));

            StreetAddress = adresse;
            City = ville;
            PostalCode = int.Parse(codePostal);

            Console.WriteLine($"Adresse : {StreetAddress}");
            Console.WriteLine($"Code postal : {PostalCode}");
            Console.WriteLine($"Ville : {City}");

            // Récupération des coordonnées
            Task.Run(async () => await FetchCoordinatesORS(addressInput)).Wait();

            if (position != null)
            {
                Console.WriteLine($"Latitude : {position.latitude}");
                Console.WriteLine($"Longitude : {position.longitude}\n");
            }
            else
            {
                Console.WriteLine("Impossible de récupérer les coordonnées.\n");
            }
        }

        public Address() { }

        private async Task FetchCoordinatesORS(string address)
        {
            string encodedAddress = Uri.EscapeDataString(address);
            string url = $"https://api.openrouteservice.org/geocode/search?api_key={ApiKeys.ORS_API_KEY}&text={encodedAddress}";

            try
            {
                // ORS gère les quotas mais on peut rester prudent
                await Task.Delay(100); // petit délai entre appels si multiples

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    if (root.GetProperty("features").GetArrayLength() == 0)
                    {
                        Console.WriteLine("Adresse non trouvée !");
                        return;
                    }

                    var firstFeature = root.GetProperty("features")[0];
                    var coords = firstFeature.GetProperty("geometry").GetProperty("coordinates");

                    double lon = coords[0].GetDouble();
                    double lat = coords[1].GetDouble();

                    this.position = new Position
                    {
                        latitude = lat,
                        longitude = lon
                    };
                }

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Erreur HTTP ORS : " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la récupération des coordonnées : " + ex.Message);
            }
        }
    }
}
