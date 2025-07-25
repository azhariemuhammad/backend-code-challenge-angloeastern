namespace ShipManagement.Helpers
{
    public static class DistanceCalculator
    {
        private const double EarthRadiusKm = 6371.0;

        public static double CalculateDistanceKm(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            // Convert degrees to radians
            double latitudeDifferenceRadians = ToRadians(latitude2 - latitude1);
            double longitudeDifferenceRadians = ToRadians(longitude2 - longitude1);

            latitude1 = ToRadians(latitude1);
            latitude2 = ToRadians(latitude2);

            double haversine = Math.Pow(Math.Sin(latitudeDifferenceRadians / 2), 2) +
                               Math.Cos(latitude1) * Math.Cos(latitude2) *
                               Math.Pow(Math.Sin(longitudeDifferenceRadians / 2), 2);

            double centralAngle = 2 * Math.Asin(Math.Sqrt(haversine));

            return EarthRadiusKm * centralAngle;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}