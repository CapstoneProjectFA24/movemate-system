namespace MoveMate.Repository.Repositories.Dtos;

public class CalculateStatisticUserDto
{
    public string Shard { get; set; }
    public int TotalUsers { get; set; }
    public int TotalBannedUsers { get; set; }
    public int TotalActiveUsers { get; set; }
    public int TotalNoActiveUsers { get; set; }

    public List<RoleUserCount> UsersByRole { get; set; } 
}