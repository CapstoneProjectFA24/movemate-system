using System.ComponentModel.DataAnnotations;

namespace MoveMate.Service.ViewModels.ModelRequests;

public class BookingCancelRequest
{
    [Required(ErrorMessage = "Filed is required")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Filed is required")]
    public string? CancelReason { get; set; }
}