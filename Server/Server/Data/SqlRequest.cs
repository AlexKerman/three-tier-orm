using GrpcContracts.Expressions;
using System.Collections.Generic;

namespace Server.Data;

public interface ISqlTable
{
	string TableDbName { get; }
	string SchemaDbName { get; }

	Dictionary<string, string> PropertiesToFields { get; }
}

public interface ITable<T>
{
	IEnumerable<T> GetEntities(RequestExpression req);
}

public class SqlRequest
{
	public string WhereSql = "";
	public string OrderBySql = "";
	public int Take = 0;
	public int Skip = 0;

	public string TableAsName { get; }

	private ISqlTable table;

	public SqlRequest(ISqlTable table)
	{
		this.table = table;
	}

	public void SetWhere(WhereCondition condition)
	{
		WhereSql = ParseWhere(condition);
	}
	public void SetLimit(Limit limit)
	{
		Skip = limit.Skip;
		Take = limit.Take;
	}

	internal void SetOrderBy(string orderByField, OrderBy orderBy)
	{
		if (orderByField != "")
		{
			OrderBySql = TableAsName + "." + table.PropertiesToFields[orderByField];
			if (orderBy == OrderBy.Descending) OrderBySql += " DESC";
		}
	}

	public string ParseWhere(WhereCondition condition)
	{
		if (condition == null) return "";
		if (condition.Operator == WhereOperator.Value) return condition.Value;
		if (condition.Operator == WhereOperator.Not) return "NOT (" + ParseWhere(condition.Left) + ")";
		if (condition.Operator == WhereOperator.Or) return "(" + ParseWhere(condition.Left) + " OR " + ParseWhere(condition.Right) + ")";
		if (condition.Operator == WhereOperator.And) return ParseWhere(condition.Left) + " AND " + ParseWhere(condition.Right);
		if (condition.Operator == WhereOperator.Equal) return ParseWhere(condition.Left) + " = " + ParseWhere(condition.Right);
		if (condition.Operator == WhereOperator.GreaterThan) return ParseWhere(condition.Left) + " > " + ParseWhere(condition.Right);
		if (condition.Operator == WhereOperator.GreaterThanOrEqual) return ParseWhere(condition.Left) + " => " + ParseWhere(condition.Right);
		if (condition.Operator == WhereOperator.LessThan) return ParseWhere(condition.Left) + " < " + ParseWhere(condition.Right);
		if (condition.Operator == WhereOperator.LessThanOrEqual) return ParseWhere(condition.Left) + " =< " + ParseWhere(condition.Right);
		if (condition.Operator == WhereOperator.Parameter) return condition.Value;

		if (condition.Operator == WhereOperator.Contains)
		{
			return ParseWhere(condition.Right) + " IN(" + ParseWhere(condition.Left) + ")";
		}

		throw new NotImplementedException(condition.Operator.ToString());
	}

	public string GetTextRequest()
	{
		var requestParts = new List<string>();
		requestParts.Add("SELECT " + GetFieldAliases());
		requestParts.Add("FROM " + table.SchemaDbName + "." + table.TableDbName + " " + TableAsName);
		if (WhereSql != "") requestParts.Add("WHERE " + WhereSql);
		if (OrderBySql != "") requestParts.Add("ORDER BY " + OrderBySql);
		if (Skip != 0) requestParts.Add("OFFSET " + Skip + " ROWS");
		if (Take != 0) requestParts.Add("FETCH NEXT " + Take + " ROWS ONLY");
		var request = string.Join("\n", requestParts);
		Console.WriteLine(request);
		return request;
	}

	private string GetFieldAliases()
	{
		var fields = new List<string>();
		foreach (var kv in table.PropertiesToFields)
		{
			var field = kv.Value;
			fields.Add(TableAsName + "." + field + " AS " + TableAsName + "_" + field);
		}

		return string.Join(", ", fields.ToArray());
	}
}