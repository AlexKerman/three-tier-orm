using System.Drawing;
using Oracle.ManagedDataAccess.Client;

namespace Server.Data;

public class FieldReader
{
	private readonly OracleDataReader dr;

	public FieldReader(OracleDataReader dr)
	{
		this.dr = dr;
	}

	public bool Read()
	{
		return dr.Read();
	}

	public void Read(ref int o, string fieldName)
	{
		o = Convert.ToInt32(dr[fieldName]);
	}

	public void Read(ref int? o, string fieldName)
	{
		if (dr[fieldName] is not DBNull) o = Convert.ToInt32(dr[fieldName]);
	}

	public void Read(ref long o, string fieldName)
	{
		o = Convert.ToInt64(dr[fieldName]);
	}

	public void Read(ref long? o, string fieldName)
	{
		if (dr[fieldName] is not DBNull) o = Convert.ToInt64(dr[fieldName]);
	}

	public void Read(ref Guid o, string fieldName)
	{
		o = ConvertToGuid(dr[fieldName]);
	}

	public void Read(ref Guid? o, string fieldName)
	{
		if (dr[fieldName] is not DBNull)
			o = ConvertToGuid(dr[fieldName]);
	}

	public void Read(ref string o, string fieldName)
	{
		o = Convert.ToString(dr[fieldName]);
	}

	public void Read(ref decimal o, string fieldName)
	{
		o = Convert.ToDecimal(dr[fieldName]);
	}

	public void Read(ref DateOnly o, string fieldName)
	{
		o = DateOnly.FromDateTime(Convert.ToDateTime(dr[fieldName]));
	}
	
	public void Read(ref DateOnly? o, string fieldName)
	{
		if (dr[fieldName] is not DBNull)
			o = DateOnly.FromDateTime(Convert.ToDateTime(dr[fieldName]));
	}

	public void Read(ref DateTime o, string fieldName)
	{
		o = Convert.ToDateTime(dr[fieldName]);
	}

	public void Read(ref DateTime? o, string fieldName)
	{
		if (dr[fieldName] is not DBNull)
			o = Convert.ToDateTime(dr[fieldName]);
	}

	public void Read(ref bool o, string fieldName)
	{
		o = Convert.ToBoolean(dr[fieldName]);
	}

	public void Read(ref bool? o, string fieldName)
	{
		if (dr[fieldName] is not DBNull)
			o = Convert.ToBoolean(dr[fieldName]);
	}

	public void Read(ref Color o, string fieldName)
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		dr.Dispose();
	}

	protected Guid ConvertToGuid(object o)
	{
		if (o == null) return Guid.Empty;
		var str = o.ToString();
		if (str == "") return Guid.Empty;
		return new Guid(str);
	}
}