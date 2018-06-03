namespace AspNetCoreExample.Ddd.Mapper
{
    partial class PostgresNamingConventionAutomapper : NHibernate.Mapping.ByCode.ConventionModelMapper
    {
        bool UseCache { get; }


        internal PostgresNamingConventionAutomapper(bool useCache)
        {
            this.UseCache = useCache;

            var mapper = this;

            mapper.IsEntity((t, declared) =>
            {
                var isEntity = t.Namespace.StartsWith("AspNetCoreExample.Ddd", System.StringComparison.InvariantCulture);

                System.Diagnostics.Debug.WriteLine("is entity " + t);
                System.Diagnostics.Debug.WriteLine(isEntity);


                return isEntity;
            });


            mapper.IsProperty((memberInfo, isProp) =>
            {
                dynamic m = memberInfo;
                System.Type pt = m.PropertyType; // why NHibernate is hiding PropertyType?
                if (!isProp && pt == typeof(Jsonb))
                    return true;

                return isProp;
            });

            mapper.IsOneToOne((memberInfo, isOneToOne) =>
            {
                var x = memberInfo.GetPropertyOrFieldType();

                System.Diagnostics.Debug.WriteLine("Hello ");
                System.Diagnostics.Debug.WriteLine(x);


                return isOneToOne;
            });

            mapper.BeforeMapClass += Mapper_BeforeMapClass;
            mapper.BeforeMapProperty += Mapper_BeforeMapProperty;
            mapper.AfterMapProperty += Mapper_AfterMapProperty;

            mapper.BeforeMapManyToOne += Mapper_BeforeMapManyToOne;
            mapper.BeforeMapManyToMany += Mapper_BeforeMapManyToMany;

            mapper.BeforeMapBag += Mapper_BeforeMapBag;
            mapper.BeforeMapSet += Mapper_BeforeMapSet;

            mapper.BeforeMapJoinedSubclass += Mapper_BeforeMapJoinedSubclass;


            MapManyToMany(mapper);            
            OverrideMapping(mapper);
        }



        void Mapper_BeforeMapClass(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            System.Type type,
            NHibernate.Mapping.ByCode.IClassAttributesMapper classCustomizer
        )
        {

            if (this.UseCache)
            {
                classCustomizer.Cache(cacheMapping => cacheMapping.Usage(NHibernate.Mapping.ByCode.CacheUsage.ReadWrite));
            }


            string fullName = type.FullName; // example: AspNetCoreExample.Ddd.IdentityDomain.ApplicationUser

            var (schemaName, tableName) = fullName.GetTableMapping();


            classCustomizer.Schema(schemaName);
            classCustomizer.Table(tableName);

            classCustomizer.Lazy(true);


            // If this needs to be turned on..
            classCustomizer.DynamicUpdate(true);
            // ..this needs to be turned on too.
            classCustomizer.SelectBeforeUpdate(true);
            // As not doing so, could cause inconsistent update: http://stackoverflow.com/questions/13954882/nhibernate-dynamic-update-disadvantages


            CustomizeIdPrimaryKey(type, classCustomizer, schemaName, tableName);
            CustomizeEnumPrimaryKey(type, classCustomizer);
        }

        static void CustomizeIdPrimaryKey(
            System.Type type,
            NHibernate.Mapping.ByCode.IClassAttributesMapper classAttributesMapper,
            string schemaName,
            string tableName
        )
        {
            var primaryKeyProperty = type.GetMember(
                "Id",
                System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance
            );


            if (primaryKeyProperty.Length == 1)
            {
                System.Reflection.MemberInfo mi = primaryKeyProperty[0];

                var propertyType = (System.Reflection.PropertyInfo)mi;

                classAttributesMapper.Id(mi,
                    idMapper =>
                    {
                        idMapper.Column("id");

                        if (propertyType.PropertyType == typeof(int))
                        {
                            idMapper.Generator(
                                NHibernate.Mapping.ByCode.Generators.Sequence,
                                generatorMapping => generatorMapping.Params(new
                                {
                                    sequence = schemaName + "." + tableName + "_id_seq"
                                })
                            );
                        }
                        else if (propertyType.PropertyType == typeof(System.Guid))
                        {
                            idMapper.Generator(NHibernate.Mapping.ByCode.Generators.Assigned);
                        }
                        else
                        {
                            throw new System.ArgumentException(
                                string.Format("Id with type of {0} is unusual. Class: {1}",
                                              propertyType.PropertyType, type)
                            );
                        }

                    });
            }
        }

        static void CustomizeEnumPrimaryKey(
            System.Type type,
            NHibernate.Mapping.ByCode.IClassAttributesMapper classAttributesMapper
        )
        {
            var primaryKeyProperty = type.GetMember(
                "Enum",
                System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance
            );

            if (primaryKeyProperty.Length == 1)
            {
                System.Reflection.MemberInfo mi = primaryKeyProperty[0];

                classAttributesMapper.Id(mi,
                    idMapper =>
                    {
                        idMapper.Column("enum");

                        idMapper.Generator(NHibernate.Mapping.ByCode.Generators.Assigned);
                    });
            }
        }



        static void Mapper_BeforeMapProperty(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath propertyPath,
            NHibernate.Mapping.ByCode.IPropertyMapper propertyMapper
        )
        {
            string postgresFriendlyName = propertyPath.ToColumnName().ToLowercaseNamingConvention();

            propertyMapper.Column(postgresFriendlyName);

            System.Type st = propertyPath.LocalMember.GetPropertyOrFieldType();

            // http://www.ienablemuch.com/2018/06/utc-all-things-with-nhibernate-datetime-postgres-timestamptz.html
            if (st == typeof(System.DateTime) || st == typeof(System.DateTime?))
            {
                propertyMapper.Type<Infrastructure.NHibernateInfra.CustomUtcType>();
            }

        }

        void Mapper_AfterMapProperty(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath propertyPath,
            NHibernate.Mapping.ByCode.IPropertyMapper propertyMapper
        )
        {
            dynamic lm = propertyPath.LocalMember;
            System.Type st = lm.PropertyType; // why NHibernate is hiding PropertyType? thanks to dynamic keyword, nothing can hide from dynamic

            if (st == typeof(Jsonb))
            {
                propertyMapper.Type(typeof(AspNetCoreExample.Infrastructure.NHibernateInfra.JsonbType), parameters: null);
            }

        }

        void Mapper_BeforeMapBag(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath propertyPath,
            NHibernate.Mapping.ByCode.IBagPropertiesMapper bagPropertiesCustomizer
        )
        {

            /*
             * class Person
             * {
             *      IList<Hobby> Hobbies
             * }
             * 
             * 
             */

            // this gets the person table. lowercase name in postgres.
            string parentEntity = propertyPath.LocalMember.DeclaringType.Name.ToLowercaseNamingConvention();
            string foreignKey = parentEntity + "_fk";
            bagPropertiesCustomizer.Key(keyMapping => keyMapping.Column(foreignKey));


            // http://www.ienablemuch.com/2014/10/inverse-cascade-variations-on-nhibernate.html
            // best persistence approach: Inverse+CascadeAll 
            bagPropertiesCustomizer.Inverse(true);
            bagPropertiesCustomizer.Cascade(NHibernate.Mapping.ByCode.Cascade.All | NHibernate.Mapping.ByCode.Cascade.DeleteOrphans);

            bagPropertiesCustomizer.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra);


            if (this.UseCache)
            {
                bagPropertiesCustomizer.Cache(
                    cacheMapping => cacheMapping.Usage(NHibernate.Mapping.ByCode.CacheUsage.ReadWrite)
                );
            }
        }


        void Mapper_BeforeMapSet(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath propertyPath,
            NHibernate.Mapping.ByCode.ISetPropertiesMapper setPropertiesCustomizer
        )
        {
            // this gets the person table. lowercase name in postgres.
            string parentEntity = propertyPath.LocalMember.DeclaringType.Name.ToLowercaseNamingConvention();
            string foreignKey = parentEntity + "_fk";
            setPropertiesCustomizer.Key(keyMapping => keyMapping.Column(foreignKey));

            // See advantage of Extra Lazy here: http://www.ienablemuch.com/2013/12/pragmatic-ddd.html
            setPropertiesCustomizer.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra);

            if (this.UseCache)
            {
                setPropertiesCustomizer.Cache(
                    cacheMapping => cacheMapping.Usage(NHibernate.Mapping.ByCode.CacheUsage.ReadWrite)
                );
            }
        }




        static void Mapper_BeforeMapManyToOne(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath propertyPath,
            NHibernate.Mapping.ByCode.IManyToOneMapper manyToOneMapper
        )
        {
            /*
             
                public class Product
                {
                    protected internal  int                             ProductId       { get; set; }

                    public              TheProduction.ProductCategory   ProductCategory { get; protected internal set; }
                    public              string                          ProductName     { get; protected internal set; }
                }
             
             */

            // ProductCategory property name maps to product_category_fk column name

            string columnName = propertyPath.ToColumnName();

            string postgresFriendlyName = columnName.ToLowercaseNamingConvention();



            if (!(columnName == "CreatedBy" || columnName == "ModifiedBy"))
                postgresFriendlyName = postgresFriendlyName + "_fk";


            manyToOneMapper.Column(postgresFriendlyName);
            // Looks like we need to use no-proxy, we might encounter ghost object
            // https://ayende.com/blog/4378/nhibernate-new-feature-no-proxy-associations
            manyToOneMapper.Lazy(NHibernate.Mapping.ByCode.LazyRelation.Proxy);
        }


        static void Mapper_BeforeMapManyToMany(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath propertyPath,
            NHibernate.Mapping.ByCode.IManyToManyMapper manyToManyMapper
        )
        {
            System.Type collectionModelType = propertyPath.CollectionElementType();

            string childKeyName =
                propertyPath.CollectionElementType().Name.ToLowercaseNamingConvention() + "_fk";

            manyToManyMapper.Column(childKeyName);
            manyToManyMapper.Lazy(NHibernate.Mapping.ByCode.LazyRelation.Proxy);
        }




        void Mapper_BeforeMapJoinedSubclass(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            System.Type type,
            NHibernate.Mapping.ByCode.IJoinedSubclassAttributesMapper joinedSubclassAttributesMapper)
        {
            // not working though
            joinedSubclassAttributesMapper.Lazy(true);

            /*              
                class Animal
                {
                }

                class Dog : Animal
                {
                }              
            */


            System.Type baseType = type.BaseType; // Animal

            var (schemaName, tableName) = type.FullName.GetTableMapping();

            joinedSubclassAttributesMapper.Schema(schemaName);
            joinedSubclassAttributesMapper.Table(tableName);

            joinedSubclassAttributesMapper.Key(k =>
            {
                // postgresFriendlyName would be lowercase animal
                string postgresFriendlyName = baseType.Name.ToLowercaseNamingConvention();

                k.Column(postgresFriendlyName + "_fk");
                k.Unique(true);
                k.NotNullable(true);
                // k.OnDelete(NHibernate.Mapping.ByCode.OnDeleteAction.Cascade);                               
            });




            System.Diagnostics.Debug.WriteLine("Before joined subclass" + type);
        }




    } // PostgresNamingConventionAutomapper


    static class StringHelper
    {
        // Based on:
        // https://www.codeproject.com/Articles/108996/Splitting-Pascal-Camel-Case-with-RegEx-Enhancement

        internal static string ToLowercaseNamingConvention(this string s, bool toLowercase = true) =>
            toLowercase ?
                System.Text.RegularExpressions.Regex.Replace(
                    s,
                    @"(?<!^)([A-Z][a-z]|(?<=[a-z])[^a-z]|(?<=[A-Z])[0-9_])",
                    "_$1",
                    System.Text.RegularExpressions.RegexOptions.Compiled).Trim().Replace("___", "__").ToLower()
                :
                    s;


        internal static (string schemaName, string tableName) GetTableMapping(this string classFullName)
        {
            // classFullName:
            // AspNetCoreExample.Ddd.IdentityDomain.User

            string[] fullNameSplit = classFullName.Split('.');


            // IdentityDomain
            string schemaDomainName = fullNameSplit[2];

            // identity
            string schemaName =
                schemaDomainName.Substring(0, schemaDomainName.Length - "Domain".Length)
                                .ToLowercaseNamingConvention();

            // User
            string className = fullNameSplit[3];

            // user
            string tableName = className.ToLowercaseNamingConvention();

            return (schemaName, tableName);
        }
    }
}


