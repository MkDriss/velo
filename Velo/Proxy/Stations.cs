using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy
{
    [DataContract]
    internal class Stations
    {

        [DataMember] public string name;
        [DataMember] public List<Station> stations;
        public Stations(string contract)
        {
            this.name = contract;
            stations = new List<Station>();
            requestStation(contract).Wait();
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
                            double longitude = element.GetProperty("position").GetProperty("lng").GetDouble();
                            double latitude= element.GetProperty("position").GetProperty("lat").GetDouble();
                            int available_bikes = element.GetProperty("available_bikes").GetInt32();
                            int available_bikes_stands = element.GetProperty("available_bike_stands").GetInt32();

                            // Création d’un objet Station minimal

                            Position position = new Position(longitude, latitude);

                            var station = new Station(number, contractName, stationName, position, available_bikes, available_bikes_stands);

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
