namespace AspNetCoreExample.Ddd.IdentityDomain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using NHibernate.Linq;

    public class User : IdentityUser<int>
    {

        /// <summary>
        ///  One-to-many external logins
        /// </summary>
        /// <value>The external logins.</value>
        public IEnumerable<ExternalLogin> ExternalLogins { get; protected set; }

        /// <summary>
        /// Many-to-many between Users and Roles
        /// </summary>
        /// <value>The roles.</value>
        public IEnumerable<Role> Roles { get; protected set; }

        public User(string userName) : base(userName) { }

        public User(string userName, string email)
        {
            this.UserName = userName;
            this.Email = email;
        }


        public void AddExternalLogin(string loginProvider, string providerKey, string providerDisplayName)
        {
            var el = new ExternalLogin(this)
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey,
                DisplayName = providerDisplayName
            };

            this.ExternalLogins.AsList().Add(el);
        }


        public async Task RemoveExternalLoginAsync(string loginProvider, string providerKey)
        {
            // It would be nice if deleting directly from List is possible.
            // According to NHibernate 5, we can now delete from IQueryable.
            // Looks like that deletion is for Aggregate roots only.
            // For child entities, it does not work:
            //      await au.ExternalLogins.AsQueryable()
            //      .Where(el => el.LoginProvider == loginProvider && el.ProviderKey == providerKey)
            //      .DeleteAsync(cancellationToken);



            var externalLogin =
                await this.ExternalLogins.AsQueryable()
                .Where(el => el.LoginProvider == loginProvider && el.ProviderKey == providerKey)
                .SingleOrDefaultAsync();

            if (externalLogin != null)
            {
                this.ExternalLogins.AsList().Remove(externalLogin);
            }
        }

        public async Task<IList<string>> GetRoleNamesAsync() =>
            await this.Roles.AsQueryable().Select(r => r.Name).ToListAsync();

        public async Task AddRole(Role roleToAdd)
        {
            var isExisting =
                await this.Roles.AsQueryable()
                          .Where(role => role == roleToAdd)
                          .AnyAsync();

            if (!isExisting)
            {
                this.Roles.AsList().Add(roleToAdd);
            }
        }

        public async Task RemoveRoleAsync(string roleName)
        {
            string normalizedRoleName = roleName.ToUpper();

            var role =
                await this.Roles.AsQueryable()
                .Where(el => el.NormalizedName == normalizedRoleName)
                .SingleOrDefaultAsync();

            if (role != null)
            {
                this.Roles.AsList().Remove(role);
            }
        }

        public async Task<bool> IsInRole(string roleName) =>
            await this.Roles.AsQueryable()
            .AnyAsync(role => role.NormalizedName == roleName.ToUpper());


        public void SetTwoFactorEnabled(bool enabled) => this.TwoFactorEnabled = enabled;

        public void SetNormalizedEmail(string normalizedEmail) => this.NormalizedEmail = normalizedEmail;

        public void SetEmailConfirmed(Boolean confirmed) => this.EmailConfirmed = confirmed;

        public void SetPhoneNumber(string phoneNumber) => this.PhoneNumber = phoneNumber;

        public void SetPhoneNumberConfirmed(Boolean confirmed) => this.PhoneNumberConfirmed = confirmed;

        public void SetPasswordHash(string passwordHash) => this.PasswordHash = passwordHash;

        public void SetEmail(string email) => this.Email = email;

        public void SetNormalizedUserName(string normalizedUserName) => this.NormalizedUserName = normalizedUserName;

        public void SetUserName(string userName) => this.UserName = userName;

        public void UpdateFromDetached(User user)
        {
            this.UserName = user.UserName;
            this.NormalizedUserName = user.NormalizedUserName;
            this.Email = user.Email;
            this.NormalizedEmail = user.NormalizedEmail;
            this.EmailConfirmed = user.EmailConfirmed;
            this.PasswordHash = user.PasswordHash;
            this.PhoneNumber = user.PhoneNumber;
            this.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            this.TwoFactorEnabled = user.TwoFactorEnabled;
        }


        public async static Task<User> FindByLoginAsync(
            IQueryable<User> users, string loginProvider, string providerKey
        ) =>
            await users.SingleOrDefaultAsync(au => 
                au.ExternalLogins.Any(el => el.LoginProvider == loginProvider && el.ProviderKey == providerKey)
            );

        public async Task<IList<UserLoginInfo>> GetUserLoginInfoListAsync() =>
            await this.ExternalLogins.AsQueryable()
            .Select(el =>
                new UserLoginInfo(
                    el.LoginProvider,
                    el.ProviderKey,
                    el.DisplayName
                )
            )
            .ToListAsync();
    }

    public class ExternalLogin
    {
        public ExternalLogin(User applicationUser)
        {
            this.User = applicationUser;
        }

        public int Id { get; protected set; }

        protected User User { get; set; }

        public string LoginProvider { get; internal protected set; } // provider: facebook, google, etc

        public string ProviderKey { get; internal protected set; } // user's id from facebook, google, etc

        public string DisplayName { get; internal protected set; } // seems same as provider
    }
}
