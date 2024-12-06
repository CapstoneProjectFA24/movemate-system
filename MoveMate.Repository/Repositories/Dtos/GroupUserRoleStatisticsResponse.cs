namespace MoveMate.Repository.Repositories.Dtos;

public class GroupUserRoleStatisticsResponse
{
    public int TotalGroups { get; set; }
    public List<GroupUserRoleStatisticDto> Groups { get; set; }
}

public class GroupUserRoleStatisticDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public int TotalUsers { get; set; }
    public List<RoleUserCount> UsersByRole { get; set; }
}