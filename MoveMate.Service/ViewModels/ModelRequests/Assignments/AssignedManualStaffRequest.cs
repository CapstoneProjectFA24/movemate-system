using System.ComponentModel.DataAnnotations;

namespace MoveMate.Service.ViewModels.ModelRequests.Assignments;

public class AssignedManualStaffRequest
{
    [Required]
    public string StaffType { get; set; }

    public int? FailedAssignmentId { get; set; }

    public int? AssignToUserId { get; set; }
    public int? NeedReplaceAssignmentId { get; set; }
}