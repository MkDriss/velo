using GPS_Client.Menus;
using System.Threading.Tasks;

namespace GPS_Client
{
    internal class Program
    {
        private static async Task Main()
        {
            var mainMenu = new MainMenu();
            await mainMenu.RunAsync();
        }
    }
}