namespace AspNetCoreExample.Ddd.Mapper
{
    using NHibernate.Mapping.ByCode;

    static class NHibernateHelper
    {
        // derived from: https://gist.github.com/NOtherDev/1569982
        internal static System.Type CollectionElementType(
            this NHibernate.Mapping.ByCode.PropertyPath member
        ) =>
            member.LocalMember.GetPropertyOrFieldType()
            .DetermineCollectionElementOrDictionaryValueType();

        internal static System.Type GetPropertyOrFieldType(
            this System.Reflection.MemberInfo memberInfo
        ) => NHibernate.Mapping.ByCode.TypeExtensions.GetPropertyOrFieldType(memberInfo);

        // https://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        internal static bool IsOpenGenericAssignableFrom(this System.Type openGeneric, System.Type fromCheck)
        {
            while (fromCheck != null && fromCheck != typeof(object))
            {
                var cur = fromCheck.IsGenericType ? fromCheck.GetGenericTypeDefinition() : fromCheck;
                if (openGeneric == cur)
                {
                    return true;
                }
                fromCheck = fromCheck.BaseType;
            }
            return false;
        }
    }

    static class IdentityCoreHelper
    {
        internal static bool IsDerivedFromIdentityCore(this System.Type modelType) =>
            typeof(Microsoft.AspNetCore.Identity.IdentityUser<>)
                .IsOpenGenericAssignableFrom(modelType.BaseType)
            || typeof(Microsoft.AspNetCore.Identity.IdentityRole<>)
                .IsOpenGenericAssignableFrom(modelType.BaseType);

    }
}