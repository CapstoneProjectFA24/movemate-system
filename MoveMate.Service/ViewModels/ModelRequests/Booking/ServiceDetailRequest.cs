using System.Text.Json.Serialization;

namespace MoveMate.Service.ViewModels.ModelRequests;

public class ServiceDetailRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int? Quantity { get; set; }
}