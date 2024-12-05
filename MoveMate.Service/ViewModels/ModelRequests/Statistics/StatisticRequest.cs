using MoveMate.Domain.Enums;

namespace MoveMate.Service.ViewModels.ModelRequests.Statistics;

public class StatisticRequest
{
    public string? Shard { get; set; }
    public string? Type { get; set; }
    public bool IsSummary { get; set; } = true; 
}