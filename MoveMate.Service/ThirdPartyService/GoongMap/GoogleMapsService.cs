using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoveMate.Service.ThirdPartyService.GoongMap.Models;

namespace MoveMate.Service.ThirdPartyService.GoongMap;

public class GoogleMapsService : IGoogleMapsService
{
    private readonly HttpClient _httpClient;
    private readonly string _goongApiKey;
    private readonly ILogger<GoogleMapsService> _logger;

    public GoogleMapsService(HttpClient httpClient, IConfiguration configuration, ILogger<GoogleMapsService> logger)
    {
        _httpClient = httpClient;
        _goongApiKey = configuration["GoongMaps:ApiKey"];
        _logger = logger;
    }

    public async Task<string> GetAddressFromLatLong(double latitude, double longitude)
    {
        try
        {
            var url = $"https://rsapi.goong.io/Geocode?latlng={latitude},{longitude}&api_key={_goongApiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred in GetAddressFromLatLong Service!");
            throw;
        }
    }

    public async Task<GoogleMapDTO?> GetDistanceAndDuration(string origins, string destinations)
    {
        try
        {
            //var goongMap = new GoongMapResponse();
            var url =
                $"https://rsapi.goong.io/DistanceMatrix?origins={origins}&destinations={destinations}&vehicle=car&api_key={_goongApiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using (var document = JsonDocument.Parse(json))
            {
                var rows = document.RootElement.GetProperty("rows").EnumerateArray().FirstOrDefault();

                var elements = rows.GetProperty("elements").EnumerateArray().FirstOrDefault();

                /*
                var distanceText = elements.GetProperty("distance").GetProperty("text").GetString();
                var durationText = elements.GetProperty("duration").GetProperty("text").GetString();*/

                var googleMapDto = new GoogleMapDTO()
                {
                    Distance = new DistanceInfo
                    {
                        Text = elements.GetProperty("distance").GetProperty("text").GetString(),
                        Value = elements.GetProperty("distance").GetProperty("value").GetInt32()
                    },
                    Duration = new DurationInfo
                    {
                        Text = elements.GetProperty("duration").GetProperty("text").GetString(),
                        Value = elements.GetProperty("duration").GetProperty("value").GetInt32()
                    },
                };
                return googleMapDto;
            }

           
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred in GetDistanceAndDuration Service!");
            throw;
        }
    }
}