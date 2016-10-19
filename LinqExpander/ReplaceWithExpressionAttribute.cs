using System;

namespace LinqExpander
{
	public class ReplaceWithExpressionAttribute : Attribute
	{
		public string MethodName { get; set; }
	}
}
