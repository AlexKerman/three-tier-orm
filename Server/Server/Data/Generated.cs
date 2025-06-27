using System;
using Grpc.Core;
using GrpcContracts;
using Oracle.ManagedDataAccess.Client;

namespace Server.Data;

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
public partial class Channel : EntityBase
{
	/// <summary>
	/// primary key column
	/// </summary>
	public int ChannelId;
	/// <summary>
	/// e.g. telesales, internet, catalog
	/// </summary>
	public string ChannelDesc;
	/// <summary>
	/// e.g. direct, indirect
	/// </summary>
	public string ChannelClass;
	public int ChannelClassId;
	public string ChannelTotal;
	public int ChannelTotalId;

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


	public override void LoadFromReader(FieldReader fr, string tableName)
	{
		fr.Read(ref ChannelId, tableName + "_CHANNEL_ID");
		fr.Read(ref ChannelDesc, tableName + "_CHANNEL_DESC");
		fr.Read(ref ChannelClass, tableName + "_CHANNEL_CLASS");
		fr.Read(ref ChannelClassId, tableName + "_CHANNEL_CLASS_ID");
		fr.Read(ref ChannelTotal, tableName + "_CHANNEL_TOTAL");
		fr.Read(ref ChannelTotalId, tableName + "_CHANNEL_TOTAL_ID");
	}
}

public partial class ChannelTable : TableBase<Channel>
{
	public override string TableDbName => "CHANNELS";
	public override string SchemaDbName => "SH";
	public override Dictionary<string, string> PropertiesToFields => new()
	{
		{ "ChannelId", "CHANNEL_ID" },
		{ "ChannelDesc", "CHANNEL_DESC" },
		{ "ChannelClass", "CHANNEL_CLASS" },
		{ "ChannelClassId", "CHANNEL_CLASS_ID" },
		{ "ChannelTotal", "CHANNEL_TOTAL" },
		{ "ChannelTotalId", "CHANNEL_TOTAL_ID" },
	};
}

public partial class Cost : EntityBase
{
	public int ProdId;
	public DateOnly TimeId;
	public int PromoId;
	public int ChannelId;
	public decimal UnitCost;
	public decimal UnitPrice;
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


	public override void LoadFromReader(FieldReader fr, string tableName)
	{
		fr.Read(ref ProdId, tableName + "_PROD_ID");
		fr.Read(ref TimeId, tableName + "_TIME_ID");
		fr.Read(ref PromoId, tableName + "_PROMO_ID");
		fr.Read(ref ChannelId, tableName + "_CHANNEL_ID");
		fr.Read(ref UnitCost, tableName + "_UNIT_COST");
		fr.Read(ref UnitPrice, tableName + "_UNIT_PRICE");
	}
}

public partial class CostTable : TableBase<Cost>
{
	public override string TableDbName => "COSTS";
	public override string SchemaDbName => "SH";
	public override Dictionary<string, string> PropertiesToFields => new()
	{
		{ "ProdId", "PROD_ID" },
		{ "TimeId", "TIME_ID" },
		{ "PromoId", "PROMO_ID" },
		{ "ChannelId", "CHANNEL_ID" },
		{ "UnitCost", "UNIT_COST" },
		{ "UnitPrice", "UNIT_PRICE" },
	};
}

/// <summary>
/// country dimension table (snowflake)
/// </summary>
public partial class Country : EntityBase
{
	/// <summary>
	/// primary key
	/// </summary>
	public int CountryId;
	public string CountryIsoCode;
	/// <summary>
	/// country name
	/// </summary>
	public string CountryName;
	/// <summary>
	/// e.g. Western Europe, to allow hierarchies
	/// </summary>
	public string CountrySubregion;
	public int CountrySubregionId;
	/// <summary>
	/// e.g. Europe, Asia
	/// </summary>
	public string CountryRegion;
	public int CountryRegionId;
	public string CountryTotal;
	public int CountryTotalId;

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


