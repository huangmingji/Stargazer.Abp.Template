using AutoMapper;

namespace Stargazer.Abp.Template.Application
{
    public class ApplicationAutoMapperProfile : Profile
    {
        public ApplicationAutoMapperProfile()
        {
            CreateUserDataMappings();
        }
        
        private void CreateUserDataMappings()
        {
            // CreateMap<UserData, UserDto>();
            // CreateMap<UserRole, UserRoleDto>();
            // CreateMap<RoleData, RoleDto>();
            // CreateMap<RolePermissionData, RolePermissionDto>();
            // CreateMap<PermissionData, PermissionDto>();
        }
    }
}