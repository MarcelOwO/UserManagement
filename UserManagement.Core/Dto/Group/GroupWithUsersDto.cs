using UserManagement.Core.Dto.User;

namespace UserManagement.Core.Dto.Group;

public record GroupWithUsersDto(
    Guid Id,
    string Name,
    List<UserDto> Users
  );

