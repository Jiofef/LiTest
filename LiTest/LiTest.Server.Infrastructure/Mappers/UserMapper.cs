using LiTest.Shared.Core.Community;
using Riok.Mapperly.Abstractions;

namespace LiTest.Server.Infrastructure.Mappers
{
    [Mapper]
    public partial class UserMapper
    {
        public partial UserDto EntityToDto(UserEntity entity);
        public partial UserEntity DtoToEntity(UserDto dto);
    }
}