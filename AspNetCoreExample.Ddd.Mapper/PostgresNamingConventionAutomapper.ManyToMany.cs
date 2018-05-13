
namespace AspNetCoreExample.Ddd.Mapper
{
    partial class PostgresNamingConventionAutomapper : NHibernate.Mapping.ByCode.ConventionModelMapper
    {
        // See Guideline on many-to-many mapping

        static void MapManyToMany(NHibernate.Mapping.ByCode.ConventionModelMapper mapper)
        {
            MapUserWithRoles(mapper);

            //MapRoleWithPermissions(mapper);                      
            //MapEventConfigurationWithRoles(mapper);
        }


        static void MapUserWithRoles(NHibernate.Mapping.ByCode.ConventionModelMapper mapper)
        {
            var junctionTable = "user_role";

            mapper.Class<IdentityDomain.User>(map =>
            {
                map.Set<IdentityDomain.Role>(entity => entity.Roles, collectionMapping =>
                {
                    collectionMapping.Schema("identity");
                    collectionMapping.Table(junctionTable);
                },
                mapping => mapping.ManyToMany());
            });

            mapper.Class<IdentityDomain.Role>(map =>
            {
                map.Set<IdentityDomain.User>(entity => entity.Users, collectionMapping =>
                {
                    collectionMapping.Schema("identity");
                    collectionMapping.Table(junctionTable);
                },
                mapping => mapping.ManyToMany());
            });
        }


    }//class PostgresNamingConventionAutomapper

}


