namespace UserManagement.Core.Dto;

public record CreateUserDto()
{
    public string Email { get; set; }
    public string Name { get; set; }
}