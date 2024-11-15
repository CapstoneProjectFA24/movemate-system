namespace MoveMate.Service.ViewModels.ModelResponses.Assignments;

public class AssignManualDriverResponse
{
    public int? BookingNeedDrivers { get; set; }
    public List<AssignmentResponse>? AssignmentManualDrivers { get; set; } = new List<AssignmentResponse>();
    
    public List<UserResponse>? OtherDrivers { get; set; } = new List<UserResponse>();

    public bool? IsSussed { get; set; } = false;
}