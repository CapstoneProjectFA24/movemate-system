namespace MoveMate.Service.ViewModels.ModelRequests;

public class ServiceDetailRequest
{
    public int Id { get; set; }
    public bool? IsQuantity { get; set; }
    public int? Quantity { get; set; }
}