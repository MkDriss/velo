using System;
using System.Threading.Tasks;
using ServiceReference1;

namespace GPS_Client
{
    class Program
    {

        

        private static string MAIN_MENU_TITLE = "#######################################\n" +
                                                "#####     Bienvenu sur Velo !     #####\n" +
                                                "#######################################\n\n\n";

        private static string MAIN_MENU_CHOICES = "Modes :\n" +
                                                  "1 - Classic\n" +
                                                  "2 - Parisiens\n" +
                                                  "Selectionnez un mode de recherche (1-2): ";

        private static string CLASSIC_SEARCH_MENU_TITLE = "#######################################\n" +
                                                          "#####     Recherche Classique     #####\n" +
                                                          "#######################################\n\n\n";

        private static string CLASSIC_SEARCH_MENU_CHOICES = "1 - Renseigner l'adresse de départ\n" +
                                                     "2 - Renseigner l'adresse de départ\n" +
                                                     "3 - Retour\n" +
                                                     "Que voulez-vous faire ? (1-3): ";

        static Task Main(string[] args)
        {
            while (true)
            {
                mainMenu();
            }
        }

        private static void mainMenu()
        {
            Console.WriteLine(MAIN_MENU_TITLE);

            int choice = 0;

            while (choice != 1 && choice != 2) 
            {
                Console.WriteLine(MAIN_MENU_CHOICES);
                choice = Console.Read();
                if(choice != 1 && choice != 2)
                {
                    Console.WriteLine("Erreur - Choix Invalide");
                }
            }

            switch (choice)
            {
                case 1:
                    Console.WriteLine("Mode Classic sélectionné.\n");
                    classicSearch();
                    break;
                case 2:
                    Console.WriteLine("Mode Parisien sélectionné.\n");
                    break;
                default:
                    Console.WriteLine("Une erreur est survenue");
                    break;
            }
        }

        private static void classicSearch()
        {
            string departure = "EMPTY_INPUT";
            string arrival = "EMPTY_INPUT";
            int choice = -1;

            Console.WriteLine(CLASSIC_SEARCH_MENU_TITLE);

            while(choice != 1 && choice != 2 && choice != 3)
            {
                Console.WriteLine(CLASSIC_SEARCH_MENU_CHOICES);
                choice = Console.Read();
                if (choice != 1 && choice != 2 && choice != 3)
                {
                    Console.WriteLine("Erreur - Choix Invalide");
                }

                switch (choice)
                {
                    case 1:
                        departure = setPoint("départ");
                        break;
                    case 2:
                        arrival = setPoint("arrivée");
                        break;
                    case 3:
                        return;
                    default:
                        break;
                }

                if (departure != "EMPTY_INPUT" && arrival != "EMPTY_INPUT")
                {
                    search_itinerary(departure, arrival);
                }
            }         

            Console.WriteLine("\nAppuyez sur Entrée pour quitter...");
            Console.ReadLine();
        }

        private static string setPoint(string type) 
        {
            Console.WriteLine("Veuillez entrer une adresse de " + type + ": ");
            string location = Console.ReadLine();
            return location;
        }

        private static async void search_itinerary(string departure, string arrival)
        {

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
        }
    }
}
