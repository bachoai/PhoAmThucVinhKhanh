namespace Quan4CulinaryTourism.Api.Helpers;

public class DistanceHelper
{
    public double CalculateDistanceMeters(double lat1, double lng1, double lat2, double lng2)
    {
        const double earthRadius = 6_371_000;
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLng = DegreesToRadians(lng2 - lng1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }

    private static double DegreesToRadians(double value) => value * Math.PI / 180d;
}
