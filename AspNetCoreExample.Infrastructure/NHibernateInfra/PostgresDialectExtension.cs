namespace AspNetCoreExample.Infrastructure.NHibernateInfra
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using NHibernate;
    using NHibernate.Dialect;
    using NHibernate.Dialect.Function;
    using NHibernate.Hql.Ast;
    using NHibernate.Linq.Functions;
    using NHibernate.Linq.Visitors;


    // Thanks to this: http://www.primordialcode.com/blog/post/nhibernate-customize-linq-provider-user-defined-sql-functions/

    public class PostgresDialectExtension : PostgreSQL83Dialect
    {
        public PostgresDialectExtension()
        {
            base.RegisterFunction("jsonb_extract_path_text",
                new StandardSQLFunction("jsonb_extract_path_text", NHibernateUtil.String));

            base.RegisterFunction("localize",
                new StandardSQLFunction("localize", NHibernateUtil.String));
        }
    }


    public class CustomJsonbValueExtractor : BaseHqlGeneratorForMethod
    {
        public CustomJsonbValueExtractor()
        {
            SupportedMethods =
                new[]
                {
                    NHibernate.Util.ReflectHelper.GetMethodDefinition(() =>
                        JsonbExtension.GetStringValue(Jsonb.Null, null)
                    )
                };
        }

        public override HqlTreeNode BuildHql(
            MethodInfo method, System.Linq.Expressions.Expression targetObject,
            ReadOnlyCollection<System.Linq.Expressions.Expression> arguments,
            HqlTreeBuilder treeBuilder,
            IHqlExpressionVisitor visitor)
        {
            IEnumerable<HqlExpression> args = arguments.Select(a => visitor.Visit(a)).Cast<HqlExpression>();

            return treeBuilder.MethodCall("jsonb_extract_path_text", args);
        }
    }


    public class CustomJsonbTranslationExtractor : BaseHqlGeneratorForMethod
    {
        public CustomJsonbTranslationExtractor()
        {
            SupportedMethods =
                new[]
                {
                    NHibernate.Util.ReflectHelper.GetMethodDefinition(() =>
                        JsonbExtension.Localize(Jsonb.Null, null, null)
                    )
                };
        }

        public override HqlTreeNode BuildHql(
            MethodInfo method, System.Linq.Expressions.Expression targetObject,
            ReadOnlyCollection<System.Linq.Expressions.Expression> arguments, 
            HqlTreeBuilder treeBuilder, 
            IHqlExpressionVisitor visitor)
        {
            IEnumerable<HqlExpression> args = arguments.Select(a => visitor.Visit(a)).Cast<HqlExpression>();

            return treeBuilder.MethodCall("localize", args);
        }
    }


    public class PostgresLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public PostgresLinqToHqlGeneratorsRegistry()
        {
            this.Merge(new CustomJsonbValueExtractor());
            this.Merge(new CustomJsonbTranslationExtractor());
        }
    }
}


/*
    Sample use of the Postgresql integration above:
*/


//public static IDictionary<string, string> UIText(IDomainAccess da, string lang)
//{
//    //var rs = da.QueryToDto<UITextDto>(
//    //    " select t.mnemonic as \"Mnemonic\", coalesce(t.translated->>@lang, t.fallback) as \"Text\" " + 
//    //    " from core.translation t", new { lang });


//    // We can now use the Jsonb method GetStringValue(which resolves to postgres's jsonb_extract_path_text)
//    // directly in Linq expression.
//    // For example, this Linq expression:
//    /*
//        var rs = from r in da.Query<CoreDomain.Translation>()                                         
//                 let Text = r.Translated.GetStringValue(lang) ?? r.Fallback
//                 orderby Text
//                 select new UITextDto { Text = Text, Mnemonic = r.Mnemonic };
//    */
//    // Resolves to:
//    /*
//    select 
//        coalesce(jsonb_extract_path_text(translatio0_.translated, ?), translatio0_.fallback) as col_0_0_, 
//        translatio0_.mnemonic as col_1_0_ 
//    from core.translation translatio0_ 
//    order by coalesce(jsonb_extract_path_text(translatio0_.translated, ?), translatio0_.fallback) asc
//    */
//    // We can now use as much as Linq as possible and avoids not refactor-friendly query string.
//    // We will only use query string when the query is not possible on Linq.

//    // Jsonb's GetStringValue is working on memory object too, that is this query:
//    /*
//        var rs = from r in da.Query<CoreDomain.Translation>().ToList()
//                 let Text = r.Translated.GetStringValue(lang) ?? r.Fallback
//                 orderby Text
//                 select new UITextDto { Text = Text, Mnemonic = r.Mnemonic };
//    */
//    // Resolves to:
//    /*
//        select 
//            translatio0_.mnemonic as mnemonic51_, 
//            translatio0_.fallback as fallback51_, 
//            translatio0_.translated as translated51_ 
//        from core.translation translatio0_
//    */


//    // TODO: add index on expression:
//    //      coalesce(jsonb_extract_path_text(translatio0_.translated, ?), translatio0_.fallback) as col_0_0_, 
//    // So when the coalesce expression is used on ORDER BY, we can speed up the sorting.

//    var rs = from r in da.Query<CoreDomain.Translation>().ToList()
//             let Text = r.Translated.GetStringValue(lang) ?? r.Fallback
//             orderby Text
//             select new UITextDto { Text = Text, Mnemonic = r.Mnemonic };

//    var dc = new Dictionary<string, string>();

//    foreach (var r in rs)
//    {
//        dc.Add(r.Mnemonic, r.Text);
//    }

//    return dc;
//}
