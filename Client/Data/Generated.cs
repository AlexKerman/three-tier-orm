using System;
using Grpc.Core;
using Client;
using GrpcContracts;

namespace Client.Data;

public static class Tables
{
	/// <summary>
	/// small dimension table
	/// </summary>
	public static ChannelTable Channels = new();
	public static CostTable Costs = new();
	/// <summary>
	/// country dimension table (snowflake)
	/// </summary>
	public static CountryTable Countries = new();
	/// <summary>
	/// dimension table
	/// </summary>
	public static CustomerTable Customers = new();
	/// <summary>
	/// dimension table
	/// </summary>
	public static ProductTable Products = new();
	/// <summary>
	/// dimension table without a PK-FK relationship with the facts table, to show outer join functionality
	/// </summary>
	public static PromotionTable Promotions = new();
	/// <summary>
	/// facts table, without a primary key; all rows are uniquely identified by the combination of all foreign keys
	/// </summary>
	public static SaleTable Sales = new();
	/// <summary>
	/// Time dimension table to support multiple hierarchies and materialized views
	/// </summary>
	public static TimeTable Times = new();
}

/// <summary>
/// small dimension table
/// </summary>
public partial class Channel
{
	/// <summary>
	/// primary key column
	/// </summary>
	public int ChannelId { get; set; }
	/// <summary>
	/// e.g. telesales, internet, catalog
	/// </summary>
	public string ChannelDesc { get; set; }
	/// <summary>
	/// e.g. direct, indirect
	/// </summary>
	public string ChannelClass { get; set; }
	public int ChannelClassId { get; set; }
	public string ChannelTotal { get; set; }
	public int ChannelTotalId { get; set; }

	public Channel() { }
	public Channel(ChannelProto proto)
	{
		ChannelId = proto.ChannelId;
		ChannelDesc = proto.ChannelDesc;
		ChannelClass = proto.ChannelClass;
		ChannelClassId = proto.ChannelClassId;
		ChannelTotal = proto.ChannelTotal;
		ChannelTotalId = proto.ChannelTotalId;
}

	public ChannelProto GetProto()
	{
		var proto = new ChannelProto();
		proto.ChannelId = ChannelId;
		proto.ChannelDesc = ChannelDesc;
		proto.ChannelClass = ChannelClass;
		proto.ChannelClassId = ChannelClassId;
		proto.ChannelTotal = ChannelTotal;
		proto.ChannelTotalId = ChannelTotalId;
		return proto;
	}

}

public partial class ChannelTable : TableBase<Channel>
{
}

public partial class Cost
{
	public int ProdId { get; set; }
	public DateOnly TimeId { get; set; }
	public int PromoId { get; set; }
	public int ChannelId { get; set; }
	public decimal UnitCost { get; set; }
	public decimal UnitPrice { get; set; }
	public Channel Channel { get; set; }
	public Product Prod { get; set; }
	public Promotion Promo { get; set; }
	public Time Time { get; set; }

	public Cost() { }
	public Cost(CostProto proto)
	{
		ProdId = proto.ProdId;
		TimeId = DateProto.ToDateOnly(proto.TimeId);
		PromoId = proto.PromoId;
		ChannelId = proto.ChannelId;
		UnitCost = DecimalProto.ToDecimal(proto.UnitCost);
		UnitPrice = DecimalProto.ToDecimal(proto.UnitPrice);
		if (proto.Channel != null)
		{
			Channel = new Channel(proto.Channel);
		}
		if (proto.Prod != null)
		{
			Prod = new Product(proto.Prod);
		}
		if (proto.Promo != null)
		{
			Promo = new Promotion(proto.Promo);
		}
		if (proto.Time != null)
		{
			Time = new Time(proto.Time);
		}
}

	public CostProto GetProto()
	{
		var proto = new CostProto();
		proto.ProdId = ProdId;
		proto.TimeId = DateProto.FromDateOnly(TimeId);
		proto.PromoId = PromoId;
		proto.ChannelId = ChannelId;
		proto.UnitCost = DecimalProto.FromDecimal(UnitCost);
		proto.UnitPrice = DecimalProto.FromDecimal(UnitPrice);
		if (Channel != null) proto.Channel = Channel.GetProto();
		if (Prod != null) proto.Prod = Prod.GetProto();
		if (Promo != null) proto.Promo = Promo.GetProto();
		if (Time != null) proto.Time = Time.GetProto();
		return proto;
	}

}

