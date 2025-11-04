using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace GPS_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = new Uri("http://localhost:8701/GPSServer/");
            using (var host = new WebServiceHost(typeof(GPS), baseAddress))
            {
                var endpoint = host.AddServiceEndpoint(typeof(IGPS), new WebHttpBinding(), "");
                endpoint.Behaviors.Add(new WebHttpBehavior());

                host.Open();
                Console.WriteLine("GPSServer running at " + baseAddress);
                Console.WriteLine("Press Enter to stop...");
                Console.ReadLine();
            }

        }
    }
}