	public override void LoadFromReader(FieldReader fr, string tableName)
	{
		fr.Read(ref CountryId, tableName + "_COUNTRY_ID");
		fr.Read(ref CountryIsoCode, tableName + "_COUNTRY_ISO_CODE");
		fr.Read(ref CountryName, tableName + "_COUNTRY_NAME");
		fr.Read(ref CountrySubregion, tableName + "_COUNTRY_SUBREGION");
		fr.Read(ref CountrySubregionId, tableName + "_COUNTRY_SUBREGION_ID");
		fr.Read(ref CountryRegion, tableName + "_COUNTRY_REGION");
		fr.Read(ref CountryRegionId, tableName + "_COUNTRY_REGION_ID");
		fr.Read(ref CountryTotal, tableName + "_COUNTRY_TOTAL");
		fr.Read(ref CountryTotalId, tableName + "_COUNTRY_TOTAL_ID");
	}
}

public partial class CountryTable : TableBase<Country>
{
	public override string TableDbName => "COUNTRIES";
	public override string SchemaDbName => "SH";
	public override Dictionary<string, string> PropertiesToFields => new()
	{
		{ "CountryId", "COUNTRY_ID" },
		{ "CountryIsoCode", "COUNTRY_ISO_CODE" },
		{ "CountryName", "COUNTRY_NAME" },
		{ "CountrySubregion", "COUNTRY_SUBREGION" },
		{ "CountrySubregionId", "COUNTRY_SUBREGION_ID" },
		{ "CountryRegion", "COUNTRY_REGION" },
		{ "CountryRegionId", "COUNTRY_REGION_ID" },
		{ "CountryTotal", "COUNTRY_TOTAL" },
		{ "CountryTotalId", "COUNTRY_TOTAL_ID" },
	};
}

/// <summary>
/// dimension table
/// </summary>
public partial class Customer : EntityBase
{
	/// <summary>
	/// primary key
	/// </summary>
	public int CustId;
	/// <summary>
	/// first name of the customer
	/// </summary>
	public string CustFirstName;
	/// <summary>
	/// last name of the customer
	/// </summary>
	public string CustLastName;
	/// <summary>
	/// gender; low cardinality attribute
	/// </summary>
	public string CustGender;
	/// <summary>
	/// customer year of birth
	/// </summary>
	public int CustYearOfBirth;
	/// <summary>
	/// customer marital status; low cardinality attribute
	/// </summary>
	public string? CustMaritalStatus;
	/// <summary>
	/// customer street address
	/// </summary>
	public string CustStreetAddress;
	/// <summary>
	/// postal code of the customer
	/// </summary>
	public string CustPostalCode;
	/// <summary>
	/// city where the customer lives
	/// </summary>
	public string CustCity;
	public int CustCityId;
	/// <summary>
	/// customer geography: state or province
	/// </summary>
	public string CustStateProvince;
	public int CustStateProvinceId;
	/// <summary>
	/// foreign key to the countries table (snowflake)
	/// </summary>
	public int CountryId;
	/// <summary>
	/// customer main phone number
	/// </summary>
	public string CustMainPhoneNumber;
	/// <summary>
	/// customer income level
	/// </summary>
	public string? CustIncomeLevel;
	/// <summary>
	/// customer credit limit
	/// </summary>
	public decimal? CustCreditLimit;
	/// <summary>
	/// customer email id
	/// </summary>
	public string? CustEmail;
	public string CustTotal;
	public int CustTotalId;
	public int? CustSrcId;
	public DateOnly? CustEffFrom;
	public DateOnly? CustEffTo;
	public string? CustValid;
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


