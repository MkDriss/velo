using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proxy
{
    internal class Contract
    {

        public string contractName;

        public Contract(string cityName) {
            requestContract(cityName).Wait();
        }



        async Task requestContract(string cityName)
        {
            HttpClient client = new HttpClient();

            try
            {
                using (HttpResponseMessage response = await client.GetAsync("https://api.jcdecaux.com/vls/v3/contracts"
                                                                                + "?apiKey=" 
                                                                                + ApiKeys.JCDECAUX_API_KEY))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        foreach (var element in doc.RootElement.EnumerateArray())
                        {
                            // Récupération des champs nécessaires
                            string cName = element.GetProperty("name").GetString();


                            if (element.TryGetProperty("cities", out JsonElement citiesElement) &&
                                citiesElement.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var city in citiesElement.EnumerateArray())
                                {
                                    if(city.GetString() == cityName)
                                    {
                                        this.contractName = cName;
                                    }
                                }
                            }                         
                            
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
