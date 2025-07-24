namespace ShipManagement.Helpers
{
    public static class DistanceCalculator
    {
        private const double EarthRadiusKm = 6371.0;

        public static double CalculateDistanceKm(decimal latitude1, decimal longitude1, decimal latitude2, decimal longitude2)
        {
            double latitudeDifferenceRadians = ToRadians((double)(latitude2 - latitude1));
            double longitudeDifferenceRadians = ToRadians((double)(longitude2 - longitude1));

            double latitude1Radians = ToRadians((double)latitude1);
            double latitude2Radians = ToRadians((double)latitude2);

            double haversineOfLat = Math.Sin(latitudeDifferenceRadians / 2) * Math.Sin(latitudeDifferenceRadians / 2);
            double haversineOfLon = Math.Sin(longitudeDifferenceRadians / 2) * Math.Sin(longitudeDifferenceRadians / 2);

            double centralAngle = haversineOfLat + Math.Cos(latitude1Radians) * Math.Cos(latitude2Radians) * haversineOfLon;
            double centralAngleInRadians = 2 * Math.Atan2(Math.Sqrt(centralAngle), Math.Sqrt(1 - centralAngle));

            double distance = EarthRadiusKm * centralAngleInRadians;
            return distance;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}