	public override void LoadFromReader(FieldReader fr, string tableName)
	{
		fr.Read(ref CustId, tableName + "_CUST_ID");
		fr.Read(ref CustFirstName, tableName + "_CUST_FIRST_NAME");
		fr.Read(ref CustLastName, tableName + "_CUST_LAST_NAME");
		fr.Read(ref CustGender, tableName + "_CUST_GENDER");
		fr.Read(ref CustYearOfBirth, tableName + "_CUST_YEAR_OF_BIRTH");
		fr.Read(ref CustMaritalStatus, tableName + "_CUST_MARITAL_STATUS");
		fr.Read(ref CustStreetAddress, tableName + "_CUST_STREET_ADDRESS");
		fr.Read(ref CustPostalCode, tableName + "_CUST_POSTAL_CODE");
		fr.Read(ref CustCity, tableName + "_CUST_CITY");
		fr.Read(ref CustCityId, tableName + "_CUST_CITY_ID");
		fr.Read(ref CustStateProvince, tableName + "_CUST_STATE_PROVINCE");
		fr.Read(ref CustStateProvinceId, tableName + "_CUST_STATE_PROVINCE_ID");
		fr.Read(ref CountryId, tableName + "_COUNTRY_ID");
		fr.Read(ref CustMainPhoneNumber, tableName + "_CUST_MAIN_PHONE_NUMBER");
		fr.Read(ref CustIncomeLevel, tableName + "_CUST_INCOME_LEVEL");
		fr.Read(ref CustCreditLimit, tableName + "_CUST_CREDIT_LIMIT");
		fr.Read(ref CustEmail, tableName + "_CUST_EMAIL");
		fr.Read(ref CustTotal, tableName + "_CUST_TOTAL");
		fr.Read(ref CustTotalId, tableName + "_CUST_TOTAL_ID");
		fr.Read(ref CustSrcId, tableName + "_CUST_SRC_ID");
		fr.Read(ref CustEffFrom, tableName + "_CUST_EFF_FROM");
		fr.Read(ref CustEffTo, tableName + "_CUST_EFF_TO");
		fr.Read(ref CustValid, tableName + "_CUST_VALID");
	}
}

public partial class CustomerTable : TableBase<Customer>
{
	public override string TableDbName => "CUSTOMERS";
	public override string SchemaDbName => "SH";
	public override Dictionary<string, string> PropertiesToFields => new()
	{
		{ "CustId", "CUST_ID" },
		{ "CustFirstName", "CUST_FIRST_NAME" },
		{ "CustLastName", "CUST_LAST_NAME" },
		{ "CustGender", "CUST_GENDER" },
		{ "CustYearOfBirth", "CUST_YEAR_OF_BIRTH" },
		{ "CustMaritalStatus", "CUST_MARITAL_STATUS" },
		{ "CustStreetAddress", "CUST_STREET_ADDRESS" },
		{ "CustPostalCode", "CUST_POSTAL_CODE" },
		{ "CustCity", "CUST_CITY" },
		{ "CustCityId", "CUST_CITY_ID" },
		{ "CustStateProvince", "CUST_STATE_PROVINCE" },
		{ "CustStateProvinceId", "CUST_STATE_PROVINCE_ID" },
		{ "CountryId", "COUNTRY_ID" },
		{ "CustMainPhoneNumber", "CUST_MAIN_PHONE_NUMBER" },
		{ "CustIncomeLevel", "CUST_INCOME_LEVEL" },
		{ "CustCreditLimit", "CUST_CREDIT_LIMIT" },
		{ "CustEmail", "CUST_EMAIL" },
		{ "CustTotal", "CUST_TOTAL" },
		{ "CustTotalId", "CUST_TOTAL_ID" },
		{ "CustSrcId", "CUST_SRC_ID" },
		{ "CustEffFrom", "CUST_EFF_FROM" },
		{ "CustEffTo", "CUST_EFF_TO" },
		{ "CustValid", "CUST_VALID" },
	};
}

/// <summary>
/// dimension table
/// </summary>
public partial class Product : EntityBase
{
	/// <summary>
	/// primary key
	/// </summary>
	public int ProdId;
	/// <summary>
	/// product name
	/// </summary>
	public string ProdName;
	/// <summary>
	/// product description
	/// </summary>
	public string ProdDesc;
	/// <summary>
	/// product subcategory
	/// </summary>
	public string ProdSubcategory;
	public int ProdSubcategoryId;
	/// <summary>
	/// product subcategory description
	/// </summary>
	public string ProdSubcategoryDesc;
	/// <summary>
	/// product category
	/// </summary>
	public string ProdCategory;
	public int ProdCategoryId;
	/// <summary>
	/// product category description
	/// </summary>
	public string ProdCategoryDesc;
	/// <summary>
	/// product weight class
	/// </summary>
	public int ProdWeightClass;
	/// <summary>
	/// product unit of measure
	/// </summary>
	public string? ProdUnitOfMeasure;
	/// <summary>
	/// product package size
	/// </summary>
	public string ProdPackSize;
	/// <summary>
	/// this column
	/// </summary>
	public int SupplierId;
	/// <summary>
	/// product status
	/// </summary>
	public string ProdStatus;
	/// <summary>
	/// product list price
	/// </summary>
	public decimal ProdListPrice;
	/// <summary>
	/// product minimum price
	/// </summary>
	public decimal ProdMinPrice;
	public string ProdTotal;
	public int ProdTotalId;
	public int? ProdSrcId;
	public DateOnly? ProdEffFrom;
	public DateOnly? ProdEffTo;
	public string? ProdValid;

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