public partial class CostTable : TableBase<Cost>
{
}

/// <summary>
/// country dimension table (snowflake)
/// </summary>
public partial class Country
{
	/// <summary>
	/// primary key
	/// </summary>
	public int CountryId { get; set; }
	public string CountryIsoCode { get; set; }
	/// <summary>
	/// country name
	/// </summary>
	public string CountryName { get; set; }
	/// <summary>
	/// e.g. Western Europe, to allow hierarchies
	/// </summary>
	public string CountrySubregion { get; set; }
	public int CountrySubregionId { get; set; }
	/// <summary>
	/// e.g. Europe, Asia
	/// </summary>
	public string CountryRegion { get; set; }
	public int CountryRegionId { get; set; }
	public string CountryTotal { get; set; }
	public int CountryTotalId { get; set; }

	public Country() { }
	public Country(CountryProto proto)
	{
		CountryId = proto.CountryId;
		CountryIsoCode = proto.CountryIsoCode;
		CountryName = proto.CountryName;
		CountrySubregion = proto.CountrySubregion;
		CountrySubregionId = proto.CountrySubregionId;
		CountryRegion = proto.CountryRegion;
		CountryRegionId = proto.CountryRegionId;
		CountryTotal = proto.CountryTotal;
		CountryTotalId = proto.CountryTotalId;
}

	public CountryProto GetProto()
	{
		var proto = new CountryProto();
		proto.CountryId = CountryId;
		proto.CountryIsoCode = CountryIsoCode;
		proto.CountryName = CountryName;
		proto.CountrySubregion = CountrySubregion;
		proto.CountrySubregionId = CountrySubregionId;
		proto.CountryRegion = CountryRegion;
		proto.CountryRegionId = CountryRegionId;
		proto.CountryTotal = CountryTotal;
		proto.CountryTotalId = CountryTotalId;
		return proto;
	}

}

public partial class CountryTable : TableBase<Country>
{
}

/// <summary>
/// dimension table
/// </summary>
public partial class Customer
{
	/// <summary>
	/// primary key
	/// </summary>
	public int CustId { get; set; }
	/// <summary>
	/// first name of the customer
	/// </summary>
	public string CustFirstName { get; set; }
	/// <summary>
	/// last name of the customer
	/// </summary>
	public string CustLastName { get; set; }
	/// <summary>
	/// gender; low cardinality attribute
	/// </summary>
	public string CustGender { get; set; }
	/// <summary>
	/// customer year of birth
	/// </summary>
	public int CustYearOfBirth { get; set; }
	/// <summary>
	/// customer marital status; low cardinality attribute
	/// </summary>
	public string? CustMaritalStatus { get; set; }
	/// <summary>
	/// customer street address
	/// </summary>
	public string CustStreetAddress { get; set; }
	/// <summary>
	/// postal code of the customer
	/// </summary>
	public string CustPostalCode { get; set; }
	/// <summary>
	/// city where the customer lives
	/// </summary>
	public string CustCity { get; set; }
	public int CustCityId { get; set; }
	/// <summary>
	/// customer geography: state or province
	/// </summary>
	public string CustStateProvince { get; set; }
	public int CustStateProvinceId { get; set; }
	/// <summary>
	/// foreign key to the countries table (snowflake)
	/// </summary>
	public int CountryId { get; set; }
	/// <summary>
	/// customer main phone number
	/// </summary>
	public string CustMainPhoneNumber { get; set; }
	/// <summary>
	/// customer income level
	/// </summary>
	public string? CustIncomeLevel { get; set; }
	/// <summary>
	/// customer credit limit
	/// </summary>
	public decimal? CustCreditLimit { get; set; }
	/// <summary>
	/// customer email id
	/// </summary>
	public string? CustEmail { get; set; }
	public string CustTotal { get; set; }
	public int CustTotalId { get; set; }
	public int? CustSrcId { get; set; }
	public DateOnly? CustEffFrom { get; set; }
	public DateOnly? CustEffTo { get; set; }
	public string? CustValid { get; set; }
	public Country Country { get; set; }

