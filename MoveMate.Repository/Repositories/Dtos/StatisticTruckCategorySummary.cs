namespace MoveMate.Repository.Repositories.Dtos;

public class StatisticTruckCategorySummary
{
    public int TruckCategoryId { get; set; }
    public string TruckCategoryName { get; set; }
    public int TotalTrucks { get; set; }
    public int TotalBookings { get; set; }
}