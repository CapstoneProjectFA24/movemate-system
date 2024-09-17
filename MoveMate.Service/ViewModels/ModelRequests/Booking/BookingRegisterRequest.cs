using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public int? HouseTypeId { get; set; }
    
    [Required(ErrorMessage = "Filed is required")]
    public string? Note { get; set; }
    
    public bool? IsRoundTrip { get; set; }
    
    public bool? IsManyItems { get; set; }
    
    public string? RoomNumber { get; set; }

    public string? FloorsNumber { get; set; }
    
    [Required(ErrorMessage = "Filed is required")]
    public List<ServiceDetailRequest> ServiceDetails {get; set;} = new List<ServiceDetailRequest>();

}