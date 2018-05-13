namespace AspNetCoreExample.Identity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    using AspNetCoreExample.Ddd.Connection;
    using AspNetCoreExample.Ddd.IdentityDomain;

    using NHibernate.Linq;

    public class UserStore :
        IUserStore<User>,
        IUserEmailStore<User>,
        IUserPhoneNumberStore<User>,
        IUserTwoFactorStore<User>,
        IUserPasswordStore<User>,
        IUserRoleStore<User>,
        IUserLoginStore<User>
    {
        IDatabaseFactory DbFactory { get; }

        public UserStore(IDatabaseFactory dbFactory) => this.DbFactory = dbFactory;

        async Task<IdentityResult> IUserStore<User>.CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = this.DbFactory.OpenDddForUpdate())
            {
                await ddd.PersistAsync(user);

                await ddd.CommitAsync();
            }

            return IdentityResult.Success;
        }

        async Task<IdentityResult> IUserStore<User>.DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = this.DbFactory.OpenDddForUpdate())
            {
                await ddd.DeleteAggregateAsync(user);

                await ddd.CommitAsync();
            }

            return IdentityResult.Success;
        }

        async Task<User> IUserStore<User>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = this.DbFactory.OpenDddForUpdate())
            {
                var user = await ddd.GetAsync<User>(int.Parse(userId));

                return user;
            }
        }

        async Task<User> IUserStore<User>.FindByNameAsync(
            string normalizedUserName, CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = this.DbFactory.OpenDdd())
            {
                var au =
                    await ddd.Query<User>()
                            .SingleOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName);

                return au;
            }
        }


        Task<string> IUserStore<User>.GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedUserName);


        Task<string> IUserStore<User>.GetUserIdAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.Id.ToString());

        Task<string> IUserStore<User>.GetUserNameAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.UserName);


        Task IUserStore<User>.SetNormalizedUserNameAsync(
            User user, string normalizedName, CancellationToken cancellationToken
        )
        {
            user.SetNormalizedUserName(normalizedName);
            return Task.FromResult(0);
        }

        Task IUserStore<User>.SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.SetUserName(userName);
            return Task.FromResult(0);
        }

        async Task<IdentityResult> IUserStore<User>.UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();


            using (var ddd = this.DbFactory.OpenDddForUpdate())
            {
                var au = await ddd.GetAsync<User>(user.Id);

                au.UpdateFromDetached(user);

                await ddd.CommitAsync();
            }

            return IdentityResult.Success;
        }

        Task IUserEmailStore<User>.SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.SetEmail(email);
            return Task.FromResult(0);
        }

        Task<string> IUserEmailStore<User>.GetEmailAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.Email);

        Task<bool> IUserEmailStore<User>.GetEmailConfirmedAsync(
            User user, CancellationToken cancellationToken
        ) => Task.FromResult(user.EmailConfirmed);

        Task IUserEmailStore<User>.SetEmailConfirmedAsync(
            User user, bool confirmed, CancellationToken cancellationToken
        )
        {
            user.SetEmailConfirmed(confirmed);
            return Task.FromResult(0);
        }

        async Task<User> IUserEmailStore<User>.FindByEmailAsync(
            string normalizedEmail, CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = this.DbFactory.OpenDdd())
            {
                var au = await ddd.Query<User>()
                            .SingleOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

                return au;
            }
        }

        Task<string> IUserEmailStore<User>.GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedEmail);

        Task IUserEmailStore<User>.SetNormalizedEmailAsync(
            User user, string normalizedEmail, CancellationToken cancellationToken
        )
        {
            user.SetNormalizedEmail(normalizedEmail);
            return Task.FromResult(0);
        }

        Task IUserPhoneNumberStore<User>.SetPhoneNumberAsync(
            User user, string phoneNumber, CancellationToken cancellationToken
        )
        {
            user.SetPhoneNumber(phoneNumber);
            return Task.FromResult(0);
        }

        Task<string> IUserPhoneNumberStore<User>.GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.PhoneNumber);


        Task<bool> IUserPhoneNumberStore<User>.GetPhoneNumberConfirmedAsync(
            User user, CancellationToken cancellationToken
        ) => Task.FromResult(user.PhoneNumberConfirmed);

        Task IUserPhoneNumberStore<User>.SetPhoneNumberConfirmedAsync(
            User user, bool confirmed, CancellationToken cancellationToken
        )
        {
            user.SetPhoneNumberConfirmed(confirmed);
            return Task.FromResult(0);
        }

        Task IUserTwoFactorStore<User>.SetTwoFactorEnabledAsync(
            User user, bool enabled, CancellationToken cancellationToken
        )
        {
            user.SetTwoFactorEnabled(enabled);
            return Task.FromResult(0);
        }

        Task<bool> IUserTwoFactorStore<User>.GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.TwoFactorEnabled);


        Task IUserPasswordStore<User>.SetPasswordHashAsync(
            User user, string passwordHash, CancellationToken cancellationToken
        )
        {
            user.SetPasswordHash(passwordHash);
            return Task.FromResult(0);
        }

        Task<string> IUserPasswordStore<User>.GetPasswordHashAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.PasswordHash);


        Task<bool> IUserPasswordStore<User>.HasPasswordAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.PasswordHash != null);

        async Task IUserRoleStore<User>.AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = this.DbFactory.OpenDddForUpdate())
            {
                var roleByName =
                    await ddd.Query<Role>()
                    .SingleOrDefaultAsync(role => role.Name == roleName);

                if (roleByName == null)
                {
                    roleByName = new Role(roleName);
                    ddd.Persist(roleByName);
                }

                var userGot = await ddd.GetAsync<User>(user.Id);

                await userGot.AddRole(roleByName);

                await ddd.CommitAsync();
            }
        }

        async Task IUserRoleStore<User>.RemoveFromRoleAsync(
            User user, string roleName, CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = this.DbFactory.OpenDddForUpdate())
            {
                var userLoaded = await ddd.GetAsync<User>(user.Id);

                await userLoaded.RemoveRoleAsync(roleName);

                await ddd.CommitAsync();
            }
        }

        async Task<IList<string>> IUserRoleStore<User>.GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = this.DbFactory.OpenDdd())
            {
                var userGot = await ddd.GetAsync<User>(user.Id);

                return await userGot.GetRoleNamesAsync();
            }
        }

        async Task<bool> IUserRoleStore<User>.IsInRoleAsync(
            User user, string roleName, CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var ddd = this.DbFactory.OpenDdd())
            {
                var userGot = await ddd.GetAsync<User>(user.Id);

                return await userGot.IsInRole(roleName);
            }
        }

        async Task<IList<User>> IUserRoleStore<User>.GetUsersInRoleAsync(
            string roleName, CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();


            using (var ddd = this.DbFactory.OpenDdd())
            {
                string normalizedRoleName = roleName.ToUpper();

                var usersList = await Role.GetUsersByRoleNameAsync(ddd.Query<User>(), normalizedRoleName);

                return usersList;
            }
        }

        async Task IUserLoginStore<User>.AddLoginAsync(
            User user, UserLoginInfo login, CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (login == null)
                throw new ArgumentNullException(nameof(login));



            using (var ddd = this.DbFactory.OpenDddForUpdate())
            {
                var au = await ddd.GetAsync<User>(user.Id);

                au.AddExternalLogin(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName);

                await ddd.CommitAsync();
            }


        }

        async Task<User> IUserLoginStore<User>.FindByLoginAsync(
            string loginProvider, string providerKey, CancellationToken cancellationToken
        )
        {
            using (var ddd = this.DbFactory.OpenDdd())
            {
                var user = await User.FindByLoginAsync(ddd.Query<User>(), loginProvider, providerKey);
                return user;
            }
        }

        async Task<IList<UserLoginInfo>> IUserLoginStore<User>.GetLoginsAsync(
            User user, CancellationToken cancellationToken
        )
        {
            using (var ddd = this.DbFactory.OpenDdd())
            {
                var au = await ddd.GetAsync<User>(user.Id);

                var list = await au.GetUserLoginInfoListAsync();

                return list;
            }
        }

        async Task IUserLoginStore<User>.RemoveLoginAsync(
            User user, string loginProvider, string providerKey, CancellationToken cancellationToken
        )
        {
            using (var ddd = this.DbFactory.OpenDddForUpdate())
            {
                var au = await ddd.GetAsync<User>(user.Id);

                await au.RemoveExternalLoginAsync(loginProvider, providerKey);

                await ddd.CommitAsync();
            }
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

    }
}