	public override void LoadFromReader(FieldReader fr, string tableName)
	{
		fr.Read(ref ProdId, tableName + "_PROD_ID");
		fr.Read(ref ProdName, tableName + "_PROD_NAME");
		fr.Read(ref ProdDesc, tableName + "_PROD_DESC");
		fr.Read(ref ProdSubcategory, tableName + "_PROD_SUBCATEGORY");
		fr.Read(ref ProdSubcategoryId, tableName + "_PROD_SUBCATEGORY_ID");
		fr.Read(ref ProdSubcategoryDesc, tableName + "_PROD_SUBCATEGORY_DESC");
		fr.Read(ref ProdCategory, tableName + "_PROD_CATEGORY");
		fr.Read(ref ProdCategoryId, tableName + "_PROD_CATEGORY_ID");
		fr.Read(ref ProdCategoryDesc, tableName + "_PROD_CATEGORY_DESC");
		fr.Read(ref ProdWeightClass, tableName + "_PROD_WEIGHT_CLASS");
		fr.Read(ref ProdUnitOfMeasure, tableName + "_PROD_UNIT_OF_MEASURE");
		fr.Read(ref ProdPackSize, tableName + "_PROD_PACK_SIZE");
		fr.Read(ref SupplierId, tableName + "_SUPPLIER_ID");
		fr.Read(ref ProdStatus, tableName + "_PROD_STATUS");
		fr.Read(ref ProdListPrice, tableName + "_PROD_LIST_PRICE");
		fr.Read(ref ProdMinPrice, tableName + "_PROD_MIN_PRICE");
		fr.Read(ref ProdTotal, tableName + "_PROD_TOTAL");
		fr.Read(ref ProdTotalId, tableName + "_PROD_TOTAL_ID");
		fr.Read(ref ProdSrcId, tableName + "_PROD_SRC_ID");
		fr.Read(ref ProdEffFrom, tableName + "_PROD_EFF_FROM");
		fr.Read(ref ProdEffTo, tableName + "_PROD_EFF_TO");
		fr.Read(ref ProdValid, tableName + "_PROD_VALID");
	}
}

public partial class ProductTable : TableBase<Product>
{
	public override string TableDbName => "PRODUCTS";
	public override string SchemaDbName => "SH";
	public override Dictionary<string, string> PropertiesToFields => new()
	{
		{ "ProdId", "PROD_ID" },
		{ "ProdName", "PROD_NAME" },
		{ "ProdDesc", "PROD_DESC" },
		{ "ProdSubcategory", "PROD_SUBCATEGORY" },
		{ "ProdSubcategoryId", "PROD_SUBCATEGORY_ID" },
		{ "ProdSubcategoryDesc", "PROD_SUBCATEGORY_DESC" },
		{ "ProdCategory", "PROD_CATEGORY" },
		{ "ProdCategoryId", "PROD_CATEGORY_ID" },
		{ "ProdCategoryDesc", "PROD_CATEGORY_DESC" },
		{ "ProdWeightClass", "PROD_WEIGHT_CLASS" },
		{ "ProdUnitOfMeasure", "PROD_UNIT_OF_MEASURE" },
		{ "ProdPackSize", "PROD_PACK_SIZE" },
		{ "SupplierId", "SUPPLIER_ID" },
		{ "ProdStatus", "PROD_STATUS" },
		{ "ProdListPrice", "PROD_LIST_PRICE" },
		{ "ProdMinPrice", "PROD_MIN_PRICE" },
		{ "ProdTotal", "PROD_TOTAL" },
		{ "ProdTotalId", "PROD_TOTAL_ID" },
		{ "ProdSrcId", "PROD_SRC_ID" },
		{ "ProdEffFrom", "PROD_EFF_FROM" },
		{ "ProdEffTo", "PROD_EFF_TO" },
		{ "ProdValid", "PROD_VALID" },
	};
}