	public Customer() { }
	public Customer(CustomerProto proto)
	{
		CustId = proto.CustId;
		CustFirstName = proto.CustFirstName;
		CustLastName = proto.CustLastName;
		CustGender = proto.CustGender;
		CustYearOfBirth = proto.CustYearOfBirth;
		if (proto.HasCustMaritalStatus) CustMaritalStatus = proto.CustMaritalStatus;
		CustStreetAddress = proto.CustStreetAddress;
		CustPostalCode = proto.CustPostalCode;
		CustCity = proto.CustCity;
		CustCityId = proto.CustCityId;
		CustStateProvince = proto.CustStateProvince;
		CustStateProvinceId = proto.CustStateProvinceId;
		CountryId = proto.CountryId;
		CustMainPhoneNumber = proto.CustMainPhoneNumber;
		if (proto.HasCustIncomeLevel) CustIncomeLevel = proto.CustIncomeLevel;
		if (proto.CustCreditLimit != null) CustCreditLimit = DecimalProto.ToDecimal(proto.CustCreditLimit);
		if (proto.HasCustEmail) CustEmail = proto.CustEmail;
		CustTotal = proto.CustTotal;
		CustTotalId = proto.CustTotalId;
		if (proto.HasCustSrcId) CustSrcId = proto.CustSrcId;
		if (proto.CustEffFrom != null) CustEffFrom = DateProto.ToDateOnly(proto.CustEffFrom);
		if (proto.CustEffTo != null) CustEffTo = DateProto.ToDateOnly(proto.CustEffTo);
		if (proto.HasCustValid) CustValid = proto.CustValid;
		if (proto.Country != null)
		{
			Country = new Country(proto.Country);
		}
}

	public CustomerProto GetProto()
	{
		var proto = new CustomerProto();
		proto.CustId = CustId;
		proto.CustFirstName = CustFirstName;
		proto.CustLastName = CustLastName;
		proto.CustGender = CustGender;
		proto.CustYearOfBirth = CustYearOfBirth;
		if (CustMaritalStatus != null) proto.CustMaritalStatus = CustMaritalStatus;
		proto.CustStreetAddress = CustStreetAddress;
		proto.CustPostalCode = CustPostalCode;
		proto.CustCity = CustCity;
		proto.CustCityId = CustCityId;
		proto.CustStateProvince = CustStateProvince;
		proto.CustStateProvinceId = CustStateProvinceId;
		proto.CountryId = CountryId;
		proto.CustMainPhoneNumber = CustMainPhoneNumber;
		if (CustIncomeLevel != null) proto.CustIncomeLevel = CustIncomeLevel;
		proto.CustCreditLimit = DecimalProto.FromDecimal(CustCreditLimit);
		if (CustEmail != null) proto.CustEmail = CustEmail;
		proto.CustTotal = CustTotal;
		proto.CustTotalId = CustTotalId;
		if (CustSrcId != null) proto.CustSrcId = CustSrcId.Value;
		proto.CustEffFrom = DateProto.FromDateOnly(CustEffFrom);
		proto.CustEffTo = DateProto.FromDateOnly(CustEffTo);
		if (CustValid != null) proto.CustValid = CustValid;
		if (Country != null) proto.Country = Country.GetProto();
		return proto;
	}

}

public partial class CustomerTable : TableBase<Customer>
{
}

