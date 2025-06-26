using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GrpcContracts.Expressions;

public class RequestExpression
{
	[XmlAttribute, DefaultValue(OrderBy.Ascending)]
	public OrderBy OrderBy = OrderBy.Ascending;
	[XmlAttribute, DefaultValue("")]
	public string OrderByField = "";

	public Limit Limit = new Limit();
	public bool ShouldSerializeLimit() => Limit.Skip > 0 || Limit.Take > 0;
	[XmlElement]
	public List<string> Include = new List<string>();
	public bool ShouldSerializeInclude() => Include.Any();
	[DefaultValue(null)]
	public WhereCondition Condition;
}

public enum OrderBy
{
	Ascending,
	Descending
}

public enum WhereOperator
{
	//unary
	Parameter,
	Value,
	Not,

	//binary
	And,
	Or,
	Equal,
	GreaterThan,
	GreaterThanOrEqual,
	LessThan,
	LessThanOrEqual,
	Contains,
}

public class Limit
{
	[XmlAttribute, DefaultValue(0)]
	public int Skip;
	[XmlAttribute, DefaultValue(0)]
	public int Take;
}

public class WhereCondition
{
	public WhereCondition() { }
	public WhereCondition(WhereCondition left, WhereOperator op, WhereCondition right)
	{
		Left = left;
		Operator = op;
		Right = right;
	}

	[XmlAttribute]
	public WhereOperator Operator;
	[XmlAttribute, DefaultValue("")]
	public string Value = "";
	[DefaultValue(null)]
	public WhereCondition Left;
	[DefaultValue(null)]
	public WhereCondition Right;
}