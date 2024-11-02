namespace MoveMate.Service.ViewModels.ModelResponses;

public class BookingValuationResponse
{
    public double Total { get; set; }
    public double Deposit { get; set; }

    public virtual List<BookingDetailsResponse> BookingDetails { get; set; } = new List<BookingDetailsResponse>();
    public virtual List<VoucherResponse> Vouchers { get; set; } = new List<VoucherResponse>();
    public virtual ICollection<FeeDetailResponse> FeeDetails { get; set; } = new List<FeeDetailResponse>();
}