/// <summary>
/// dimension table
/// </summary>
public partial class Product
{
	/// <summary>
	/// primary key
	/// </summary>
	public int ProdId { get; set; }
	/// <summary>
	/// product name
	/// </summary>
	public string ProdName { get; set; }
	/// <summary>
	/// product description
	/// </summary>
	public string ProdDesc { get; set; }
	/// <summary>
	/// product subcategory
	/// </summary>
	public string ProdSubcategory { get; set; }
	public int ProdSubcategoryId { get; set; }
	/// <summary>
	/// product subcategory description
	/// </summary>
	public string ProdSubcategoryDesc { get; set; }
	/// <summary>
	/// product category
	/// </summary>
	public string ProdCategory { get; set; }
	public int ProdCategoryId { get; set; }
	/// <summary>
	/// product category description
	/// </summary>
	public string ProdCategoryDesc { get; set; }
	/// <summary>
	/// product weight class
	/// </summary>
	public int ProdWeightClass { get; set; }
	/// <summary>
	/// product unit of measure
	/// </summary>
	public string? ProdUnitOfMeasure { get; set; }
	/// <summary>
	/// product package size
	/// </summary>
	public string ProdPackSize { get; set; }
	/// <summary>
	/// this column
	/// </summary>
	public int SupplierId { get; set; }
	/// <summary>
	/// product status
	/// </summary>
	public string ProdStatus { get; set; }
	/// <summary>
	/// product list price
	/// </summary>
	public decimal ProdListPrice { get; set; }
	/// <summary>
	/// product minimum price
	/// </summary>
	public decimal ProdMinPrice { get; set; }
	public string ProdTotal { get; set; }
	public int ProdTotalId { get; set; }
	public int? ProdSrcId { get; set; }
	public DateOnly? ProdEffFrom { get; set; }
	public DateOnly? ProdEffTo { get; set; }
	public string? ProdValid { get; set; }

	public Product() { }
	public Product(ProductProto proto)
	{
		ProdId = proto.ProdId;
		ProdName = proto.ProdName;
		ProdDesc = proto.ProdDesc;
		ProdSubcategory = proto.ProdSubcategory;
		ProdSubcategoryId = proto.ProdSubcategoryId;
		ProdSubcategoryDesc = proto.ProdSubcategoryDesc;
		ProdCategory = proto.ProdCategory;
		ProdCategoryId = proto.ProdCategoryId;
		ProdCategoryDesc = proto.ProdCategoryDesc;
		ProdWeightClass = proto.ProdWeightClass;
		if (proto.HasProdUnitOfMeasure) ProdUnitOfMeasure = proto.ProdUnitOfMeasure;
		ProdPackSize = proto.ProdPackSize;
		SupplierId = proto.SupplierId;
		ProdStatus = proto.ProdStatus;
		ProdListPrice = DecimalProto.ToDecimal(proto.ProdListPrice);
		ProdMinPrice = DecimalProto.ToDecimal(proto.ProdMinPrice);
		ProdTotal = proto.ProdTotal;
		ProdTotalId = proto.ProdTotalId;
		if (proto.HasProdSrcId) ProdSrcId = proto.ProdSrcId;
		if (proto.ProdEffFrom != null) ProdEffFrom = DateProto.ToDateOnly(proto.ProdEffFrom);
		if (proto.ProdEffTo != null) ProdEffTo = DateProto.ToDateOnly(proto.ProdEffTo);
		if (proto.HasProdValid) ProdValid = proto.ProdValid;
}

	public ProductProto GetProto()
	{
		var proto = new ProductProto();
		proto.ProdId = ProdId;
		proto.ProdName = ProdName;
		proto.ProdDesc = ProdDesc;
		proto.ProdSubcategory = ProdSubcategory;
		proto.ProdSubcategoryId = ProdSubcategoryId;
		proto.ProdSubcategoryDesc = ProdSubcategoryDesc;
		proto.ProdCategory = ProdCategory;
		proto.ProdCategoryId = ProdCategoryId;
		proto.ProdCategoryDesc = ProdCategoryDesc;
		proto.ProdWeightClass = ProdWeightClass;
		if (ProdUnitOfMeasure != null) proto.ProdUnitOfMeasure = ProdUnitOfMeasure;
		proto.ProdPackSize = ProdPackSize;
		proto.SupplierId = SupplierId;
		proto.ProdStatus = ProdStatus;
		proto.ProdListPrice = DecimalProto.FromDecimal(ProdListPrice);
		proto.ProdMinPrice = DecimalProto.FromDecimal(ProdMinPrice);
		proto.ProdTotal = ProdTotal;
		proto.ProdTotalId = ProdTotalId;
		if (ProdSrcId != null) proto.ProdSrcId = ProdSrcId.Value;
		proto.ProdEffFrom = DateProto.FromDateOnly(ProdEffFrom);
		proto.ProdEffTo = DateProto.FromDateOnly(ProdEffTo);
		if (ProdValid != null) proto.ProdValid = ProdValid;
		return proto;
	}

}

