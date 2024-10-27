using System.Text.Json.Serialization;

namespace MoveMate.Service.ViewModels.ModelRequests;

public class ResourceRequest
{
    public string? Type { get; set; }

    public string? ResourceUrl { get; set; }

    public string? ResourceCode { get; set; }
    [JsonIgnore]
    public bool? IsDeleted { get; set; } = false;
}