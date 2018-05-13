namespace AspNetCoreExample.Ddd.IdentityDomain
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    
	using Microsoft.AspNetCore.Identity;

    public class Role : IdentityRole<int>
    {
        /// <summary>
        /// Many-to-many between Roles and Users
        /// </summary>
        /// <value>The users.</value>
        public IEnumerable<User> Users { get; protected set; } = new Collection<User>();


        public Role(string roleName) : base(roleName) { }


        public void UpdateFromDetached(Role role)
        {
            this.Name = role.Name;
            this.NormalizedName = role.NormalizedName;
        }

        public void SetRoleName(string roleName) => this.Name = roleName;

        public void SetNormalizedName(string normalizedName) => this.NormalizedName = normalizedName;

        public static async Task<IList<User>> GetUsersByRoleNameAsync(IQueryable<User> users, string normalizedRoleName)
        {
            var criteria =
                    from user in users
                    where user.Roles.AsQueryable().Any(role => role.NormalizedName == normalizedRoleName)
                    select user;

            return await criteria.ToListAsyncOk();
        }
    }
}
