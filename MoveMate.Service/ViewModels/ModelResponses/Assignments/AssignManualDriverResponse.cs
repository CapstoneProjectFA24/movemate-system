namespace MoveMate.Service.ViewModels.ModelResponses.Assignments;

public class AssignManualDriverResponse
{
    public int? BookingNeedStaffs { get; set; }
    public List<AssignmentResponse>? AssignmentManualStaffs { get; set; } = new List<AssignmentResponse>();
    
    public List<UserResponse>? OtherStaffs { get; set; } = new List<UserResponse>();
    
    public String StaffType { get; set; }

    public bool? IsSuccessed { get; set; } = false;
}