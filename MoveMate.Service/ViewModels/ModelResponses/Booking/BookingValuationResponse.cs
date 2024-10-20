namespace MoveMate.Service.ViewModels.ModelResponses;

public class BookingValuationResponse
{
    public double Total { get; set; }
    public double Deposit { get; set; }

    public virtual List<ServiceDetailsResponse> ServiceDetails { get; set; } = new List<ServiceDetailsResponse>();
    public virtual ICollection<FeeDetailResponse> FeeDetails { get; set; } = new List<FeeDetailResponse>();
}