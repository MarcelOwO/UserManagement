namespace UserManagement.Core.Dto;

public record UserDto(
    Guid Id,
    string Email,
    string Name,
    List<Guid> Groups
    );