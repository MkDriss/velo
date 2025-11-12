using GPS_Client.Utils;

namespace GPS_Client.Menus
{
    internal class MainMenu
    {
        private const string MAIN_MENU_TITLE = """
#######################################
#####     Bienvenue sur Velo !     ####
#######################################

""";

        private const string MAIN_MENU_CHOICES = """
Modes :
1 - Classique
2 - Parisien
Sélectionnez un mode de recherche (1-2) : 
""";

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(MAIN_MENU_TITLE);

                var choice = ConsoleHelper.AskChoice(MAIN_MENU_CHOICES, new[] { "1", "2"});

                switch (choice)
                {
                    case "1":
                        var classicMenu = new ClassicSearchMenu();
                        await classicMenu.RunAsync();
                        break;

                    case "2":
                        var parisSearchMenu = new ParisSearchMenu();
                        await parisSearchMenu.RunAsync();
                        ConsoleHelper.Pause();
                        break;
                }
            }
        }
    }
}
