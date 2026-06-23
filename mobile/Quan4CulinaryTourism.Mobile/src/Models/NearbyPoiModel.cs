namespace Quan4CulinaryTourism.Mobile.Models;

public class NearbyPoiResponse : PoiResponse
{
    public double DistanceMeters { get; set; }

    public string DistanceText =>
        DistanceMeters < 1000
            ? $"{Math.Round(DistanceMeters)} m"
            : $"{DistanceMeters / 1000d:0.0} km";
}
