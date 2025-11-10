using GPS_Client.Services;
using GPS_Client.Utils;

namespace GPS_Client.Menus
{
    internal class ClassicSearchMenu
    {
        private const string TITLE = """
#######################################
#####     Recherche Classique     #####
#######################################

""";

        private const string CHOICES = """
1 - Renseigner l'adresse de départ
2 - Renseigner l'adresse d'arrivée
3 - Ajouter une étape
4 - Calculer l'itinéraire
5 - Retour
Que voulez-vous faire ? (1-5) : 
""";

        private string? _departure;
        private string? _arrival;
        private readonly List<string> _steps = new();

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(TITLE);

                Console.WriteLine($"Départ  : {_departure ?? "(non défini)"}");
                Console.WriteLine($"Arrivée : {_arrival ?? "(non définie)"}");
                Console.WriteLine($"Étapes  : {(_steps.Any() ? string.Join(" → ", _steps) : "(aucune)")}");
                Console.WriteLine();

                var choice = ConsoleHelper.AskChoice(CHOICES, new[] { "1", "2", "3", "4", "5" });

                switch (choice)
                {
                    case "1":
                        _departure = ConsoleHelper.AskInput("Entrez l'adresse de départ : ");
                        break;

                    case "2":
                        _arrival = ConsoleHelper.AskInput("Entrez l'adresse d'arrivée : ");
                        break;

                    case "3":
                        _steps.Add(ConsoleHelper.AskInput("Entrez une étape intermédiaire : "));
                        break;

                    case "4":
                        await CalculateItineraryAsync();
                        break;

                    case "5":
                        return;
                }
            }
        }

        private async Task CalculateItineraryAsync()
        {
            if (_departure is null || _arrival is null)
            {
                Console.WriteLine("\nVeuillez définir un point de départ et un point d'arrivée avant de continuer.");
                ConsoleHelper.Pause();
                return;
            }

            var service = new ItineraryService();
            await service.SearchItineraryAsync(_departure, _arrival, _steps);
        }
    }
}
