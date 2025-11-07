using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GPS_Server.ProxyCache;

namespace GPS_Server
{
	// REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
	public class GPS : IGPS
	{
        public string GetItinerary(string address1, string address2)
        {

            AddCorsHeaders();

            Console.WriteLine($"[GPS] - Calcul d'itinérarire entre {address1} et {address2}");

            Address start = new Address(address1);
            Address end = new Address(address2);

            
            string startContract = JCDecauxUtils.getContract(start.City);
            string endContract = JCDecauxUtils.getContract(end.City);

            // Si les deux villes sont dans le même contract
            if (startContract == endContract && startContract != null)
            {
                Console.WriteLine("[GPS] - Meme contract");
                Stations stations = JCDecauxUtils.getStations(startContract);

                int precisionStation = 2;
                Console.WriteLine("[GPS] - Start Finding ClosestStations");
                List<Station> startClosestStation = findClosestStations(stations, start.position, precisionStation, true);
                List<Station> endClosestStation = findClosestStations(stations, end.position, precisionStation, false);

                Console.WriteLine("[GPS] - End Finding ClosestStations");

                ORSUtils ORS = new ORSUtils();

                Console.WriteLine("[GPS] - Compute Itinerary");

                return ORS.computeBestItinerary(startClosestStation, endClosestStation, start.position, end.position).RootElement.GetRawText();

            }

            else
            {
                Console.WriteLine("[GPS] - Contract différent");
                Stations stations = JCDecauxUtils.getAllStations();

                ORSUtils ORS = new ORSUtils();

                return ORS.computeComplexItinerary(stations.stations.ToList(), start.position, end.position).GetAwaiter().GetResult().RootElement.GetRawText();

            }
        }

        private List<Station> findClosestStations(Stations stList, Position position, int numberStation,bool isStart)
        {
            // Vérification basique
            if (stList == null || stList.stations == null || stList.stations.Length == 0) return new List<Station>();

            // On crée une liste (station, distance)
            var stationsWithDistance = new List<(Station station, double distance)>();

            foreach (Station s in stList.stations)
            {
                double distance = GeoUtils.HaversineDistance(position, s.position);

                if (isStart) 
                {
                    if (s.available_bikes > 0) stationsWithDistance.Add((s, distance)); 
                }
                else
                {
                    if (s.available_bikes_stands > 0) stationsWithDistance.Add((s, distance));
                }
            }

            // On trie par distance croissante et on prend les N premières
            var closestStations = stationsWithDistance
                .OrderBy(sd => sd.distance)
                .Take(numberStation)
                .Select(sd => sd.station)
                .ToList();

            return closestStations;
        }


        private void AddCorsHeaders()
        {
            var response = WebOperationContext.Current.OutgoingResponse;
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "*");
            response.Headers.Add("Access-Control-Allow-Headers", "*");
        }

    }
}
