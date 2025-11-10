using System;
using System.Collections.Generic;
using System.Linq;

namespace GPS_Client.Utils
{
    internal static class ConsoleHelper
    {
        public static string AskChoice(string prompt, IEnumerable<string> validChoices)
        {
            string? choice;

            do
            {
                Console.Write(prompt);
                choice = Console.ReadLine()?.Trim();

                if (!validChoices.Contains(choice))
                {
                    Console.WriteLine("Choix invalide, veuillez réessayer.\n");
                }

            } while (!validChoices.Contains(choice));

            return choice!;
        }

        public static string AskInput(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(input) ? "Inconnu" : input;
        }

        public static void Pause()
        {
            Console.WriteLine("\nAppuyez sur Entrée pour continuer...");
            Console.ReadLine();
        }
    }
}