/// <summary>
/// dimension table without a PK-FK relationship with the facts table, to show outer join functionality
/// </summary>
public partial class Promotion : EntityBase
{
	/// <summary>
	/// primary key column
	/// </summary>
	public int PromoId;
	/// <summary>
	/// promotion description
	/// </summary>
	public string PromoName;
	/// <summary>
	/// enables to investigate promotion hierarchies
	/// </summary>
	public string PromoSubcategory;
	public int PromoSubcategoryId;
	/// <summary>
	/// promotion category
	/// </summary>
	public string PromoCategory;
	public int PromoCategoryId;
	/// <summary>
	/// promotion cost, to do promotion effect calculations
	/// </summary>
	public decimal PromoCost;
	/// <summary>
	/// promotion begin day
	/// </summary>
	public DateOnly PromoBeginDate;
	/// <summary>
	/// promotion end day
	/// </summary>
	public DateOnly PromoEndDate;
	public string PromoTotal;
	public int PromoTotalId;

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


	public override void LoadFromReader(FieldReader fr, string tableName)
	{
		fr.Read(ref PromoId, tableName + "_PROMO_ID");
		fr.Read(ref PromoName, tableName + "_PROMO_NAME");
		fr.Read(ref PromoSubcategory, tableName + "_PROMO_SUBCATEGORY");
		fr.Read(ref PromoSubcategoryId, tableName + "_PROMO_SUBCATEGORY_ID");
		fr.Read(ref PromoCategory, tableName + "_PROMO_CATEGORY");
		fr.Read(ref PromoCategoryId, tableName + "_PROMO_CATEGORY_ID");
		fr.Read(ref PromoCost, tableName + "_PROMO_COST");
		fr.Read(ref PromoBeginDate, tableName + "_PROMO_BEGIN_DATE");
		fr.Read(ref PromoEndDate, tableName + "_PROMO_END_DATE");
		fr.Read(ref PromoTotal, tableName + "_PROMO_TOTAL");
		fr.Read(ref PromoTotalId, tableName + "_PROMO_TOTAL_ID");
	}
}

public partial class PromotionTable : TableBase<Promotion>
{
	public override string TableDbName => "PROMOTIONS";
	public override string SchemaDbName => "SH";
	public override Dictionary<string, string> PropertiesToFields => new()
	{
		{ "PromoId", "PROMO_ID" },
		{ "PromoName", "PROMO_NAME" },
		{ "PromoSubcategory", "PROMO_SUBCATEGORY" },
		{ "PromoSubcategoryId", "PROMO_SUBCATEGORY_ID" },
		{ "PromoCategory", "PROMO_CATEGORY" },
		{ "PromoCategoryId", "PROMO_CATEGORY_ID" },
		{ "PromoCost", "PROMO_COST" },
		{ "PromoBeginDate", "PROMO_BEGIN_DATE" },
		{ "PromoEndDate", "PROMO_END_DATE" },
		{ "PromoTotal", "PROMO_TOTAL" },
		{ "PromoTotalId", "PROMO_TOTAL_ID" },
	};
}

/// <summary>
/// facts table, without a primary key; all rows are uniquely identified by the combination of all foreign keys
/// </summary>
public partial class Sale : EntityBase
{
	/// <summary>
	/// FK to the products dimension table
	/// </summary>
	public int ProdId;
	/// <summary>
	/// FK to the customers dimension table
	/// </summary>
	public int CustId;
	/// <summary>
	/// FK to the times dimension table
	/// </summary>
	public DateOnly TimeId;
	/// <summary>
	/// FK to the channels dimension table
	/// </summary>
	public int ChannelId;
	/// <summary>
	/// promotion identifier, without FK constraint (intentionally) to show outer join optimization
	/// </summary>
	public int PromoId;
	/// <summary>
	/// product quantity sold with the transaction
	/// </summary>
	public int QuantitySold;
	/// <summary>
	/// invoiced amount to the customer
	/// </summary>
	public decimal AmountSold;
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


