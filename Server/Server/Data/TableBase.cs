using System.Collections;
using GrpcContracts.Expressions;
using System.Linq.Expressions;

namespace Server.Data;

public abstract class TableBase<T> : ISqlTable, ITable<T>
{
	public abstract string TableDbName { get; }
	public abstract string SchemaDbName { get; }

	public abstract Dictionary<string, string> PropertiesToFields { get; }
	public abstract string[] PrimaryKeyProperties { get; }

	protected SqlRequest CreateRequest() => new SqlRequest(this);

	public IEnumerable<T> GetEntities()
	{
		var sqlRequest = CreateRequest();
		return GetEntities(sqlRequest);
	}

	public IEnumerable<T> GetEntities(RequestExpression expression)
	{
		var request = GetRequest(expression);
		return GetEntities(request);
	}

	internal IEnumerable<T> GetEntities(string xmlRequest)
	{
		var expression = ExpressionSerializer.Deserialize(xmlRequest);
		var request = GetRequest(expression);
		return GetEntities(request);
	}

	public abstract IEnumerable<T> GetEntities(SqlRequest req);

	private SqlRequest GetRequest(RequestExpression expression)
	{
		var sqlRequest = CreateRequest();
		sqlRequest.SetWhere(expression.Condition);
		sqlRequest.SetLimit(expression.Limit);
		sqlRequest.SetOrderBy(expression.OrderByField, expression.OrderBy);
		return sqlRequest;
	}
}

public abstract class EntityBase
{
	public abstract void LoadFromReader(FieldReader fr, string tableName);
}