using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AspNetCoreExample.Test")]

namespace AspNetCoreExample.Ddd.Mapper
{
    using System.Linq;

    using NHibernate.Cfg;


    public static class TheMapper
    {
        public static NHibernate.ISessionFactory BuildSessionFactory(
            string connectionString,
            bool useCache,
            bool useUnitTest = false)
        {
            var mapper = new PostgresNamingConventionAutomapper(useCache); //  NHibernate.Mapping.ByCode.ConventionModelMapper();


            var cfg = new NHibernate.Cfg.Configuration();


            cfg.DataBaseIntegration(c =>
            {
                // PostgreSQL                
                c.Driver<AspNetCoreExample.Infrastructure.NHibernateNpgsqlInfra.NpgsqlDriverExtended>();
                c.Dialect<AspNetCoreExample.Infrastructure.NHibernateInfra.PostgresDialectExtension>();

                c.ConnectionString = connectionString;

                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;

#if DEBUG
                c.LogSqlInConsole = true;
                c.LogFormattedSql = true;
#endif

            });


            System.Collections.Generic.IEnumerable<System.Type> entities =
                typeof(AspNetCoreExample.Ddd.IdentityDomain.User)
                .Assembly.GetExportedTypes()
                .Where(x => !(x.IsAbstract && x.IsSealed)); // exclude static 



            NHibernate.Cfg.MappingSchema.HbmMapping mapping =
                mapper.CompileMappingFor(entities);

            // The class name Template exits on both namespace RichDomainModel.Document and RichDomainModel.Notification
            // Solution found on: 
            // http://stackoverflow.com/questions/1156281/nhibernate-duplicatemappingexception-when-two-classes-have-the-same-name-but-dif
            mapping.autoimport = false;

            cfg.AddMapping(mapping);




            // http://www.ienablemuch.com/2013/06/multilingual-and-caching-on-nhibernate.html
            //var filterDef = new NHibernate.Engine.FilterDefinition("lf", /*default condition*/ null,
            //                                           new Dictionary<string, NHibernate.Type.IType>
            //                                                           {
            //                                                               { "LanguageCultureCode", NHibernate.NHibernateUtil.String}
            //                                                           }, useManyToOne: false);
            //cfg.AddFilterDefinition(filterDef);



            if (useCache)
            {
                cfg.Cache(x =>
                {
                    // SysCache is not stable on unit testing
                    if (!useUnitTest)
                    {
                        x.Provider<NHibernate.Caches.CoreMemoryCache.CoreMemoryCacheProvider>();

                        // I don't know why SysCacheProvider is not stable on simultaneous unit testing, 
                        // might be SysCacheProvider is just giving one session factory, so simultaneous test see each other caches
                        // This solution doesn't work: http://stackoverflow.com/questions/700043/mstest-executing-all-my-tests-simultaneously-breaks-tests-what-to-do                    
                    }
                    else
                    {
                        x.Provider<NHibernate.Caches.CoreMemoryCache.CoreMemoryCacheProvider>();
                    }


                    // http://stackoverflow.com/questions/2365234/how-does-query-caching-improves-performance-in-nhibernate

                    // Need to be explicitly turned on so the .Cacheable directive on Linq will work:                    
                    x.UseQueryCache = true;
                });
            }


            ////// temporarily set to true so we can see the SQL when we are in web, when we are not in unit test.
            //if (true || useUnitTest)
            //{
            //    cfg.SetInterceptor(new NHSQLInterceptor());
            //}


            cfg.LinqToHqlGeneratorsRegistry<
               AspNetCoreExample.Infrastructure.NHibernateInfra.PostgresLinqToHqlGeneratorsRegistry
            >();

            var sf = cfg.BuildSessionFactory();


            return sf;
        }


    }//class Mapper


}


