using System;
using Grpc.Core;
using Client;

namespace Client.Data;

public static class Tables
{
	/// <summary>
	///small dimension table
	/// </summary>
	public static ChannelTable Channels = new();
	public static CostTable Costs = new();
	/// <summary>
	///country dimension table (snowflake)
	/// </summary>
	public static CountryTable Countries = new();
	/// <summary>
	///dimension table
	/// </summary>
	public static CustomerTable Customers = new();
	/// <summary>
	///dimension table
	/// </summary>
	public static ProductTable Products = new();
	/// <summary>
	///dimension table without a PK-FK relationship with the facts table, to show outer join functionality
	/// </summary>
	public static PromotionTable Promotions = new();
	/// <summary>
	///facts table, without a primary key; all rows are uniquely identified by the combination of all foreign keys
	/// </summary>
	public static SaleTable Sales = new();
	/// <summary>
	///Time dimension table to support multiple hierarchies and materialized views
	/// </summary>
	public static TimeTable Times = new();
}

/// <summary>
///small dimension table
/// </summary>
public partial class Channel
{
	/// <summary>
	///primary key column
	/// </summary>
	public int ChannelId { get; set; }
	/// <summary>
	///e.g. telesales, internet, catalog
	/// </summary>
	public string ChannelDesc { get; set; }
	/// <summary>
	///e.g. direct, indirect
	/// </summary>
	public string ChannelClass { get; set; }
	public int ChannelClassId { get; set; }
	public string ChannelTotal { get; set; }
	public int ChannelTotalId { get; set; }
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
}

public partial class CostTable : TableBase<Cost>
{
}

/// <summary>
///country dimension table (snowflake)
/// </summary>
public partial class Country
{
	/// <summary>
	///primary key
	/// </summary>
	public int CountryId { get; set; }
	public string CountryIsoCode { get; set; }
	/// <summary>
	///country name
	/// </summary>
	public string CountryName { get; set; }
	/// <summary>
	///e.g. Western Europe, to allow hierarchies
	/// </summary>
	public string CountrySubregion { get; set; }
	public int CountrySubregionId { get; set; }
	/// <summary>
	///e.g. Europe, Asia
	/// </summary>
	public string CountryRegion { get; set; }
	public int CountryRegionId { get; set; }
	public string CountryTotal { get; set; }
	public int CountryTotalId { get; set; }
}

public partial class CountryTable : TableBase<Country>
{
}

/// <summary>
///dimension table
/// </summary>
public partial class Customer
{
	/// <summary>
	///primary key
	/// </summary>
	public int CustId { get; set; }
	/// <summary>
	///first name of the customer
	/// </summary>
	public string CustFirstName { get; set; }
	/// <summary>
	///last name of the customer
	/// </summary>
	public string CustLastName { get; set; }
	/// <summary>
	///gender; low cardinality attribute
	/// </summary>
	public string CustGender { get; set; }
	/// <summary>
	///customer year of birth
	/// </summary>
	public int CustYearOfBirth { get; set; }
	/// <summary>
	///customer marital status; low cardinality attribute
	/// </summary>
	public string? CustMaritalStatus { get; set; }
	/// <summary>
	///customer street address
	/// </summary>
	public string CustStreetAddress { get; set; }
	/// <summary>
	///postal code of the customer
	/// </summary>
	public string CustPostalCode { get; set; }
	/// <summary>
	///city where the customer lives
	/// </summary>
	public string CustCity { get; set; }
	public int CustCityId { get; set; }
	/// <summary>
	///customer geography: state or province
	/// </summary>
	public string CustStateProvince { get; set; }
	public int CustStateProvinceId { get; set; }
	/// <summary>
	///foreign key to the countries table (snowflake)
	/// </summary>
	public int CountryId { get; set; }
	/// <summary>
	///customer main phone number
	/// </summary>
	public string CustMainPhoneNumber { get; set; }
	/// <summary>
	///customer income level
	/// </summary>
	public string? CustIncomeLevel { get; set; }
	/// <summary>
	///customer credit limit
	/// </summary>
	public decimal? CustCreditLimit { get; set; }
	/// <summary>
	///customer email id
	/// </summary>
	public string? CustEmail { get; set; }
	public string CustTotal { get; set; }
	public int CustTotalId { get; set; }
	public int? CustSrcId { get; set; }
	public DateOnly? CustEffFrom { get; set; }
	public DateOnly? CustEffTo { get; set; }
	public string? CustValid { get; set; }
	public Country Country { get; set; }
}

