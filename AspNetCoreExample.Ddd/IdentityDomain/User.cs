namespace AspNetCoreExample.Ddd.IdentityDomain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser<int>
    {
        /// <summary>
        ///  One-to-many to external logins
        /// </summary>
        /// <value>The external logins.</value>
        public IEnumerable<ExternalLogin> ExternalLogins { get; protected set; } = new Collection<ExternalLogin>();

        /// <summary>
        /// Many-to-many between Users and Roles
        /// </summary>
        /// <value>The roles.</value>
        public IEnumerable<Role> Roles { get; protected set; } = new Collection<Role>();


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

            this.ExternalLogins.AsCollection().Add(el);
        }


        public async Task RemoveExternalLoginAsync(string loginProvider, string providerKey)
        {
            var externalLogin =
                await this.ExternalLogins.AsQueryable()
                .SingleOrDefaultAsyncOk(el => el.LoginProvider == loginProvider && el.ProviderKey == providerKey);

            if (externalLogin != null)
            {
                this.ExternalLogins.AsCollection().Remove(externalLogin);
            }
        }

        public async Task<IList<string>> GetRoleNamesAsync() =>
            await this.Roles.AsQueryable().Select(r => r.Name).ToListAsyncOk();

        public async Task AddRole(Role roleToAdd)
        {
            var isExisting = await this.Roles.AsQueryable().AnyAsyncOk(role => role == roleToAdd);

            if (!isExisting)
            {
                this.Roles.AsCollection().Add(roleToAdd);
            }
        }

        public async Task RemoveRoleAsync(string roleName)
        {
            string normalizedRoleName = roleName.ToUpper();

            var role =
                await this.Roles.AsQueryable()
                .Where(el => el.NormalizedName == normalizedRoleName)
                .SingleOrDefaultAsyncOk();

            if (role != null)
            {
                this.Roles.AsCollection().Remove(role);
            }
        }

        public async Task<bool> IsInRole(string roleName) =>
            await this.Roles.AsQueryable()
            .AnyAsyncOk(role => role.NormalizedName == roleName.ToUpper());


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
            await users.SingleOrDefaultAsyncOk(au =>
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
            // The cache of a user's external logins gets trashed when another user updates his/her external logins.
            // Explore how to make collection caching more robust. Disable for the meantime.
            // .CacheableOk() 
            .ToListAsyncOk();
    }

    public class ExternalLogin
    {
		/// <summary>
        /// Many-to-one to a user
        /// </summary>
        /// <value>The user.</value>
        protected User User { get; set; }

        internal ExternalLogin(User applicationUser) => this.User = applicationUser;

        // Was:
        // public int Id { get; protected set; }

        // Below is better as we don't need to expose primary key of child entities
        // But the above could be useful if we want to directly update, delete 
        // based on Id, for performance concern. 
        protected int Id { get; set; }

        public string LoginProvider { get; internal protected set; } // provider: facebook, google, etc

        public string ProviderKey { get; internal protected set; } // user's id from facebook, google, etc

        public string DisplayName { get; internal protected set; } // seems same as provider               
    }
}
