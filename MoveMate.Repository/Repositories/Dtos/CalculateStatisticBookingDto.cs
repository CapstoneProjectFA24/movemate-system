namespace MoveMate.Repository.Repositories.Dtos;

public class CalculateStatisticBookingDto
{
    public string Shard { get; set; }
    public int TotalBookings { get; set; }
    public int TotalInProcessBookings { get; set; }
    public int TotalCancelBookings { get; set; }
    public int? MostBookedHouseType { get; set; }
    public int? MostBookedTruck { get; set; }
    public int? MostBookedTime { get; set; }
    public string? MostBookedDayOfWeek { get; set; }
    public DateTime? MostBookedDate { get; set; }
    
}