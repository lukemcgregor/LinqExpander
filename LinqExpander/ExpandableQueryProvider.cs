using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqExpander
{
	internal class ExtendableQueryProvider : IQueryProvider
	{
		IQueryProvider _underlyingQueryProvider;

		private ExtendableQueryProvider()
		{
		}

		internal ExtendableQueryProvider(IQueryProvider underlyingQueryProvider)
		{
			_underlyingQueryProvider = underlyingQueryProvider;
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new ExpandableQuery<TElement>(this, expression);
		}

		public IQueryable CreateQuery(Expression expression)
		{
			Type elementType = expression.Type.GetElementType();
			try
			{
				return (IQueryable)Activator.CreateInstance(typeof(ExpandableQuery<>).MakeGenericType(elementType), new object[] { this, expression });
			}
			catch (System.Reflection.TargetInvocationException tie)
			{
				throw tie.InnerException;
			}
		}

		internal IEnumerable<T> ExecuteQuery<T>(Expression expression)
		{
			return _underlyingQueryProvider.CreateQuery<T>(Visit(expression)).AsEnumerable();
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return _underlyingQueryProvider.Execute<TResult>(Visit(expression));
		}

		public object Execute(Expression expression)
		{
			return _underlyingQueryProvider.Execute(Visit(expression));
		}

		private Expression Visit(Expression exp)
		{
			ExpandableVisitor vstr = new ExpandableVisitor(_underlyingQueryProvider);
			Expression visitedExp = vstr.Visit(exp);

			return visitedExp;
		}
	}
}
