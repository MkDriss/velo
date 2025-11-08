using System;
using System.Threading.Tasks;
using ServiceReference1;

namespace GPS_Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("###################################");
            Console.WriteLine("#####     Welcome on Velo     #####");
            Console.WriteLine("###################################\n\n\n");

            string departure = "EMPTY_INPUT";
            string arrival = "EMPTY_INPUT";

            while (string.IsNullOrWhiteSpace(departure) || string.IsNullOrWhiteSpace(arrival)
                   || departure == "EMPTY_INPUT" || arrival == "EMPTY_INPUT")
            {
                Console.Write("Choose a departure point: ");
                departure = Console.ReadLine() ?? "EMPTY_INPUT";

                Console.Write("Choose an arrival point: ");
                arrival = Console.ReadLine() ?? "EMPTY_INPUT";

                Console.WriteLine($"\nDeparture: {departure}");
                Console.WriteLine($"Arrival: {arrival}");
            }

            Console.WriteLine("\nRécupération de l'itinéraire en cours...\n");

            try
            {
                // Création du client SOAP généré par ta référence de service
                using (var client = new GPSClient())
                {
                    // Appel de la méthode asynchrone du service
                    var itinerary = await client.GetItineraryAsync(departure, arrival);

                    Console.WriteLine("===== ITINÉRAIRE =====");
                    Console.WriteLine(itinerary);
                    Console.WriteLine("=======================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'appel du service : {ex.Message}");
            }

            Console.WriteLine("\nAppuyez sur Entrée pour quitter...");
            Console.ReadLine();
        }
    }
}
