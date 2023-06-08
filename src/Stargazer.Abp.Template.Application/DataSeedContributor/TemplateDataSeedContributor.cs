using Stargazer.Abp.Account.Domain.Repository;
using Stargazer.Abp.Account.Domain.Role;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Stargazer.Abp.Template.Application.DataSeedContributor;

public class TemplateDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<PermissionData, Guid> _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentTenant _currentTenant;
    private readonly IGuidGenerator _guidGenerator;

    public TemplateDataSeedContributor(
        IGuidGenerator guidGenerator, ICurrentTenant currentTenant,
        IRepository<PermissionData, Guid> permissionRepository, IRoleRepository roleRepository)
    {
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            // var permissions = await _permissionRepository.GetListAsync();

            // foreach (var item in OfficeOnlinePermissions.DefaultPermissions())
            // {
            //     var data = permissions.FirstOrDefault(x => x.Permission == item.Permission);
            //     if (data == null)
            //     {
            //         data = new PermissionData(_guidGenerator.Create(), item.Name, item.Permission);
            //         await _permissionRepository.InsertAsync(data);
            //     }

            //     Thread.Sleep(200);
            //     foreach (var permission in item.Permissions)
            //     {
            //         var permissionChild = permissions.FirstOrDefault(x => x.Permission == permission.Permission);
            //         if (permissionChild == null)
            //         {
            //             permissionChild = new PermissionData(_guidGenerator.Create(), permission.Name,
            //                 permission.Permission, data.Id);
            //             await _permissionRepository.InsertAsync(permissionChild);
            //         }

            //         Thread.Sleep(200);
            //     }
            // }
            
            // var role = await _roleRepository.FindAsync(x => x.Name == "一般用户");
            // if (role == null)
            // {
            //     role = new RoleData(_guidGenerator.Create(), "一般用户", true, true, false);
            //     foreach (var item in OfficeOnlinePermissions.GetAll())
            //     {
            //         Thread.Sleep(200);
            //         role.Permissions.Add(new RolePermissionData(_guidGenerator.Create(), role.Id, item));
            //     }
            //     await _roleRepository.InsertAsync(role);
            // }
        }
    }
}