public partial class CustomerTable : TableBase<Customer>
{
}

/// <summary>
///dimension table
/// </summary>
public partial class Product
{
	/// <summary>
	///primary key
	/// </summary>
	public int ProdId { get; set; }
	/// <summary>
	///product name
	/// </summary>
	public string ProdName { get; set; }
	/// <summary>
	///product description
	/// </summary>
	public string ProdDesc { get; set; }
	/// <summary>
	///product subcategory
	/// </summary>
	public string ProdSubcategory { get; set; }
	public int ProdSubcategoryId { get; set; }
	/// <summary>
	///product subcategory description
	/// </summary>
	public string ProdSubcategoryDesc { get; set; }
	/// <summary>
	///product category
	/// </summary>
	public string ProdCategory { get; set; }
	public int ProdCategoryId { get; set; }
	/// <summary>
	///product category description
	/// </summary>
	public string ProdCategoryDesc { get; set; }
	/// <summary>
	///product weight class
	/// </summary>
	public int ProdWeightClass { get; set; }
	/// <summary>
	///product unit of measure
	/// </summary>
	public string? ProdUnitOfMeasure { get; set; }
	/// <summary>
	///product package size
	/// </summary>
	public string ProdPackSize { get; set; }
	/// <summary>
	///this column
	/// </summary>
	public int SupplierId { get; set; }
	/// <summary>
	///product status
	/// </summary>
	public string ProdStatus { get; set; }
	/// <summary>
	///product list price
	/// </summary>
	public decimal ProdListPrice { get; set; }
	/// <summary>
	///product minimum price
	/// </summary>
	public decimal ProdMinPrice { get; set; }
	public string ProdTotal { get; set; }
	public int ProdTotalId { get; set; }
	public int? ProdSrcId { get; set; }
	public DateOnly? ProdEffFrom { get; set; }
	public DateOnly? ProdEffTo { get; set; }
	public string? ProdValid { get; set; }
}

public partial class ProductTable : TableBase<Product>
{
}

/// <summary>
///dimension table without a PK-FK relationship with the facts table, to show outer join functionality
/// </summary>
public partial class Promotion
{
	/// <summary>
	///primary key column
	/// </summary>
	public int PromoId { get; set; }
	/// <summary>
	///promotion description
	/// </summary>
	public string PromoName { get; set; }
	/// <summary>
	///enables to investigate promotion hierarchies
	/// </summary>
	public string PromoSubcategory { get; set; }
	public int PromoSubcategoryId { get; set; }
	/// <summary>
	///promotion category
	/// </summary>
	public string PromoCategory { get; set; }
	public int PromoCategoryId { get; set; }
	/// <summary>
	///promotion cost, to do promotion effect calculations
	/// </summary>
	public decimal PromoCost { get; set; }
	/// <summary>
	///promotion begin day
	/// </summary>
	public DateOnly PromoBeginDate { get; set; }
	/// <summary>
	///promotion end day
	/// </summary>
	public DateOnly PromoEndDate { get; set; }
	public string PromoTotal { get; set; }
	public int PromoTotalId { get; set; }
}

public partial class PromotionTable : TableBase<Promotion>
{
}

/// <summary>
///facts table, without a primary key; all rows are uniquely identified by the combination of all foreign keys
/// </summary>
public partial class Sale
{
	/// <summary>
	///FK to the products dimension table
	/// </summary>
	public int ProdId { get; set; }
	/// <summary>
	///FK to the customers dimension table
	/// </summary>
	public int CustId { get; set; }
	/// <summary>
	///FK to the times dimension table
	/// </summary>
	public DateOnly TimeId { get; set; }
	/// <summary>
	///FK to the channels dimension table
	/// </summary>
	public int ChannelId { get; set; }
	/// <summary>
	///promotion identifier, without FK constraint (intentionally) to show outer join optimization
	/// </summary>
	public int PromoId { get; set; }
	/// <summary>
	///product quantity sold with the transaction
	/// </summary>
	public int QuantitySold { get; set; }
	/// <summary>
	///invoiced amount to the customer
	/// </summary>
	public decimal AmountSold { get; set; }
	public Channel Channel { get; set; }
	public Customer Cust { get; set; }
	public Product Prod { get; set; }
	public Promotion Promo { get; set; }
	public Time Time { get; set; }
}

public partial class SaleTable : TableBase<Sale>
{
}