	public override void LoadFromReader(FieldReader fr, string tableName)
	{
		fr.Read(ref ProdId, tableName + "_PROD_ID");
		fr.Read(ref CustId, tableName + "_CUST_ID");
		fr.Read(ref TimeId, tableName + "_TIME_ID");
		fr.Read(ref ChannelId, tableName + "_CHANNEL_ID");
		fr.Read(ref PromoId, tableName + "_PROMO_ID");
		fr.Read(ref QuantitySold, tableName + "_QUANTITY_SOLD");
		fr.Read(ref AmountSold, tableName + "_AMOUNT_SOLD");
	}
}

public partial class SaleTable : TableBase<Sale>
{
	public override string TableDbName => "SALES";
	public override string SchemaDbName => "SH";
	public override Dictionary<string, string> PropertiesToFields => new()
	{
		{ "ProdId", "PROD_ID" },
		{ "CustId", "CUST_ID" },
		{ "TimeId", "TIME_ID" },
		{ "ChannelId", "CHANNEL_ID" },
		{ "PromoId", "PROMO_ID" },
		{ "QuantitySold", "QUANTITY_SOLD" },
		{ "AmountSold", "AMOUNT_SOLD" },
	};
}

/// <summary>
/// Time dimension table to support multiple hierarchies and materialized views
/// </summary>
public partial class Time : EntityBase
{
	/// <summary>
	/// primary key; day date, finest granularity, CORRECT ORDER
	/// </summary>
	public DateOnly TimeId;
	/// <summary>
	/// Monday to Sunday, repeating
	/// </summary>
	public string DayName;
	/// <summary>
	/// 1 to 7, repeating
	/// </summary>
	public int DayNumberInWeek;
	/// <summary>
	/// 1 to 31, repeating
	/// </summary>
	public int DayNumberInMonth;
	/// <summary>
	/// 1 to 53, repeating
	/// </summary>
	public int CalendarWeekNumber;
	/// <summary>
	/// 1 to 53, repeating
	/// </summary>
	public int FiscalWeekNumber;
	/// <summary>
	/// date of last day in week, CORRECT ORDER
	/// </summary>
	public DateOnly WeekEndingDay;
	public int WeekEndingDayId;
	/// <summary>
	/// 1 to 12, repeating
	/// </summary>
	public int CalendarMonthNumber;
	/// <summary>
	/// 1 to 12, repeating
	/// </summary>
	public int FiscalMonthNumber;
	/// <summary>
	/// e.g. 1998-01, CORRECT ORDER
	/// </summary>
	public string CalendarMonthDesc;
	public int CalendarMonthId;
	/// <summary>
	/// e.g. 1998-01, CORRECT ORDER
	/// </summary>
	public string FiscalMonthDesc;
	public int FiscalMonthId;
	/// <summary>
	/// e.g. 28,31, repeating
	/// </summary>
	public decimal DaysInCalMonth;
	/// <summary>
	/// e.g. 25,32, repeating
	/// </summary>
	public decimal DaysInFisMonth;
	/// <summary>
	/// last day of calendar month
	/// </summary>
	public DateOnly EndOfCalMonth;
	/// <summary>
	/// last day of fiscal month
	/// </summary>
	public DateOnly EndOfFisMonth;
	/// <summary>
	/// January to December, repeating
	/// </summary>
	public string CalendarMonthName;
	/// <summary>
	/// January to December, repeating
	/// </summary>
	public string FiscalMonthName;
	/// <summary>
	/// e.g. 1998-Q1, CORRECT ORDER
	/// </summary>
	public string CalendarQuarterDesc;
	public int CalendarQuarterId;
	/// <summary>
	/// e.g. 1999-Q3, CORRECT ORDER
	/// </summary>
	public string FiscalQuarterDesc;
	public int FiscalQuarterId;
	/// <summary>
	/// e.g. 88,90, repeating
	/// </summary>
	public decimal DaysInCalQuarter;
	/// <summary>
	/// e.g. 88,90, repeating
	/// </summary>
	public decimal DaysInFisQuarter;
	/// <summary>
	/// last day of calendar quarter
	/// </summary>
	public DateOnly EndOfCalQuarter;
	/// <summary>
	/// last day of fiscal quarter
	/// </summary>
	public DateOnly EndOfFisQuarter;
	/// <summary>
	/// 1 to 4, repeating
	/// </summary>
	public int CalendarQuarterNumber;
	/// <summary>
	/// 1 to 4, repeating
	/// </summary>
	public int FiscalQuarterNumber;
	/// <summary>
	/// e.g. 1999, CORRECT ORDER
	/// </summary>
	public int CalendarYear;
	public int CalendarYearId;
	/// <summary>
	/// e.g. 1999, CORRECT ORDER
	/// </summary>
	public int FiscalYear;
	public int FiscalYearId;
	/// <summary>
	/// 365,366 repeating
	/// </summary>
	public decimal DaysInCalYear;
	/// <summary>
	/// e.g. 355,364, repeating
	/// </summary>
	public decimal DaysInFisYear;
	/// <summary>
	/// last day of cal year
	/// </summary>
	public DateOnly EndOfCalYear;
	/// <summary>
	/// last day of fiscal year
	/// </summary>
	public DateOnly EndOfFisYear;

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