public partial class ProductTable : TableBase<Product>
{
}

/// <summary>
/// dimension table without a PK-FK relationship with the facts table, to show outer join functionality
/// </summary>
public partial class Promotion
{
	/// <summary>
	/// primary key column
	/// </summary>
	public int PromoId { get; set; }
	/// <summary>
	/// promotion description
	/// </summary>
	public string PromoName { get; set; }
	/// <summary>
	/// enables to investigate promotion hierarchies
	/// </summary>
	public string PromoSubcategory { get; set; }
	public int PromoSubcategoryId { get; set; }
	/// <summary>
	/// promotion category
	/// </summary>
	public string PromoCategory { get; set; }
	public int PromoCategoryId { get; set; }
	/// <summary>
	/// promotion cost, to do promotion effect calculations
	/// </summary>
	public decimal PromoCost { get; set; }
	/// <summary>
	/// promotion begin day
	/// </summary>
	public DateOnly PromoBeginDate { get; set; }
	/// <summary>
	/// promotion end day
	/// </summary>
	public DateOnly PromoEndDate { get; set; }
	public string PromoTotal { get; set; }
	public int PromoTotalId { get; set; }

	public Promotion() { }
	public Promotion(PromotionProto proto)
	{
		PromoId = proto.PromoId;
		PromoName = proto.PromoName;
		PromoSubcategory = proto.PromoSubcategory;
		PromoSubcategoryId = proto.PromoSubcategoryId;
		PromoCategory = proto.PromoCategory;
		PromoCategoryId = proto.PromoCategoryId;
		PromoCost = DecimalProto.ToDecimal(proto.PromoCost);
		PromoBeginDate = DateProto.ToDateOnly(proto.PromoBeginDate);
		PromoEndDate = DateProto.ToDateOnly(proto.PromoEndDate);
		PromoTotal = proto.PromoTotal;
		PromoTotalId = proto.PromoTotalId;
}

	public PromotionProto GetProto()
	{
		var proto = new PromotionProto();
		proto.PromoId = PromoId;
		proto.PromoName = PromoName;
		proto.PromoSubcategory = PromoSubcategory;
		proto.PromoSubcategoryId = PromoSubcategoryId;
		proto.PromoCategory = PromoCategory;
		proto.PromoCategoryId = PromoCategoryId;
		proto.PromoCost = DecimalProto.FromDecimal(PromoCost);
		proto.PromoBeginDate = DateProto.FromDateOnly(PromoBeginDate);
		proto.PromoEndDate = DateProto.FromDateOnly(PromoEndDate);
		proto.PromoTotal = PromoTotal;
		proto.PromoTotalId = PromoTotalId;
		return proto;
	}

}

public partial class PromotionTable : TableBase<Promotion>
{
}

/// <summary>
/// facts table, without a primary key; all rows are uniquely identified by the combination of all foreign keys
/// </summary>
public partial class Sale
{
	/// <summary>
	/// FK to the products dimension table
	/// </summary>
	public int ProdId { get; set; }
	/// <summary>
	/// FK to the customers dimension table
	/// </summary>
	public int CustId { get; set; }
	/// <summary>
	/// FK to the times dimension table
	/// </summary>
	public DateOnly TimeId { get; set; }
	/// <summary>
	/// FK to the channels dimension table
	/// </summary>
	public int ChannelId { get; set; }
	/// <summary>
	/// promotion identifier, without FK constraint (intentionally) to show outer join optimization
	/// </summary>
	public int PromoId { get; set; }
	/// <summary>
	/// product quantity sold with the transaction
	/// </summary>
	public int QuantitySold { get; set; }
	/// <summary>
	/// invoiced amount to the customer
	/// </summary>
	public decimal AmountSold { get; set; }
	public Channel Channel { get; set; }
	public Customer Cust { get; set; }
	public Product Prod { get; set; }
	public Promotion Promo { get; set; }
	public Time Time { get; set; }

