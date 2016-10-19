using System.Linq;

namespace LinqExpander
{
	public static class AsExpandableExtension
	{
		public static IQueryable<T> AsExpandable<T>(this IQueryable<T> source)
		{

			if (source is ExpandableQuery<T>)
			{
				return (ExpandableQuery<T>)source;
			}

			return new ExtendableQueryProvider(source.Provider).CreateQuery<T>(source.Expression);
		}
	}
}
