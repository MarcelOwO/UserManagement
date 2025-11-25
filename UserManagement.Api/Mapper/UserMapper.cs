using UserManagement.Api.Models;
using UserManagement.Core.Dto.User;

namespace UserManagement.Api.Mapper;

public static class UserMapper
{

    public static User ToModel(this UserDto dto)
    {
        return new User()
        {
            Id = dto.Id,
            Email = dto.Email,
            Name = dto.Name,
        };
    }

    
}