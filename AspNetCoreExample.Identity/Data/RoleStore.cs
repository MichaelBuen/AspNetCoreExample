namespace AspNetCoreExample.Identity.Data
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;

    using AspNetCoreExample.Dal;
    using AspNetCoreExample.Ddd.IdentityDomain;

    using NHibernate.Linq;

    public class RoleStore : IRoleStore<Role>
    {
        IDddFactory _dddF;

        public RoleStore(IConfiguration configuration, IDddFactory daf)
        {
            _dddF = daf;
        }

        async Task<IdentityResult> IRoleStore<Role>.CreateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = _dddF.OpenDdd())
            {
                await ddd.PersistAsync(role);

                await ddd.CommitAsync();
            }

            return IdentityResult.Success;
        }

        async Task<IdentityResult> IRoleStore<Role>.UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = _dddF.OpenDdd())
            {
                var roleGot = await ddd.GetAsync<Role>(role.Id);

                roleGot.UpdateFromDetached(role);

                await ddd.CommitAsync();
            }



            return IdentityResult.Success;
        }

        async Task<IdentityResult> IRoleStore<Role>.DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = _dddF.OpenDddForChanges())
            {
                await ddd.DeleteAggregateAsync(role);

                await ddd.CommitAsync();
            }

            return IdentityResult.Success;
        }

        Task<string> IRoleStore<Role>.GetRoleIdAsync(Role role, CancellationToken cancellationToken) =>
            Task.FromResult(role.Id.ToString());


        Task<string> IRoleStore<Role>.GetRoleNameAsync(Role role, CancellationToken cancellationToken) =>
            Task.FromResult(role.Name);
        

        Task IRoleStore<Role>.SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.SetRoleName(roleName);
            return Task.FromResult(0);
        }

        Task<string> IRoleStore<Role>.GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken) =>
            Task.FromResult(role.NormalizedName);        

        Task IRoleStore<Role>.SetNormalizedRoleNameAsync(
            Role role, string normalizedName, CancellationToken cancellationToken
        )
        {
            role.SetNormalizedName(normalizedName);
            return Task.FromResult(0);
        }

        async Task<Role> IRoleStore<Role>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = _dddF.OpenDdd())
            {
                var role = await ddd.GetAsync<Role>(roleId);

                return role;
            }
        }

        async Task<Role> IRoleStore<Role>.FindByNameAsync(
            string normalizedRoleName, CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = _dddF.OpenDdd())
            {
                var role =
                    await ddd.Query<Role>()
                    .SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName);

                return role;
            }
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}