	public Sale() { }
	public Sale(SaleProto proto)
	{
		ProdId = proto.ProdId;
		CustId = proto.CustId;
		TimeId = DateProto.ToDateOnly(proto.TimeId);
		ChannelId = proto.ChannelId;
		PromoId = proto.PromoId;
		QuantitySold = proto.QuantitySold;
		AmountSold = DecimalProto.ToDecimal(proto.AmountSold);
		if (proto.Channel != null)
		{
			Channel = new Channel(proto.Channel);
		}
		if (proto.Cust != null)
		{
			Cust = new Customer(proto.Cust);
		}
		if (proto.Prod != null)
		{
			Prod = new Product(proto.Prod);
		}
		if (proto.Promo != null)
		{
			Promo = new Promotion(proto.Promo);
		}
		if (proto.Time != null)
		{
			Time = new Time(proto.Time);
		}
}

	public SaleProto GetProto()
	{
		var proto = new SaleProto();
		proto.ProdId = ProdId;
		proto.CustId = CustId;
		proto.TimeId = DateProto.FromDateOnly(TimeId);
		proto.ChannelId = ChannelId;
		proto.PromoId = PromoId;
		proto.QuantitySold = QuantitySold;
		proto.AmountSold = DecimalProto.FromDecimal(AmountSold);
		if (Channel != null) proto.Channel = Channel.GetProto();
		if (Cust != null) proto.Cust = Cust.GetProto();
		if (Prod != null) proto.Prod = Prod.GetProto();
		if (Promo != null) proto.Promo = Promo.GetProto();
		if (Time != null) proto.Time = Time.GetProto();
		return proto;
	}

}

public partial class SaleTable : TableBase<Sale>
{
}

