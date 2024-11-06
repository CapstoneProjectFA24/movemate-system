using MoveMate.Service.ThirdPartyService.GoongMap.Models;

namespace MoveMate.Service.ThirdPartyService.GoongMap;

public interface IGoogleMapsService
{
    Task<string> GetAddressFromLatLong(double latitude, double longitude);
    Task<GoogleMapDTO?> GetDistanceAndDuration(string origins, string destinations);
}