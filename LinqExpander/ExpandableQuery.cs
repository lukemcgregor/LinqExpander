using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqExpander
{
	internal class ExpandableQuery<T> : IQueryable<T>, IOrderedQueryable<T>
	{
		ExtendableQueryProvider _provider;
		Expression _expression;

		public ExpandableQuery(ExtendableQueryProvider provider, Expression expression)
		{
			_provider = provider;
			_expression = expression;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _provider.ExecuteQuery<T>(_expression).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Type ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		public Expression Expression
		{
			get
			{
				return _expression;
			}
		}

		public IQueryProvider Provider
		{
			get
			{
				return _provider;
			}
		}
	}
}