	public override void LoadFromReader(FieldReader fr, string tableName)
	{
		fr.Read(ref TimeId, tableName + "_TIME_ID");
		fr.Read(ref DayName, tableName + "_DAY_NAME");
		fr.Read(ref DayNumberInWeek, tableName + "_DAY_NUMBER_IN_WEEK");
		fr.Read(ref DayNumberInMonth, tableName + "_DAY_NUMBER_IN_MONTH");
		fr.Read(ref CalendarWeekNumber, tableName + "_CALENDAR_WEEK_NUMBER");
		fr.Read(ref FiscalWeekNumber, tableName + "_FISCAL_WEEK_NUMBER");
		fr.Read(ref WeekEndingDay, tableName + "_WEEK_ENDING_DAY");
		fr.Read(ref WeekEndingDayId, tableName + "_WEEK_ENDING_DAY_ID");
		fr.Read(ref CalendarMonthNumber, tableName + "_CALENDAR_MONTH_NUMBER");
		fr.Read(ref FiscalMonthNumber, tableName + "_FISCAL_MONTH_NUMBER");
		fr.Read(ref CalendarMonthDesc, tableName + "_CALENDAR_MONTH_DESC");
		fr.Read(ref CalendarMonthId, tableName + "_CALENDAR_MONTH_ID");
		fr.Read(ref FiscalMonthDesc, tableName + "_FISCAL_MONTH_DESC");
		fr.Read(ref FiscalMonthId, tableName + "_FISCAL_MONTH_ID");
		fr.Read(ref DaysInCalMonth, tableName + "_DAYS_IN_CAL_MONTH");
		fr.Read(ref DaysInFisMonth, tableName + "_DAYS_IN_FIS_MONTH");
		fr.Read(ref EndOfCalMonth, tableName + "_END_OF_CAL_MONTH");
		fr.Read(ref EndOfFisMonth, tableName + "_END_OF_FIS_MONTH");
		fr.Read(ref CalendarMonthName, tableName + "_CALENDAR_MONTH_NAME");
		fr.Read(ref FiscalMonthName, tableName + "_FISCAL_MONTH_NAME");
		fr.Read(ref CalendarQuarterDesc, tableName + "_CALENDAR_QUARTER_DESC");
		fr.Read(ref CalendarQuarterId, tableName + "_CALENDAR_QUARTER_ID");
		fr.Read(ref FiscalQuarterDesc, tableName + "_FISCAL_QUARTER_DESC");
		fr.Read(ref FiscalQuarterId, tableName + "_FISCAL_QUARTER_ID");
		fr.Read(ref DaysInCalQuarter, tableName + "_DAYS_IN_CAL_QUARTER");
		fr.Read(ref DaysInFisQuarter, tableName + "_DAYS_IN_FIS_QUARTER");
		fr.Read(ref EndOfCalQuarter, tableName + "_END_OF_CAL_QUARTER");
		fr.Read(ref EndOfFisQuarter, tableName + "_END_OF_FIS_QUARTER");
		fr.Read(ref CalendarQuarterNumber, tableName + "_CALENDAR_QUARTER_NUMBER");
		fr.Read(ref FiscalQuarterNumber, tableName + "_FISCAL_QUARTER_NUMBER");
		fr.Read(ref CalendarYear, tableName + "_CALENDAR_YEAR");
		fr.Read(ref CalendarYearId, tableName + "_CALENDAR_YEAR_ID");
		fr.Read(ref FiscalYear, tableName + "_FISCAL_YEAR");
		fr.Read(ref FiscalYearId, tableName + "_FISCAL_YEAR_ID");
		fr.Read(ref DaysInCalYear, tableName + "_DAYS_IN_CAL_YEAR");
		fr.Read(ref DaysInFisYear, tableName + "_DAYS_IN_FIS_YEAR");
		fr.Read(ref EndOfCalYear, tableName + "_END_OF_CAL_YEAR");
		fr.Read(ref EndOfFisYear, tableName + "_END_OF_FIS_YEAR");
	}
}

