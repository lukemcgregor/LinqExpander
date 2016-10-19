using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqExpander
{
	internal class ExpandableVisitor : ExpressionVisitor
	{
		private readonly IQueryProvider _provider;
		private readonly Dictionary<ParameterExpression, Expression> _replacements = new Dictionary<ParameterExpression, Expression>();

		internal ExpandableVisitor(IQueryProvider provider)
		{
			_provider = provider;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			bool expandNode = node.Method.GetCustomAttributes(typeof(ExpandQueryableAttribute), false).Any();
			if (expandNode && node.Method.IsStatic)
			{
				object[] args = new object[node.Arguments.Count];
				args[0] = _provider.CreateQuery(node.Arguments[0]);

				for (int i = 1; i < node.Arguments.Count; i++)
				{
					Expression arg = node.Arguments[i];
					args[i] = (arg.NodeType == ExpressionType.Constant) ? ((ConstantExpression)arg).Value : arg;
				}
				return Visit(((IQueryable)node.Method.Invoke(null, args)).Expression);
			}
			var replaceNodeAttributes = node.Method.GetCustomAttributes(typeof(ReplaceWithExpressionAttribute), false).Cast<ReplaceWithExpressionAttribute>();
			if (replaceNodeAttributes.Any() && node.Method.IsStatic)
			{
				var replaceWith = node.Method.DeclaringType.GetMethod(replaceNodeAttributes.First().MethodName).Invoke(null, null);
				if (replaceWith is LambdaExpression)
				{
					RegisterReplacementParameters(node.Arguments.ToArray(), replaceWith as LambdaExpression);
					return Visit((replaceWith as LambdaExpression).Body);
				}
			}
			return base.VisitMethodCall(node);
		}
		protected override Expression VisitParameter(ParameterExpression node)
		{
			Expression replacement;
			if (_replacements.TryGetValue(node, out replacement))
				return Visit(replacement);
			return base.VisitParameter(node);
		}
		private void RegisterReplacementParameters(Expression[] parameterValues, LambdaExpression expressionToVisit)
		{
			if (parameterValues.Length != expressionToVisit.Parameters.Count)
				throw new ArgumentException(string.Format("The parameter values count ({0}) does not match the expression parameter count ({1})", parameterValues.Length, expressionToVisit.Parameters.Count));
			foreach (var x in expressionToVisit.Parameters.Select((p, idx) => new { Index = idx, Parameter = p }))
			{
				if (_replacements.ContainsKey(x.Parameter))
				{
					throw new Exception("Parameter already registered, this shouldn't happen.");
				}
				_replacements.Add(x.Parameter, parameterValues[x.Index]);
			}
		}
	}

}
