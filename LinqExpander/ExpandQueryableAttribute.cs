using System;

namespace LinqExpander
{

	/// <summary>
	/// Used in conjunction with the .AsExpandable() extention on a queryable this will take the extension method
	/// tagged and evaluate the queryable as an expression, appending it to the actual query.
	/// </summary>
	public class ExpandQueryableAttribute : Attribute { }
}
