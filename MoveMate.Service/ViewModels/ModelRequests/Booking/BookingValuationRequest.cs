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
    public int TruckCategoryId {get; set;} 
    
    [Required(ErrorMessage = "Filed is required")]
    public int TruckNumber {get; set;} 
    
    public string? FloorsNumber {get; set;} 

    public List<ServiceDetailRequest> ServiceDetails {get; set;} = new List<ServiceDetailRequest>();

}