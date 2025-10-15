using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy
{
    internal class Stations
    {
        private string name;
        private List<Station> stations;
        public Stations(string contract)
        {
            this.name = contract;
            stations = new List<Station>();
            requestStation(contract);
        }

        async Task requestStation(string contract)
        {
            HttpClient client = new HttpClient();

            try
            {
                using (HttpResponseMessage response = await client.GetAsync("https://api.jcdecaux.com/vls/v1/stations?contract="
                                                                                + contract
                                                                                + "&apiKey="
                                                                                + ApiKeys.JCDECAUX_API_KEY))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        foreach (var element in doc.RootElement.EnumerateArray())
                        {
                            // Récupération des champs nécessaires
                            int number = element.GetProperty("number").GetInt32();
                            string contractName = element.GetProperty("contract_name").GetString();
                            string stationName = element.GetProperty("name").GetString();

                            // Création d’un objet Station minimal
                            var station = new Station(number, contractName, stationName);

                            // Ajout à la liste
                            stations.Add(station);
                        }
                    }

                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}
