using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GrpcContracts.Expressions;

public class ExpressionSerializer
{
	private static XmlSerializer serializer;

	private readonly List<Expression> expressions;
	private readonly List<string> include;

	public ExpressionSerializer(List<Expression> expressions, List<string> include)
	{
		this.expressions = expressions;
		this.include = include;
	}

	public static RequestExpression Deserialize(string request)
	{
		if (serializer == null) serializer = new XmlSerializer(typeof(RequestExpression));
		using var reader = new StringReader(request);
		return serializer.Deserialize(reader) as RequestExpression;
	}

	public bool IsDefaultNull { get; internal set; }

	public string GetXml()
	{
		if (serializer == null) serializer = new XmlSerializer(typeof(RequestExpression));

		var exp = GetRequestExpression();

		var settings = new XmlWriterSettings();
		settings.IndentChars = "\t";
		settings.Indent = true;
		settings.NewLineChars = "\n";
		settings.Encoding = Encoding.UTF8;

		using (var writer = new StringWriter())
		using (XmlWriter xw = XmlWriter.Create(writer, settings))
		{
			serializer.Serialize(xw, exp);
			return writer.ToString();
		}
	}

	public RequestExpression GetRequestExpression()
	{
		var exp = new RequestExpression();
		foreach (var e in expressions)
		{
			AddExpression(exp, e);
		}
		exp.Include.AddRange(include);
		return exp;
	}

	private void AddExpression(RequestExpression re, Expression e)
	{
		var method = e as MethodCallExpression;
		var name = method.Method.Name;
		if (name == "Where")
		{
			AddWhereCondition(re, method);
			return;
		}
		if (name == "OrderBy")
		{
			AddOrderBy(re, method);
			re.OrderBy = OrderBy.Ascending;
			return;
		}
		if (name == "OrderByDescending")
		{
			AddOrderBy(re, method);
			re.OrderBy = OrderBy.Descending;
			return;
		}

		if (name == "Skip")
		{
			re.Limit.Skip = GetIntArgument(method);
			return;
		}

		if (name == "Take")
		{
			re.Limit.Take = GetIntArgument(method);
			return;
		}
		throw new NotImplementedException("method " + name);

	}

	private int GetIntArgument(MethodCallExpression method)
	{
		var args = method.Arguments;
		if (!args.Any()) throw new Exception("no args");
		var arg = method.Arguments.Last();

		if (arg is ConstantExpression ce)
		{
			return (int)ce.Value;
		}
		if (arg is MemberExpression ex)
		{
			var objectMember = Expression.Convert(ex, typeof(object));
			var getterLambda = Expression.Lambda<Func<object>>(objectMember);
			var getter = getterLambda.Compile();
			var value = getter();
			return (int)value;
		}

		throw new Exception("must be member expression");
	}

	private void AddOrderBy(RequestExpression re, MethodCallExpression method)
	{
		var args = method.Arguments;

		if (!args.Any()) return;

		var arg = args.Last();

		if (!(arg is UnaryExpression lambda)) return;
		if (arg.NodeType != ExpressionType.Quote) return;

		if (lambda.Operand is not LambdaExpression predicate) return;
		if (predicate.Body is not MemberExpression ex) return;
		if (ex.Expression.NodeType != ExpressionType.Parameter) return;
		re.OrderByField = ex.Member.Name;
	}

	private void AddWhereCondition(RequestExpression re, MethodCallExpression method)
	{
		var args = method.Arguments;

		if (!args.Any()) return;

		var arg = args.Last();

		if (arg is not UnaryExpression lambda) return;
		if (arg.NodeType is not ExpressionType.Quote) return;
		if (lambda.Operand is not LambdaExpression lambdaExpression) return;

		var wc = new LambdaExpressionParser(lambdaExpression).GetWhereCondition();

		if (re.Condition == null)
		{
			re.Condition = wc;
			return;
		}

		re.Condition = new WhereCondition(re.Condition, WhereOperator.And, wc);
	}
}