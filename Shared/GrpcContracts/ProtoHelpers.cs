using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcContracts;

public class ProtoHelpers
{
}

public partial class DateProto
{
	public static DateOnly ToDateOnly(DateProto date)
	{
		return new DateOnly(date.Year, date.Month, date.Day);
	}

	public static DateProto FromDateOnly(DateOnly? date)
	{
		if (date == null) return null;

		return new DateProto
		{
			Year = date.Value.Year,
			Month = date.Value.Month,
			Day = date.Value.Day
		};
	}
}

public partial class DecimalProto
{
	public static decimal ToDecimal(DecimalProto value)
	{
		return new decimal(new int[] { value.V1, value.V2, value.V3, value.V4 });
	}

	public static DecimalProto FromDecimal(decimal? value)
	{
		if (value == null) return null;

		var bits = decimal.GetBits(value.Value);
		return new DecimalProto
		{
			V1 = bits[0],
			V2 = bits[1],
			V3 = bits[2],
			V4 = bits[3]
		};
	}
}
