using System;

using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;

using NHibernate.Linq;
using Remotion.Linq;

namespace AspNetCoreExample
{

    /// cross-cutting concerns here.    
    /// UnitTestFriendlyExtensionMethods detects if the IQueryable is owned by NHibernate or not.
    /// Can't use NHibernate's built-in .Cacheable on non-NHibernate IQueryable (e.g., coming from unit test), it will throw an error    
    /// Further read:
    /// http://www.ienablemuch.com/2011/08/how-to-make-your-controller-unit.html
    /// http://www.ienablemuch.com/2011/08/nhibernate-is-testable-too.html
    /// http://mycodinglife.blog.com/2013/06/10/fetch-good-boy-now-play-nice-with-my-unit-tests/



    // Code courtesy of: http://mycodinglife.blog.com/2013/06/10/fetch-good-boy-now-play-nice-with-my-unit-tests/
    /// <summary>
    /// Provides extension method wrappers for NHibernate methods 
    /// to allow consuming source code to avoid "using" NHibernate.
    /// </summary>
    public static class NHibernateExtensions
    {
        /// <summary>
        /// Eager-loads a projection of the specified queryable, 
        /// referencing a mapped child object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRel">The type of the rel.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static IFetchRequest<T, TRel> FetchOk<T, TRel>(
            this IQueryable<T> queryable,
            Expression<Func<T, TRel>> expression)
            =>
            queryable is QueryableBase<T> ?
                FetchHelper.Create(queryable.Fetch(expression))
            :
                FetchHelper.CreateNonNH<T, TRel>(queryable);


        /// <summary>
        /// Eager-loads a second-level projection of the specified queryable, 
        /// referencing a mapped child of the first eager-loaded child.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRel">The type of the rel.</typeparam>
        /// <typeparam name="TRel2">The type of the rel2.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static IFetchRequest<T, TRel2> ThenFetchOk<T, TRel, TRel2>(
            this IFetchRequest<T, TRel> queryable,
            Expression<Func<TRel, TRel2>> expression)
            =>
            queryable is QueryableFetchHelper<T, TRel> ?
                FetchHelper.CreateNonNH<T, TRel2>(queryable)
            :
                FetchHelper.Create(queryable.ThenFetch(expression));


        /// <summary>
        /// Eager-loads a projection of the specified queryable, 
        /// referencing a mapped child object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRel">The type of the rel.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static IFetchRequest<T, TRel> FetchManyOk<T, TRel>(
            this IQueryable<T> queryable,
            Expression<Func<T, IEnumerable<TRel>>> expression)
            =>
            queryable is QueryableBase<T> ?
                FetchHelper.Create(queryable.FetchMany(expression))
            :
                FetchHelper.CreateNonNH<T, TRel>(queryable);

        /// <summary>
        /// Eager-loads a second-level projection of the specified queryable, 
        /// referencing a mapped child of the first eager-loaded child.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRel">The type of the rel.</typeparam>
        /// <typeparam name="TRel2">The type of the rel2.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static IFetchRequest<T, TRel2> ThenFetchManyOk
            <T, TRel, TRel2>(
            this IFetchRequest<T, TRel> queryable,
            Expression<Func<TRel, IEnumerable<TRel2>>> expression)
            =>
            queryable is QueryableFetchHelper<T, TRel> ?
                FetchHelper.CreateNonNH<T, TRel2>(queryable)
            :
                FetchHelper.Create(queryable.ThenFetchMany(expression));

    } // class NHibernateExtensions

    /// <summary>
    /// Provides a wrapper for NHibernate's FetchRequest interface, 
    /// so libraries that run eager-loaded queries don't have to reference 
    /// NHibernate assemblies.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TFetch">The type of the fetch.</typeparam>
    public interface IFetchRequest<TQuery, TFetch> : INhFetchRequest<TQuery, TFetch> { }

    internal class NhFetchHelper<TQuery, TFetch> : IFetchRequest<TQuery, TFetch>
    {
        readonly INhFetchRequest<TQuery, TFetch> realFetchRequest;

        //this is the real deal for NHibernate queries
        internal NhFetchHelper(INhFetchRequest<TQuery, TFetch> realFetchRequest)
            => this.realFetchRequest = realFetchRequest;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can 
        /// be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TQuery> GetEnumerator() => realFetchRequest.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object 
        /// that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => realFetchRequest.GetEnumerator();

        /// <summary>
        /// Gets the expression tree that is associated with the instance of 
        /// <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is 
        /// associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public Expression Expression => realFetchRequest.Expression;

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression 
        /// tree associated with this instance of 
        /// <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the 
        /// element(s) that are returned when the expression tree associated 
        /// with this object is executed.
        /// </returns>
        public Type ElementType => realFetchRequest.ElementType;

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated 
        /// with this data source.
        /// </returns>
        public IQueryProvider Provider => realFetchRequest.Provider;
    }

    internal class QueryableFetchHelper<TQuery, TFetch> : IFetchRequest<TQuery, TFetch>
    {
        readonly IQueryable<TQuery> queryable;

        //for use against non-NH datastores
        internal QueryableFetchHelper(IQueryable<TQuery> queryable) => this.queryable = queryable;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that 
        /// can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TQuery> GetEnumerator() => queryable.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can 
        /// be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => queryable.GetEnumerator();

        /// <summary>
        /// Gets the expression tree that is associated with the instance of 
        /// <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is 
        /// associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public Expression Expression => queryable.Expression;

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression 
        /// tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> 
        /// is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the 
        /// element(s) that are returned when the expression tree associated 
        /// with this object is executed.
        /// </returns>
        public Type ElementType => queryable.ElementType;

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated 
        /// with this data source.
        /// </returns>
        public IQueryProvider Provider => queryable.Provider;
    }

    /// <summary>
    /// The static "front door" to FetchHelper, with generic factories allowing 
    /// generic type inference.
    /// </summary>
    internal static class FetchHelper
    {
        public static NhFetchHelper<TQuery, TFetch> Create<TQuery, TFetch>(INhFetchRequest<TQuery, TFetch> nhFetch)
            => new NhFetchHelper<TQuery, TFetch>(nhFetch);

        public static NhFetchHelper<TQuery, TFetch> Create<TQuery, TFetch>(IFetchRequest<TQuery, TFetch> nhFetch)
            => new NhFetchHelper<TQuery, TFetch>(nhFetch);

        public static IFetchRequest<TQuery, TRel> CreateNonNH<TQuery, TRel>(IQueryable<TQuery> queryable)
            => new QueryableFetchHelper<TQuery, TRel>(queryable);
    }
}