namespace AspNetCoreExample.Ddd.Mapper
{
    using System;
    using System.Linq;
    using NHibernate.Mapping.ByCode;

    partial class PostgresNamingConventionAutomapper : NHibernate.Mapping.ByCode.ConventionModelMapper
    {
        public PostgresNamingConventionAutomapper()
        {
            var mapper = this;

            mapper.IsEntity(
                (t, declared) => t.Namespace.StartsWith("AspNetCoreExample.Ddd", System.StringComparison.InvariantCulture)
            );


            mapper.IsProperty((mi, isProp) =>
            {
                dynamic m = mi;
                Type pt = m.PropertyType; // why NHibernate is hiding PropertyType?
                if (!isProp && pt == typeof(Jsonb))
                    return true;

                return isProp;
            });

            mapper.BeforeMapClass += Mapper_BeforeMapClass;
            mapper.BeforeMapProperty += Mapper_BeforeMapProperty;
            mapper.AfterMapProperty += Mapper_AfterMapProperty;

            mapper.BeforeMapManyToOne += Mapper_BeforeMapManyToOne;
            mapper.BeforeMapManyToMany += Mapper_BeforeMapManyToMany;

            mapper.BeforeMapBag += Mapper_BeforeMapBag;
            mapper.BeforeMapSet += Mapper_BeforeMapSet;


            OverrideMapping(mapper);
            MapManyToMany(mapper);
        }