/// <summary>
/// Time dimension table to support multiple hierarchies and materialized views
/// </summary>
public partial class Time
{
	/// <summary>
	/// primary key; day date, finest granularity, CORRECT ORDER
	/// </summary>
	public DateOnly TimeId { get; set; }
	/// <summary>
	/// Monday to Sunday, repeating
	/// </summary>
	public string DayName { get; set; }
	/// <summary>
	/// 1 to 7, repeating
	/// </summary>
	public int DayNumberInWeek { get; set; }
	/// <summary>
	/// 1 to 31, repeating
	/// </summary>
	public int DayNumberInMonth { get; set; }
	/// <summary>
	/// 1 to 53, repeating
	/// </summary>
	public int CalendarWeekNumber { get; set; }
	/// <summary>
	/// 1 to 53, repeating
	/// </summary>
	public int FiscalWeekNumber { get; set; }
	/// <summary>
	/// date of last day in week, CORRECT ORDER
	/// </summary>
	public DateOnly WeekEndingDay { get; set; }
	public int WeekEndingDayId { get; set; }
	/// <summary>
	/// 1 to 12, repeating
	/// </summary>
	public int CalendarMonthNumber { get; set; }
	/// <summary>
	/// 1 to 12, repeating
	/// </summary>
	public int FiscalMonthNumber { get; set; }
	/// <summary>
	/// e.g. 1998-01, CORRECT ORDER
	/// </summary>
	public string CalendarMonthDesc { get; set; }
	public int CalendarMonthId { get; set; }
	/// <summary>
	/// e.g. 1998-01, CORRECT ORDER
	/// </summary>
	public string FiscalMonthDesc { get; set; }
	public int FiscalMonthId { get; set; }
	/// <summary>
	/// e.g. 28,31, repeating
	/// </summary>
	public decimal DaysInCalMonth { get; set; }
	/// <summary>
	/// e.g. 25,32, repeating
	/// </summary>
	public decimal DaysInFisMonth { get; set; }
	/// <summary>
	/// last day of calendar month
	/// </summary>
	public DateOnly EndOfCalMonth { get; set; }
	/// <summary>
	/// last day of fiscal month
	/// </summary>
	public DateOnly EndOfFisMonth { get; set; }
	/// <summary>
	/// January to December, repeating
	/// </summary>
	public string CalendarMonthName { get; set; }
	/// <summary>
	/// January to December, repeating
	/// </summary>
	public string FiscalMonthName { get; set; }
	/// <summary>
	/// e.g. 1998-Q1, CORRECT ORDER
	/// </summary>
	public string CalendarQuarterDesc { get; set; }
	public int CalendarQuarterId { get; set; }
	/// <summary>
	/// e.g. 1999-Q3, CORRECT ORDER
	/// </summary>
	public string FiscalQuarterDesc { get; set; }
	public int FiscalQuarterId { get; set; }
	/// <summary>
	/// e.g. 88,90, repeating
	/// </summary>
	public decimal DaysInCalQuarter { get; set; }
	/// <summary>
	/// e.g. 88,90, repeating
	/// </summary>
	public decimal DaysInFisQuarter { get; set; }
	/// <summary>
	/// last day of calendar quarter
	/// </summary>
	public DateOnly EndOfCalQuarter { get; set; }
	/// <summary>
	/// last day of fiscal quarter
	/// </summary>
	public DateOnly EndOfFisQuarter { get; set; }
	/// <summary>
	/// 1 to 4, repeating
	/// </summary>
	public int CalendarQuarterNumber { get; set; }
	/// <summary>
	/// 1 to 4, repeating
	/// </summary>
	public int FiscalQuarterNumber { get; set; }
	/// <summary>
	/// e.g. 1999, CORRECT ORDER
	/// </summary>
	public int CalendarYear { get; set; }
	public int CalendarYearId { get; set; }
	/// <summary>
	/// e.g. 1999, CORRECT ORDER
	/// </summary>
	public int FiscalYear { get; set; }
	public int FiscalYearId { get; set; }
	/// <summary>
	/// 365,366 repeating
	/// </summary>
	public decimal DaysInCalYear { get; set; }
	/// <summary>
	/// e.g. 355,364, repeating
	/// </summary>
	public decimal DaysInFisYear { get; set; }
	/// <summary>
	/// last day of cal year
	/// </summary>
	public DateOnly EndOfCalYear { get; set; }
	/// <summary>
	/// last day of fiscal year
	/// </summary>
	public DateOnly EndOfFisYear { get; set; }

	public Time() { }
	public Time(TimeProto proto)
	{
		TimeId = DateProto.ToDateOnly(proto.TimeId);
		DayName = proto.DayName;
		DayNumberInWeek = proto.DayNumberInWeek;
		DayNumberInMonth = proto.DayNumberInMonth;
		CalendarWeekNumber = proto.CalendarWeekNumber;
		FiscalWeekNumber = proto.FiscalWeekNumber;
		WeekEndingDay = DateProto.ToDateOnly(proto.WeekEndingDay);
		WeekEndingDayId = proto.WeekEndingDayId;
		CalendarMonthNumber = proto.CalendarMonthNumber;
		FiscalMonthNumber = proto.FiscalMonthNumber;
		CalendarMonthDesc = proto.CalendarMonthDesc;
		CalendarMonthId = proto.CalendarMonthId;
		FiscalMonthDesc = proto.FiscalMonthDesc;
		FiscalMonthId = proto.FiscalMonthId;
		DaysInCalMonth = DecimalProto.ToDecimal(proto.DaysInCalMonth);
		DaysInFisMonth = DecimalProto.ToDecimal(proto.DaysInFisMonth);
		EndOfCalMonth = DateProto.ToDateOnly(proto.EndOfCalMonth);
		EndOfFisMonth = DateProto.ToDateOnly(proto.EndOfFisMonth);
		CalendarMonthName = proto.CalendarMonthName;
		FiscalMonthName = proto.FiscalMonthName;
		CalendarQuarterDesc = proto.CalendarQuarterDesc;
		CalendarQuarterId = proto.CalendarQuarterId;
		FiscalQuarterDesc = proto.FiscalQuarterDesc;
		FiscalQuarterId = proto.FiscalQuarterId;
		DaysInCalQuarter = DecimalProto.ToDecimal(proto.DaysInCalQuarter);
		DaysInFisQuarter = DecimalProto.ToDecimal(proto.DaysInFisQuarter);
		EndOfCalQuarter = DateProto.ToDateOnly(proto.EndOfCalQuarter);
		EndOfFisQuarter = DateProto.ToDateOnly(proto.EndOfFisQuarter);
		CalendarQuarterNumber = proto.CalendarQuarterNumber;
		FiscalQuarterNumber = proto.FiscalQuarterNumber;
		CalendarYear = proto.CalendarYear;
		CalendarYearId = proto.CalendarYearId;
		FiscalYear = proto.FiscalYear;
		FiscalYearId = proto.FiscalYearId;
		DaysInCalYear = DecimalProto.ToDecimal(proto.DaysInCalYear);
		DaysInFisYear = DecimalProto.ToDecimal(proto.DaysInFisYear);
		EndOfCalYear = DateProto.ToDateOnly(proto.EndOfCalYear);
		EndOfFisYear = DateProto.ToDateOnly(proto.EndOfFisYear);
}

