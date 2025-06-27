using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using GrpcContracts;
using GrpcContracts.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data;

public static class IQueryableExtensions
{
public static IQueryable<T> Include<T>(this IQueryable<T> queryable, Expression<Func<T, object>> expr)
{
	var member = expr.Body as MemberExpression;
	var name = member.Member.Name;

	var provider = queryable as LinqProvider<T>;
	if (provider == null)
	{
		var table = queryable as TableBase<T>;
		if (table == null) throw new Exception("Can't include: repo must be LinqProvider or TableBase");
		provider = new LinqProvider<T>(table);
	}

	provider = provider.Clone();
	provider.IncludeFields.Add(name);
	return provider;
}
}

public class LinqProvider<T> : IQueryProvider, IOrderedQueryable<T>
{
	public List<string> IncludeFields = new();

	private List<Expression> expressions = new();
	private TableBase<T> tableBase;
	public TableBase<T> TableBase => tableBase;

	public LinqProvider(TableBase<T> tableBase)
	{
		this.tableBase = tableBase;
	}

	public IQueryable CreateQuery(Expression expression)
	{
		var type = expression.Type;
		if (!type.IsGenericType)
			throw new ArgumentException($"Don't know how to handle non-generic type '{type}'");
		var genericType = type.GetGenericTypeDefinition();
		if (genericType == typeof(IQueryable<>) || genericType == typeof(IOrderedQueryable<>))
			type = type.GetGenericArguments()[0];
		else throw new ArgumentException($"Don't know how to handle type '{type}'");
		return null;
	}

	public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
	{
		var tb = tableBase as TableBase<TElement>;
		if (tb == null) throw new Exception("Change type to '" + typeof(TElement) + "' is not supported");
		var provider = new LinqProvider<TElement>(tb);
		provider.IncludeFields.AddRange(IncludeFields);
		provider.expressions.AddRange(expressions);
		provider.expressions.Add(expression);
		return provider;
	}

	public object Execute(Expression expression)
	{
		throw new NotImplementedException();
	}

	public TResult Execute<TResult>(Expression expression)
	{
		var returnType = typeof(TResult);
		expressions.Add(expression);
		var serializer = GetSerializer();
		expressions.Clear();

		if (returnType == typeof(IEnumerable<T>))
		{
			var objects = GetEnumerable(serializer);
			return (TResult)Convert.ChangeType(objects, returnType);
		}

		//First/FirstOrDefault support
		if (returnType == typeof(T))
		{
			var list = GetEnumerable(serializer).ToList();
			if (list.Count == 0)
			{
				if (serializer.IsDefaultNull) return default;
				throw new InvalidOperationException();
			}
			return (TResult)Convert.ChangeType(list[0], returnType);
		}

		throw new NotImplementedException(returnType.ToString());
	}

	private IEnumerable<T> GetEnumerable(ExpressionSerializer serializer)
	{
		var xmlRequest = serializer.GetXml();
		var client = Services.GetOrmClient();

		var request = new SelectRequest();
		request.Request = xmlRequest;
		var reply = tableBase.Select(request, client);

		return reply;
	}

	public Expression Expression => Expression.Constant(this);

	public Type ElementType => typeof(T);

	public IQueryProvider Provider => this;

	public IEnumerator<T> GetEnumerator()
	{
		var serializer = GetSerializer();
		var list = GetEnumerable(serializer);
		return list.GetEnumerator();
	}

	public ExpressionSerializer GetSerializer()
	{
		return new ExpressionSerializer(expressions, IncludeFields);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public LinqProvider<T> Clone()
	{
		var provider = new LinqProvider<T>(tableBase);
		provider.expressions.AddRange(expressions);
		provider.IncludeFields.AddRange(IncludeFields);
		return provider;
	}
}