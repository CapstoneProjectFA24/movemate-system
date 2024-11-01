using System.ComponentModel.DataAnnotations;

namespace MoveMate.Service.ViewModels.ModelRequests;

public class BookingValuationRequest
{
    [Required(ErrorMessage = "Filed is required")]
    public string? PickupAddress { get; set; }

    [Required(ErrorMessage = "Filed is required")]
    public string? PickupPoint { get; set; }

    [Required(ErrorMessage = "Filed is required")]
    public string? DeliveryAddress { get; set; }

    [Required(ErrorMessage = "Filed is required")]
    public string? DeliveryPoint { get; set; }

    [Required(ErrorMessage = "Filed is required")]
    public string? EstimatedDistance { get; set; }

    [Required(ErrorMessage = "Filed is required")]
    public int HouseTypeId { get; set; }

    [Required(ErrorMessage = "Filed is required")]
    public int TruckCategoryId { get; set; }

    public string? FloorsNumber { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    [Required(ErrorMessage = "Filed is required")]
    public DateTime? BookingAt { get; set; }

    public bool? IsRoundTrip { get; set; } = false;

    public List<BookingDetailRequest> BookingDetails { get; set; } = new List<BookingDetailRequest>();
    public List<AddVoucherRequest> Vouchers { get; set; } = new List<AddVoucherRequest>();

    public bool IsValid()
    {
        return AreVouchersUnique();
    }

    public bool AreVouchersUnique()
    {

        var promotionIds = Vouchers.Select(v => v.PromotionCategoryId).ToList();
        return promotionIds.Distinct().Count() == promotionIds.Count;
    }
}