	public TimeProto GetProto()
	{
		var proto = new TimeProto();
		proto.TimeId = DateProto.FromDateOnly(TimeId);
		proto.DayName = DayName;
		proto.DayNumberInWeek = DayNumberInWeek;
		proto.DayNumberInMonth = DayNumberInMonth;
		proto.CalendarWeekNumber = CalendarWeekNumber;
		proto.FiscalWeekNumber = FiscalWeekNumber;
		proto.WeekEndingDay = DateProto.FromDateOnly(WeekEndingDay);
		proto.WeekEndingDayId = WeekEndingDayId;
		proto.CalendarMonthNumber = CalendarMonthNumber;
		proto.FiscalMonthNumber = FiscalMonthNumber;
		proto.CalendarMonthDesc = CalendarMonthDesc;
		proto.CalendarMonthId = CalendarMonthId;
		proto.FiscalMonthDesc = FiscalMonthDesc;
		proto.FiscalMonthId = FiscalMonthId;
		proto.DaysInCalMonth = DecimalProto.FromDecimal(DaysInCalMonth);
		proto.DaysInFisMonth = DecimalProto.FromDecimal(DaysInFisMonth);
		proto.EndOfCalMonth = DateProto.FromDateOnly(EndOfCalMonth);
		proto.EndOfFisMonth = DateProto.FromDateOnly(EndOfFisMonth);
		proto.CalendarMonthName = CalendarMonthName;
		proto.FiscalMonthName = FiscalMonthName;
		proto.CalendarQuarterDesc = CalendarQuarterDesc;
		proto.CalendarQuarterId = CalendarQuarterId;
		proto.FiscalQuarterDesc = FiscalQuarterDesc;
		proto.FiscalQuarterId = FiscalQuarterId;
		proto.DaysInCalQuarter = DecimalProto.FromDecimal(DaysInCalQuarter);
		proto.DaysInFisQuarter = DecimalProto.FromDecimal(DaysInFisQuarter);
		proto.EndOfCalQuarter = DateProto.FromDateOnly(EndOfCalQuarter);
		proto.EndOfFisQuarter = DateProto.FromDateOnly(EndOfFisQuarter);
		proto.CalendarQuarterNumber = CalendarQuarterNumber;
		proto.FiscalQuarterNumber = FiscalQuarterNumber;
		proto.CalendarYear = CalendarYear;
		proto.CalendarYearId = CalendarYearId;
		proto.FiscalYear = FiscalYear;
		proto.FiscalYearId = FiscalYearId;
		proto.DaysInCalYear = DecimalProto.FromDecimal(DaysInCalYear);
		proto.DaysInFisYear = DecimalProto.FromDecimal(DaysInFisYear);
		proto.EndOfCalYear = DateProto.FromDateOnly(EndOfCalYear);
		proto.EndOfFisYear = DateProto.FromDateOnly(EndOfFisYear);
		return proto;
	}

}

public partial class TimeTable : TableBase<Time>
{
}