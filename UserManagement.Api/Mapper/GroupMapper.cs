using Org.BouncyCastle.Asn1.X509;
using UserManagement.Api.Models;
using UserManagement.Core.Dto.Group;

namespace UserManagement.Api.Mapper;

public static class GroupMapper
{
    
    public static Group ToModel(this GroupDto dto)
    {
        return new Group()
        {
            Id = dto.Id,
            Name =dto.Name,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
    }

    public static GroupDto ToDto(this Group group)
    {
       return new GroupDto(
           group.Id,
           group.Name
       );
    }
    
}