namespace MoveMate.Repository.Repositories.Dtos;

public class RoleUserCount
{
    public string RoleName { get; set; }
    public int UserCount { get; set; }
    
    public int TotalActiveUsers { get; set; }
    public int TotalNoActiveUsers { get; set; }

}
