using UserManagement.Core.Dto.Group;

namespace UserManagement.Core.Dto.User;


public record UserWithGroupsDto(
    Guid Id,
    string Email,
    string Name,
    List<GroupDto> Groups
    );
