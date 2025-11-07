using GPS_Server.ProxyCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Server
{
    class GeoUtils
    {

        private const double EarthRadiusKm = 6371.0;

        public static double HaversineDistance(Position pos1, Position pos2)
        {
            // Conversion des degrés en radians
            double dLat = ToRadians(pos2.latitude - pos1.latitude);
            double dLon = ToRadians(pos2.longitude - pos1.longitude);

            double rLat1 = ToRadians(pos1.latitude);
            double rLat2 = ToRadians(pos2.latitude);

            // Formule de Haversine
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(rLat1) * Math.Cos(rLat2) * Math.Pow(Math.Sin(dLon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
