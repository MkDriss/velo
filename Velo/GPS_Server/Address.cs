using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPS_Server
{
    class Address
    {
        public string City;        // Exemple : Marseille
        public int PostalCode;    // Exemple : 13000
        public string StreetAddress; // Exemple : 2 rue des lilas
        public double Latitude;
        public double Longitude;

        public Address(string addressInput)
        {

            Console.WriteLine("===== Creation d'une addresse =====");
            string[] parts = addressInput.Split(' ');

            // Le code postal est l’avant-dernier élément
            string codePostal = parts[parts.Length - 2];
            string ville = parts[parts.Length - 1];

            // L'adresse correspond au reste
            string adresse = string.Join(" ", parts.Take(parts.Length - 2));

            StreetAddress = adresse;
            City = ville;
            PostalCode = int.Parse(codePostal);

            Console.WriteLine($"Adresse : {StreetAddress}");
            Console.WriteLine($"Code postal : {PostalCode}");
            Console.WriteLine($"Ville : {City}");

            // Appel asynchrone bloquant ici pour simplifier
            Task.Run(async () => await FetchCoordinates(addressInput)).Wait();

            Console.WriteLine($"Latitude : {Latitude}");
            Console.WriteLine($"Longitude : {Longitude}\n");
        }

        private async Task FetchCoordinates(string address)
        {
            string encodedAddress = Uri.EscapeDataString(address);
            string url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json";

            using (HttpClient client = new HttpClient())

            {
                // Nominatim demande un User-Agent personnalisé
                client.DefaultRequestHeaders.Add("User-Agent", "GPS_ServerApp/1.0 (jean@example.com)");

                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();

                    var results = JsonDocument.Parse(json).RootElement;

                    if (results.GetArrayLength() == 0)
                    {
                        Console.WriteLine("Adresse non trouvée !");
                        return;
                    }

                    var first = results[0];
                    Latitude = double.Parse(first.GetProperty("lat").GetString(), CultureInfo.InvariantCulture); // ON le fait car en fr on a des , et en EN des .
                    Longitude = double.Parse(first.GetProperty("lon").GetString(), CultureInfo.InvariantCulture);
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la récupération des coordonnées : " + ex.Message);
                }
            }
        }
    }
}
