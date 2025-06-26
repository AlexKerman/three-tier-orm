using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GrpcContracts.Expressions;

public class LambdaExpressionParser
{
	private readonly Expression expression;
	private string parameterName;

	public LambdaExpressionParser(LambdaExpression lambda)
	{
		expression = lambda.Body;
		parameterName = lambda.Parameters[0].Name;
	}

	public WhereCondition GetWhereCondition()
	{
		return GetWhereCondition(expression);
	}

	private WhereCondition GetWhereCondition(Expression expr)
	{
		if (expr is BinaryExpression bin)
		{
			var left = GetWhereCondition(bin.Left);
			var right = GetWhereCondition(bin.Right);

			if (bin.NodeType == ExpressionType.AndAlso) return new WhereCondition(left, WhereOperator.And, right);
			if (bin.NodeType == ExpressionType.OrElse) return new WhereCondition(left, WhereOperator.Or, right);
			if (bin.NodeType == ExpressionType.Equal) return new WhereCondition(left, WhereOperator.Equal, right);
			if (bin.NodeType == ExpressionType.GreaterThan)
				return new WhereCondition(left, WhereOperator.GreaterThan, right);
			if (bin.NodeType == ExpressionType.GreaterThanOrEqual)
				return new WhereCondition(left, WhereOperator.GreaterThanOrEqual, right);
			if (bin.NodeType == ExpressionType.LessThan)
				return new WhereCondition(left, WhereOperator.LessThan, right);
			if (bin.NodeType == ExpressionType.LessThanOrEqual)
				return new WhereCondition(left, WhereOperator.LessThanOrEqual, right);

			throw new NotImplementedException(bin.NodeType.ToString());
		}

		if (expr is UnaryExpression un)
		{
			if (un.NodeType == ExpressionType.Not)
			{
				return new WhereCondition(GetWhereCondition(un.Operand), WhereOperator.Not, null);
			}

			if (un.NodeType == ExpressionType.Convert)
				return GetWhereCondition(un.Operand);

			throw new NotImplementedException(un.NodeType.ToString());
		}

		if (expr is MemberExpression ex)
		{
			var memberNames = GetMemberChain(ex);

			if (memberNames.First() == parameterName)
			{
				memberNames.RemoveAt(0);
				var par = new WhereCondition()
				{
					Operator = WhereOperator.Parameter,
					Value = string.Join(".", memberNames)
				};

				var propInfo = ex.Member as PropertyInfo;

				if (propInfo?.PropertyType == typeof(bool))
				{
					par = new WhereCondition
					{
						Left = par,
						Right = new WhereCondition
						{
							Operator = WhereOperator.Value,
							Value = "1"
						},
						Operator = WhereOperator.Equal
					};
				}

				return par;
			}

			//https://stackoverflow.com/questions/2616638/access-the-value-of-a-member-expression
			var objectMember = Expression.Convert(ex, typeof(object));
			var getterLambda = Expression.Lambda<Func<object>>(objectMember);
			var getter = getterLambda.Compile();
			var value = getter();
			return new WhereCondition
			{
				Operator = WhereOperator.Value,
				Value = GetValue(value)
			};
		}

		if (expr is ConstantExpression c)
		{
			return new WhereCondition
			{
				Operator = WhereOperator.Value,
				Value = GetValue(c.Value)
			};
		}

		if (expr is MethodCallExpression call)
		{
			if (call.Method.Name == "Contains")
			{
				WhereCondition left;
				WhereCondition right;

				if (call.Arguments.Count == 1)
				{
					var exArg = call.Object;
					left = GetWhereCondition(exArg);
					right = GetWhereCondition(call.Arguments[0]);
					return new WhereCondition(left, WhereOperator.Contains, right);
				}

				if (call.Arguments.Count == 2)
				{
					left = GetWhereCondition(call.Arguments[0]);
					right = GetWhereCondition(call.Arguments[1]);
					return new WhereCondition(left, WhereOperator.Contains, right);
				}
			}

			throw new NotImplementedException(expr.ToString());
		}

		throw new NotImplementedException(expr.ToString());
	}

	private List<string> GetMemberChain(MemberExpression ex)
	{
		var list = new List<string>();
		if (ex.Expression is MemberExpression me) list = GetMemberChain(me);
		if (ex.Expression is ParameterExpression pe)
		{
			list.Add(pe.Name);
		}
		list.Add(ex.Member.Name);
		return list;
	}

	private string GetValue(object obj)
	{
		if (obj is string s) return "'" + s + "'";
		if (obj is int i) return i.ToString();
		if (obj is long l) return l.ToString();
		if (obj is Enum) return ((int)obj).ToString();
		if (obj is DateTime dt) return $"'{dt:G}'";
		if (obj is IEnumerable<int> en) return string.Join(", ", en);
		if (obj is decimal d) return d.ToString();
		if (obj is null) return "null";
		throw new NotImplementedException("Unsupported argument type " + obj.GetType());
	}
}