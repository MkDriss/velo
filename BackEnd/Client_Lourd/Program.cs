using System;
using System.ServiceModel;

namespace GPS_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("###################################");
            Console.WriteLine("#####     Welcome on Velo     #####");
            Console.WriteLine("###################################\n\n\n");


            string departure = "EMPTY_INPUT";
            string arrival = "EMPTY_INPUT";
            
            while((departure == "EMPTY_INPUT") && (arrival == "EMPTY_INPUT"))
            {
                // Récupération du départ
                Console.Write("Choose a departure point: ");
                departure = Console.ReadLine() ?? "EMPTY_INPUT";

                // Récupération de l'arrivée
                Console.Write("Choose an arrival point: ");
                arrival = Console.ReadLine() ?? "EMPTY_INPUT";

                Console.WriteLine($"\nDeparture: {departure}");
                Console.WriteLine($"Arrival: {arrival}");
            }
            

            // Ici, tu peux appeler ton service SOAP ou REST avec ces données
            // Par exemple, si tu as une méthode GPS.GetRoute(departure, arrival)
            // tu peux l'utiliser pour récupérer l'itinéraire
        }
    }
}
