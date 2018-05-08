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
            //mapper.Class<Ddd._CoreDomain.Translation>(x =>
            //{
            //    x.Id(id => id.Mnemonic, idMapper =>
            //    {
            //        idMapper.Column("mnemonic");
            //        idMapper.Generator(NHibernate.Mapping.ByCode.Generators.Assigned);
            //    });
            //});


            //mapper.Class<Ddd.moonlightDomain.BRHave>(x =>
            //{
            //    x.Id(id => id.Field, idMapper =>
            //    {
            //        idMapper.Column("field");
            //        idMapper.Generator(NHibernate.Mapping.ByCode.Generators.Assigned);
            //    });
            //});


            //mapper.Class<Ddd._CoreDomain.Setting>(x =>
            //{
            //    x.Id(id => id.Key, idMapper =>
            //    {
            //        idMapper.Column("key");
            //        idMapper.Generator(NHibernate.Mapping.ByCode.Generators.Assigned);
            //    });
            //});

            //mapper.Class<Ddd.NotificationDomain.Event>(x =>
            //{
            //    x.Bag<Ddd.NotificationDomain.EventField>("Fields", collectionMapping =>
            //    {
            //        collectionMapping.Key(keyMapping => keyMapping.Column(DbNames.Notification.EventField.EventEnum));
            //    });
            //});
        }

    }
}
        