public partial class TimeTable : TableBase<Time>
{
	public override string TableDbName => "TIMES";
	public override string SchemaDbName => "SH";
	public override Dictionary<string, string> PropertiesToFields => new()
	{
		{ "TimeId", "TIME_ID" },
		{ "DayName", "DAY_NAME" },
		{ "DayNumberInWeek", "DAY_NUMBER_IN_WEEK" },
		{ "DayNumberInMonth", "DAY_NUMBER_IN_MONTH" },
		{ "CalendarWeekNumber", "CALENDAR_WEEK_NUMBER" },
		{ "FiscalWeekNumber", "FISCAL_WEEK_NUMBER" },
		{ "WeekEndingDay", "WEEK_ENDING_DAY" },
		{ "WeekEndingDayId", "WEEK_ENDING_DAY_ID" },
		{ "CalendarMonthNumber", "CALENDAR_MONTH_NUMBER" },
		{ "FiscalMonthNumber", "FISCAL_MONTH_NUMBER" },
		{ "CalendarMonthDesc", "CALENDAR_MONTH_DESC" },
		{ "CalendarMonthId", "CALENDAR_MONTH_ID" },
		{ "FiscalMonthDesc", "FISCAL_MONTH_DESC" },
		{ "FiscalMonthId", "FISCAL_MONTH_ID" },
		{ "DaysInCalMonth", "DAYS_IN_CAL_MONTH" },
		{ "DaysInFisMonth", "DAYS_IN_FIS_MONTH" },
		{ "EndOfCalMonth", "END_OF_CAL_MONTH" },
		{ "EndOfFisMonth", "END_OF_FIS_MONTH" },
		{ "CalendarMonthName", "CALENDAR_MONTH_NAME" },
		{ "FiscalMonthName", "FISCAL_MONTH_NAME" },
		{ "CalendarQuarterDesc", "CALENDAR_QUARTER_DESC" },
		{ "CalendarQuarterId", "CALENDAR_QUARTER_ID" },
		{ "FiscalQuarterDesc", "FISCAL_QUARTER_DESC" },
		{ "FiscalQuarterId", "FISCAL_QUARTER_ID" },
		{ "DaysInCalQuarter", "DAYS_IN_CAL_QUARTER" },
		{ "DaysInFisQuarter", "DAYS_IN_FIS_QUARTER" },
		{ "EndOfCalQuarter", "END_OF_CAL_QUARTER" },
		{ "EndOfFisQuarter", "END_OF_FIS_QUARTER" },
		{ "CalendarQuarterNumber", "CALENDAR_QUARTER_NUMBER" },
		{ "FiscalQuarterNumber", "FISCAL_QUARTER_NUMBER" },
		{ "CalendarYear", "CALENDAR_YEAR" },
		{ "CalendarYearId", "CALENDAR_YEAR_ID" },
		{ "FiscalYear", "FISCAL_YEAR" },
		{ "FiscalYearId", "FISCAL_YEAR_ID" },
		{ "DaysInCalYear", "DAYS_IN_CAL_YEAR" },
		{ "DaysInFisYear", "DAYS_IN_FIS_YEAR" },
		{ "EndOfCalYear", "END_OF_CAL_YEAR" },
		{ "EndOfFisYear", "END_OF_FIS_YEAR" },
	};
}