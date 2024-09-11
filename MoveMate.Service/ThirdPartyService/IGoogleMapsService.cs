namespace MoveMate.Service.ThirdPartyService;

public interface IGoogleMapsService
{
    Task<string> GetAddressFromLatLong(double latitude, double longitude);
    Task<bool> GetDistanceAndDuration(string origins, string destinations);
    
}