namespace MoveMate.Service.ViewModels.ModelResponses;

public class BookingValuationResponse
{
    public double Amount { get; set; }

    public virtual List<ServiceDetailsResponse> ServiceDetails { get; set; } = new List<ServiceDetailsResponse>();
    public virtual ICollection<FeeDetailResponse> FeeDetails { get; set; } = new List<FeeDetailResponse>();
}