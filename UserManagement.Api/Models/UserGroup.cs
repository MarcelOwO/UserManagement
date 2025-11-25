namespace UserManagement.Api.Models;

public class UserGroup
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = default!;
}