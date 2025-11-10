using GPS_Client.Utils;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPS_Client.Services
{
    internal class ItineraryService
    {
        public async Task SearchItineraryAsync(string departure, string arrival, List<string> steps)
        {
            Console.Clear();
            Console.WriteLine("Calcul de l'itinéraire en cours...\n");

            try
            {
                using var client = new GPSClient();

                var fullPath = new List<string> { departure };
                fullPath.AddRange(steps);
                fullPath.Add(arrival);

                for (int i = 0; i < fullPath.Count - 1; i++)
                {
                    var from = fullPath[i];
                    var to = fullPath[i + 1];

                    Console.WriteLine($"De {from} vers {to}");
                    var itinerary = await client.GetItineraryAsync(from, to);
                    Console.WriteLine(itinerary);
                    Console.WriteLine(new string('-', 40));
                }

                Console.WriteLine("\nItinéraire complet affiché.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'appel du service : {ex.Message}");
            }

            ConsoleHelper.Pause();
        }

        public async Task SearchItineraryParisMode(string departure, string arrival)
        {
            Console.Clear();
            Console.WriteLine("Calcul de l'itinéraire en cours...\n");

            try
            {
                using var client = new GPSClient();

                var fullPath = new List<string> { departure };
               
                Console.WriteLine($"De {departure} vers {arrival}");
                var itinerary = await client.ThrowBikeSeineAsync(departure);
                Console.WriteLine(itinerary);
                Console.WriteLine(new string('-', 40));
                itinerary = await client.GetItineraryAsync("Quai de la Seine 75019 Paris", arrival);
                Console.WriteLine(itinerary);
                Console.WriteLine(new string('-', 40));

                Console.WriteLine("\nItinéraire complet affiché.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'appel du service : {ex.Message}");
            }

            ConsoleHelper.Pause();
        }
    }
}
