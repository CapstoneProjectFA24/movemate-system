namespace MoveMate.Repository.Repositories.Dtos;

public class StatisticTruckCategoryResult
{
    public int TotalTruckCategories { get; set; }  
    public List<StatisticTruckCategorySummary> TruckCategories { get; set; } 
}