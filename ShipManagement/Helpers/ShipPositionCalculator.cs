namespace ShipManagement.Helpers
{
    public static class ShipPositionCalculator
    {
        private const double KnotsToKmPerHour = 1.852;
        private const double DegreesToRadians = Math.PI / 180.0;
        private const double RadiansToDegrees = 180.0 / Math.PI;
        private const double EarthRadiusKm = 6371.0;

        public static (decimal newLatitude, decimal newLongitude) CalculateNewPosition(
            decimal currentLatitude,
            decimal currentLongitude,
            decimal velocityKnots,
            int headingDegrees,
            double timeIntervalSeconds)
        {
            // Convert time to hours
            var timeHours = timeIntervalSeconds / 3600.0;

            // Calculate distance traveled in km
            var distanceKm = (double)velocityKnots * KnotsToKmPerHour * timeHours;

            // Convert to radians
            var latRad = (double)currentLatitude * DegreesToRadians;
            var lonRad = (double)currentLongitude * DegreesToRadians;
            var headingRad = headingDegrees * DegreesToRadians;

            // Calculate angular distance
            var angularDistance = distanceKm / EarthRadiusKm;

            // Calculate new latitude
            var newLatRad = Math.Asin(
                Math.Sin(latRad) * Math.Cos(angularDistance) +
                Math.Cos(latRad) * Math.Sin(angularDistance) * Math.Cos(headingRad)
            );

            // Calculate new longitude
            var newLonRad = lonRad + Math.Atan2(
                Math.Sin(headingRad) * Math.Sin(angularDistance) * Math.Cos(latRad),
                Math.Cos(angularDistance) - Math.Sin(latRad) * Math.Sin(newLatRad)
            );

            // Convert back to degrees
            var newLatitude = (decimal)(newLatRad * RadiansToDegrees);
            var newLongitude = (decimal)(newLonRad * RadiansToDegrees);

            // Normalize longitude to [-180, 180]
            while (newLongitude > 180) newLongitude -= 360;
            while (newLongitude < -180) newLongitude += 360;

            return (newLatitude, newLongitude);
        }

        public static double CalculateDistanceFromLastPosition(ShipPositionUpdate lastPosition, ShipPositionUpdate currentPosition)
        {
            return DistanceCalculator.CalculateDistanceKm(
                (double)lastPosition.Latitude,
                (double)lastPosition.Longitude,
                (double)currentPosition.Latitude,
                (double)currentPosition.Longitude);
        }
    }
}