/// <summary>
///Time dimension table to support multiple hierarchies and materialized views
/// </summary>
public partial class Time
{
	/// <summary>
	///primary key; day date, finest granularity, CORRECT ORDER
	/// </summary>
	public DateOnly TimeId { get; set; }
	/// <summary>
	///Monday to Sunday, repeating
	/// </summary>
	public string DayName { get; set; }
	/// <summary>
	///1 to 7, repeating
	/// </summary>
	public int DayNumberInWeek { get; set; }
	/// <summary>
	///1 to 31, repeating
	/// </summary>
	public int DayNumberInMonth { get; set; }
	/// <summary>
	///1 to 53, repeating
	/// </summary>
	public int CalendarWeekNumber { get; set; }
	/// <summary>
	///1 to 53, repeating
	/// </summary>
	public int FiscalWeekNumber { get; set; }
	/// <summary>
	///date of last day in week, CORRECT ORDER
	/// </summary>
	public DateOnly WeekEndingDay { get; set; }
	public int WeekEndingDayId { get; set; }
	/// <summary>
	///1 to 12, repeating
	/// </summary>
	public int CalendarMonthNumber { get; set; }
	/// <summary>
	///1 to 12, repeating
	/// </summary>
	public int FiscalMonthNumber { get; set; }
	/// <summary>
	///e.g. 1998-01, CORRECT ORDER
	/// </summary>
	public string CalendarMonthDesc { get; set; }
	public int CalendarMonthId { get; set; }
	/// <summary>
	///e.g. 1998-01, CORRECT ORDER
	/// </summary>
	public string FiscalMonthDesc { get; set; }
	public int FiscalMonthId { get; set; }
	/// <summary>
	///e.g. 28,31, repeating
	/// </summary>
	public decimal DaysInCalMonth { get; set; }
	/// <summary>
	///e.g. 25,32, repeating
	/// </summary>
	public decimal DaysInFisMonth { get; set; }
	/// <summary>
	///last day of calendar month
	/// </summary>
	public DateOnly EndOfCalMonth { get; set; }
	/// <summary>
	///last day of fiscal month
	/// </summary>
	public DateOnly EndOfFisMonth { get; set; }
	/// <summary>
	///January to December, repeating
	/// </summary>
	public string CalendarMonthName { get; set; }
	/// <summary>
	///January to December, repeating
	/// </summary>
	public string FiscalMonthName { get; set; }
	/// <summary>
	///e.g. 1998-Q1, CORRECT ORDER
	/// </summary>
	public string CalendarQuarterDesc { get; set; }
	public int CalendarQuarterId { get; set; }
	/// <summary>
	///e.g. 1999-Q3, CORRECT ORDER
	/// </summary>
	public string FiscalQuarterDesc { get; set; }
	public int FiscalQuarterId { get; set; }
	/// <summary>
	///e.g. 88,90, repeating
	/// </summary>
	public decimal DaysInCalQuarter { get; set; }
	/// <summary>
	///e.g. 88,90, repeating
	/// </summary>
	public decimal DaysInFisQuarter { get; set; }
	/// <summary>
	///last day of calendar quarter
	/// </summary>
	public DateOnly EndOfCalQuarter { get; set; }
	/// <summary>
	///last day of fiscal quarter
	/// </summary>
	public DateOnly EndOfFisQuarter { get; set; }
	/// <summary>
	///1 to 4, repeating
	/// </summary>
	public int CalendarQuarterNumber { get; set; }
	/// <summary>
	///1 to 4, repeating
	/// </summary>
	public int FiscalQuarterNumber { get; set; }
	/// <summary>
	///e.g. 1999, CORRECT ORDER
	/// </summary>
	public int CalendarYear { get; set; }
	public int CalendarYearId { get; set; }
	/// <summary>
	///e.g. 1999, CORRECT ORDER
	/// </summary>
	public int FiscalYear { get; set; }
	public int FiscalYearId { get; set; }
	/// <summary>
	///365,366 repeating
	/// </summary>
	public decimal DaysInCalYear { get; set; }
	/// <summary>
	///e.g. 355,364, repeating
	/// </summary>
	public decimal DaysInFisYear { get; set; }
	/// <summary>
	///last day of cal year
	/// </summary>
	public DateOnly EndOfCalYear { get; set; }
	/// <summary>
	///last day of fiscal year
	/// </summary>
	public DateOnly EndOfFisYear { get; set; }
}

public partial class TimeTable : TableBase<Time>
{
}