        static void Mapper_BeforeMapClass(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            System.Type type,
            NHibernate.Mapping.ByCode.IClassAttributesMapper classCustomizer
        )
        {

#if USECACHE
            classCustomizer.Cache(cacheMapping => cacheMapping.Usage(NHibernate.Mapping.ByCode.CacheUsage.ReadWrite));
#endif


            string fullName = type.FullName; // example: AspNetCoreExample.Ddd.IdentityDomain.User

            string[] fullNameSplit = fullName.Split('.');

            string schemaDomainName = fullNameSplit[2];
            string schemaName = schemaDomainName.Substring(0, schemaDomainName.Length - "Domain".Length).ToLowercaseNamingConvention().Replace("__", "_");

            string className = fullNameSplit[3];

            string tableName = className.ToLowercaseNamingConvention();


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
            NHibernate.Mapping.ByCode.IClassAttributesMapper classCustomizer,
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

                classCustomizer.Id(mi,
                    idMapper =>
                    {
                        idMapper.Column("id");

                        if (propertyType.PropertyType == typeof(int))
                        {
                            idMapper.Generator(
                                NHibernate.Mapping.ByCode.Generators.Sequence,
                                generatorMapping => generatorMapping.Params(new { 
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
            NHibernate.Mapping.ByCode.IClassAttributesMapper classCustomizer
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

                classCustomizer.Id(mi,
                    idMapper =>
                    {
                        idMapper.Column("enum");

                        idMapper.Generator(NHibernate.Mapping.ByCode.Generators.Assigned);
                    });
            }
        }



        static void Mapper_BeforeMapProperty(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath member,
            NHibernate.Mapping.ByCode.IPropertyMapper propertyCustomizer
        )
        {
            string postgresFriendlyName = member.ToColumnName().ToLowercaseNamingConvention();

            propertyCustomizer.Column(postgresFriendlyName);

        }

        static void Mapper_AfterMapProperty(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector, 
            NHibernate.Mapping.ByCode.PropertyPath member, 
            NHibernate.Mapping.ByCode.IPropertyMapper propertyCustomizer
        )
        {
            dynamic lm = member.LocalMember;
            System.Type st = lm.PropertyType; // why NHibernate is hiding PropertyType? thanks to dynamic keyword, nothing can hide from dynamic

            if (st == typeof(Jsonb))
            {
                propertyCustomizer.Type(typeof(AspNetCoreExample.Infrastructure.NHibernateInfra.JsonbType), parameters: null);
            }

        }

        static void Mapper_BeforeMapBag(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath member,
            NHibernate.Mapping.ByCode.IBagPropertiesMapper propertyCustomizer
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

            string parentEntity = member.LocalMember.DeclaringType.Name.ToLowercaseNamingConvention(); // this gets the person table. lowercase name in postgres.
            string foreignKey = parentEntity + "_id";
            propertyCustomizer.Key(keyMapping => keyMapping.Column(foreignKey));


            // http://www.ienablemuch.com/2014/10/inverse-cascade-variations-on-nhibernate.html
            // best persistence approach: Inverse+CascadeAll 
            propertyCustomizer.Inverse(true);
            propertyCustomizer.Cascade(NHibernate.Mapping.ByCode.Cascade.All | NHibernate.Mapping.ByCode.Cascade.DeleteOrphans);

            propertyCustomizer.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra);


#if USECACHE
            propertyCustomizer.Cache(cacheMapping => cacheMapping.Usage(NHibernate.Mapping.ByCode.CacheUsage.ReadWrite));
#endif
        }


        static void Mapper_BeforeMapSet(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath member,
            NHibernate.Mapping.ByCode.ISetPropertiesMapper propertyCustomizer
        )
        {

            string parentEntity = member.LocalMember.DeclaringType.Name.ToLowercaseNamingConvention(); // this gets the person table. lowercase name in postgres.
            string foreignKey = parentEntity + "_id";
            propertyCustomizer.Key(keyMapping => keyMapping.Column(foreignKey));

            // See advantage of Extra Lazy here: http://www.ienablemuch.com/2013/12/pragmatic-ddd.html
            propertyCustomizer.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra);

#if USECACHE
            propertyCustomizer.Cache(cacheMapping => cacheMapping.Usage(NHibernate.Mapping.ByCode.CacheUsage.ReadWrite));
#endif
        }




        static void Mapper_BeforeMapManyToOne(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath member,
            NHibernate.Mapping.ByCode.IManyToOneMapper propertyCustomizer
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

            // ProductCategory property name maps to ProductCategoryId column name



            // TODO: Improve this to fetch the actual primary key. 
            // This relies on the convention that most primary key(s) are the first property of the class.

            string suffix;


            Type modelType = member.LocalMember.GetPropertyOrFieldType().UnderlyingSystemType;


            var isPrimaryKeyPredetermined = modelType.IsDerivedFromIdentityCore();

            if (isPrimaryKeyPredetermined)
            {
                suffix = 
                    nameof(Microsoft.AspNetCore.Identity.IdentityUser.Id)
                    .ToLowercaseNamingConvention();
            }
            else
            {
                suffix = modelType.GetProperties().First().Name
                        .ToLowercaseNamingConvention();
            }


            /*
            Examples:
                Client:
                    Id
                BROperator
                    Enum
                BRHave
                    Field
            */

            string columnName = member.ToColumnName();

            string postgresFriendlyName = columnName.ToLowercaseNamingConvention();



            if (!(columnName == "CreatedBy" || columnName == "ModifiedBy"))
                postgresFriendlyName = postgresFriendlyName + "_" + suffix;


            propertyCustomizer.Column(postgresFriendlyName);
            propertyCustomizer.Lazy(NHibernate.Mapping.ByCode.LazyRelation.Proxy);
        }


        static void Mapper_BeforeMapManyToMany(
            NHibernate.Mapping.ByCode.IModelInspector modelInspector,
            NHibernate.Mapping.ByCode.PropertyPath member,
            NHibernate.Mapping.ByCode.IManyToManyMapper collectionRelationManyToManyCustomizer
        )
        {
            Type collectionModelType = member.CollectionElementType();

            string suffix;

            var isPrimaryKeyPredetermined = collectionModelType.IsDerivedFromIdentityCore();

            System.Diagnostics.Debug.WriteLine("is predetermined: " + isPrimaryKeyPredetermined);

            if (isPrimaryKeyPredetermined)
            {
                suffix = nameof(Microsoft.AspNetCore.Identity.IdentityUser.Id);
            }
            else
            {
                suffix = collectionModelType.GetProperties().First().Name
                        .ToLowercaseNamingConvention();
            }


            string childKeyName = member.CollectionElementType().Name.ToLowercaseNamingConvention() + "_" + suffix;

            collectionRelationManyToManyCustomizer.Column(childKeyName);
            collectionRelationManyToManyCustomizer.Lazy(NHibernate.Mapping.ByCode.LazyRelation.Proxy);
        }



    } // PostgresNamingConventionAutomapper


    static class StringHelper
    {
        internal static string ToLowercaseNamingConvention(this string s, bool toLowercase = true) =>
            toLowercase ?
                new System.Text.RegularExpressions.Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace)
                          .Replace(s, "_").ToLower()

            :
                s;

    }

    static class NHibernateHelper
    {
        // derived from: https://gist.github.com/NOtherDev/1569982
        internal static System.Type CollectionElementType(
            this NHibernate.Mapping.ByCode.PropertyPath member
        ) =>
          member.LocalMember.GetPropertyOrFieldType()
          .DetermineCollectionElementOrDictionaryValueType();



        // https://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        internal static bool IsOpenGenericAssignableFrom(this Type openGeneric, Type fromCheck)
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
        internal static bool IsDerivedFromIdentityCore(this Type modelType) =>
            typeof(Microsoft.AspNetCore.Identity.IdentityUser<>)
                .IsOpenGenericAssignableFrom(modelType.BaseType)
            || typeof(Microsoft.AspNetCore.Identity.IdentityRole<>)
                .IsOpenGenericAssignableFrom(modelType.BaseType);

    }

}