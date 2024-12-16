namespace MoveMate.Repository.Repositories.Dtos;

/// <summary>
/// A DTO to represent service statistics.
/// </summary>
public class ServiceStatistics
{
    public int TotalServices { get; set; }
    public int TotalActivedServices { get; set; }
    public int TotalNoActivedServices { get; set; }
    public int ParentServices { get; set; }
    public int ChildServices { get; set; }
}