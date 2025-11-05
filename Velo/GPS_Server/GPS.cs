using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GPS_Server.ServiceReference1;
using Proxy;

namespace GPS_Server
{
	// REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
	public class GPS : IGPS
	{
        public double GetItinerary(string address1, string address2)
        {
            Console.WriteLine($"[GPS] - Calcul d'itinérarire entre {address1} et {address2}");

            Address start = new Address(address1);
            Address end = new Address(address2);

            string startContract = getContract(start.City);
            string endContract = getContract(end.City);

            if (startContract == endContract)
            {

                Console.WriteLine("AAAAAAAAAAAAAAAAa");
                Stations stations = getStations(startContract);

                Station startClosestStation = findClosestStation(stations, start.Longitude, start.Latitude);
                Station endClosestStation = findClosestStation(stations, end.Longitude, end.Latitude);

                Console.WriteLine($"start : {start.Longitude}  :  {start.Latitude}");
                Console.WriteLine($"startStation : {startClosestStation.position.longitude}  :  {startClosestStation.position.latitude}");
                Console.WriteLine("AAAAAAAAAAAAAAAAa");


            }

            return 1234.5;

        }



        private string getContract(string cityName)
        {
            using (var client = new ProxyCacheClient())
            {
                try
                {
                    string contract = client.GetContract(cityName);
                    if (contract == null)
                    {
                        Console.WriteLine($"[GPS] - Aucun contract n'a été trouvé pour la ville : {cityName}");
                    }
                    else
                    {
                        Console.WriteLine($"[GPS] - Contract trouvé pour la ville : {cityName}");
                    }
                    return contract;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GPS] - Erreur lors de l’appel ProxyCacheClient : {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    return null;
                }
            }
        }

        private Stations getStations(string contractName)
        {
            using (var client = new ProxyCacheClient())
            {

                try
                {
                    if (contractName != null)
                    {
                        Stations stations = client.GetStations(contractName);
                        Console.WriteLine($"[GPS] - Nombre de stations récupérées pour {contractName} : {stations.stations.Length}");
                        return stations;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GPS] - Erreur lors de l’appel ProxyCacheClient : {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    return null;    
                }
            }

        }


        public double GetWalkingDistance(double lon1, double lat1, double lon2, double lat2)
        {
            string url = "https://api.openrouteservice.org/v2/directions/foot-walking";

            double[][] coordinates = new double[][]
            {
                new double[] { lon1, lat1 },
                new double[] { lon2, lat2 }
            };

            var requestBody = new { coordinates };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", ApiKeys.ORS_API_KEY);

                var content = new StringContent(JsonSerializer.Serialize(requestBody));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                // Appel async de manière synchrone (C# 7.3)
                var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                using (JsonDocument json = JsonDocument.Parse(responseBody))
                {
                    double distance = json.RootElement
                        .GetProperty("features")[0]
                        .GetProperty("properties")
                        .GetProperty("summary")
                        .GetProperty("distance")
                        .GetDouble();

                    return distance;
                }
            }
        }

        private Station findClosestStation(Stations stList, double lon, double lat)
        {
            double minDist = Double.MaxValue;
            Station closestStation = null;

            foreach (Station s in stList.stations)
            {
                double walkingDistance = GetWalkingDistance(s.position.longitude, s.position.latitude, lon, lat);

                if(minDist > walkingDistance)
                {
                    minDist = walkingDistance;
                    closestStation = s;
                }
            }

            return closestStation;
        }

    }
}
