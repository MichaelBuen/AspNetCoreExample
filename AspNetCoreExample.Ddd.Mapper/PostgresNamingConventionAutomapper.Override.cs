namespace AspNetCoreExample.Ddd.Mapper
{
    using NHibernate.Mapping.ByCode;

    partial class PostgresNamingConventionAutomapper
    {
        /// <summary>
        /// anything that doesn't fit in automapping conventions must be handled with custom mapping.
        /// do them here.
        /// </summary>
        /// <param name="mapper"></param>
        static void OverrideMapping(ConventionModelMapper mapper)
        {
            //mapper.Class<Ddd._CoreDomain.Setting>(x =>
            //{
            //    x.Id(id => id.Key, idMapper =>
            //    {
            //        idMapper.Column("key");
            //        idMapper.Generator(NHibernate.Mapping.ByCode.Generators.Assigned);
            //    });
            //});                      
        }

    }
}
