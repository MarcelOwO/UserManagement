namespace UserManagement.Core.Dto;

public record UpdateUserDto(string Name, string Email, List<Guid> Groups);