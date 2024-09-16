using System.ComponentModel.DataAnnotations;

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
    
    [Required(ErrorMessage = "Filed is required")]
    public List<int?> ServiceDetailList {get; set;} = new List<int?>();
}