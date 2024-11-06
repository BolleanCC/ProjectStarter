<Query Kind="Program">
  <Connection>
    <ID>32451b4c-24be-4b68-aeb7-d6f1718a0935</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>SONGTAOLIU\SQLEXPRESS</Server>
    <Database>eTools2023</Database>
    <DisplayName>eTools2023-Entity</DisplayName>
    <NoCapitalization>true</NoCapitalization>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

//	Driver is responsible for orchestrating the flow by calling 
//	various methods and classes that contain the actual business logic 
//	or data processing operations.
void Main()
{
	
}
#region Test Methods

#endregion

//	This region contains all methods responsible 
//	for executing business logic and operations.
#region Methods

public List<CategoriesView> GetCategories(int categoryID)
{
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	// Validate the categoryID 
	if (categoryID <= 0)
	{
		errorList.Add(new ArgumentException("Category ID must be greater than zero."));
	}

	// Any errors in the error list, throw an AggregateException
	if (errorList.Count > 0)
	{
		throw new AggregateException("Invalid input provided. Please check the error messages.", errorList);
	}
	
	return Categories
				 .Where(c => c.CategoryID == categoryID)
				 .Select(c => new CategoriesView
				 {
				    CategoryID = c.CategoryID,
					Description = c.Description,
					RemoveFromViewFlag = c.RemoveFromViewFlag
				 }).ToList();
}


public List<StockItemsView> GetItemByCategoryID(int categoryID)
{
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	// Validate the categoryID 
	if (categoryID <= 0)
	{
		errorList.Add(new ArgumentException("Category ID must be greater than zero."));
	}

	// Any errors in the error list, throw an AggregateException
	if (errorList.Count > 0)
	{
		throw new AggregateException("Invalid input provided. Please check the error messages.", errorList);
	}

	return StockItems
				   .Where(si => si.CategoryID == categoryID)
				   .Select(si => new StockItemsView 
				   {
				      StockItemsID = si.StockItemID,
					  Description = si.Description,
					  SellingPrice = si.SellingPrice,
					  PurchasePrice = si.PurchasePrice,
					  QuantityOnHand = si.QuantityOnHand,
					  QuantityOnOrder = si.QuantityOnOrder,
					  ReOrderLevel = si.ReOrderLevel,
					  Discontinued = si.Discontinued,
					  VendorID = si.VendorID,
					  VendorStockNumber = si.VendorStockNumber,
					  CategoryID = si.CategoryID,
					  RemoveFromViewFlag = si.RemoveFromViewFlag					  
				   }).ToList();
}
#endregion


public Exception GetInnerException(System.Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}


//	This region includes the view models used to 
//	represent and structure data for the UI.
#region View Models
public class CategoriesView 
{
	public int CategoryID {get; set;}
	public string Description {get; set;}
	public bool RemoveFromViewFlag {get; set;}
}

public class StockItemsView 
{
	public int StockItemsID{ get; set; }
	public string Description { get; set; }
	public decimal SellingPrice { get; set; }
	public decimal PurchasePrice { get; set; }
	public int QuantityOnHand { get; set; }
	public int QuantityOnOrder{ get; set; }
	public int ReOrderLevel { get; set; }
	public bool Discontinued { get; set; }
	public int VendorID{ get; set; }
	public string VendorStockNumber { get; set; }
	public int CategoryID { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}

public class SalesView 
{
	public int SaleID { get; set; }
	public DateTime SaleDate { get; set; }
	public string PaymentType { get; set; }
	public int EmployeeID{ get; set; }
	public decimal TaxAmount { get; set; }
	public decimal SubTotal { get; set; }
	public int? CouponID { get; set; }
	public Guid? PaymentToken { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}

public class SaleDetailsView
{
	public int SaleDetailID { get; set; }
	public int SaleID { get; set; }
	public int StockIemID{ get; set; }
	public decimal SellingPrice { get; set; }
	public int Quantity { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}

public class SaleRefundView
{
	public int SaleRefundID { get; set; }
	public DateTime SaleRefundDate { get; set; }
	public int SaleID { get; set; }
	public int EmployeeID { get; set; }
	public decimal TaxAmount { get; set; }
	public decimal SubTotal { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}

public class SaleRefundDetailsView
{
	public int SaleRefundDetailID { get; set; }
	public int SaleRefundID { get; set; }
	public int StockItemID { get; set; }
	public decimal SellingPrice { get; set; }
	public int Quantity { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}
#endregion

