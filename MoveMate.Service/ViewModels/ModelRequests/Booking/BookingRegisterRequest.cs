using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MoveMate.Service.ViewModels.ModelRequests;

public class BookingRegisterRequest
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
    public string? Note { get; set; }

    public bool? IsRoundTrip { get; set; } = false;

    public bool? IsManyItems { get; set; } = false;
    
    [DisplayFormat(DataFormatString = "{0:N0}")]
    public string? RoomNumber { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:N0}")] 
    public string? FloorsNumber { get; set; }
    
    [Required(ErrorMessage = "Filed is required")]
    public List<ServiceDetailRequest> ServiceDetails {get; set;} = new List<ServiceDetailRequest>();
    
    [Required(ErrorMessage = "Filed is required")]
    public int TruckCategoryId {get; set;} 
    
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    [Required(ErrorMessage = "Filed is required")]
    public DateTime? BookingAt { get; set; } 
    
    [JsonIgnore]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [JsonIgnore]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public bool? IsCancel { get; set; } = false;
    [JsonIgnore]
    public bool? IsDeleted { get; set; } = false;
    [JsonIgnore]
    public bool? IsReported { get; set; } = false;
    [JsonIgnore]
    public bool? IsBonus { get; set; } = false;
    [JsonIgnore]
    public bool? IsDeposited { get; set; } = false;

    public bool? IsReviewOnline { get; set; } = true;

    public virtual ICollection<ResourceRequest> ResourceList { get; set; } = new List<ResourceRequest>();


    public bool IsBookingAtValid()
    {
        if (BookingAt.HasValue)
        {
            return BookingAt.Value >= DateTime.Now;
        }
        
        return false;
    }


}