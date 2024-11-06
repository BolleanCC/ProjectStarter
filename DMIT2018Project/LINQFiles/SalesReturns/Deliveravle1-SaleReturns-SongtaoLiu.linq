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

public List<SaleRefundsView> GetSaleRefund(int saleRefundID)
{
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();

	// Validate the categoryID 
	if (saleRefundID <= 0)
	{
		errorList.Add(new ArgumentException("SaleRefund ID must be greater than zero."));
	}

	// Any errors in the error list, throw an AggregateException
	if (errorList.Count > 0)
	{
		throw new AggregateException("Invalid input provided. Please check the error messages.", errorList);
	}
	
	return SaleRefunds
				   .Where(sr => sr.SaleRefundID == saleRefundID)
				   .Select(sr => new SaleRefundsView 
				   {
				      SaleRefundID = sr.SaleRefundID,
					  SaleRefundDate = sr.SaleRefundDate,
					  SaleID = sr.SaleID,
					  EmployeeID = sr.EmployeeID,
					  TaxAmount = sr.TaxAmount,
					  SubTotal = sr.SubTotal,
					  RemoveFromViewFlag = sr.RemoveFromViewFlag					  
				   }).ToList();

}

public void SaveSales(SalesView salesView)
{

	List<Exception> errorList = new List<Exception>();

	// Validate 
	if (salesView == null)
	{
		errorList.Add(new ArgumentNullException("Sale cannot be null."));
	}
	else
	{
		if (salesView.EmployeeID <= 0)
		{
			errorList.Add(new ArgumentException("Invalid Employee ID."));
		}
		if (string.IsNullOrWhiteSpace(salesView.PaymentType))
		{
			errorList.Add(new ArgumentException("Payment type cannot be null or empty."));
		}
		if (salesView.SubTotal <= 0)
		{
			errorList.Add(new ArgumentException("SubTotal must be greater than zero."));
		}
	}

	// Validate sale details
	if (salesView.saleDetails == null || salesView.saleDetails.Count() == 0)
	{
		errorList.Add(new ArgumentException("Sale details cannot be null or empty."));
	}
	else
	{
		foreach (var saleDetail in salesView.saleDetails)
		{
			if (saleDetail.Quantity <= 0)
			{
				errorList.Add(new ArgumentException($"Invalid quantity for item ID {saleDetail.StockItemID}."));
			}
			if (saleDetail.SellingPrice <= 0)
			{
				errorList.Add(new ArgumentException($"Invalid selling price for item ID {saleDetail.StockItemID}."));
			}
		}
	}

	// Any errors, throw an AggregateException
	if (errorList.Count() > 0)
	{
		ChangeTracker.Clear();
		string errorMsg = "Unable to save.";
		errorMsg += " Please check error message(s)";
		throw new AggregateException(errorMsg, errorList);
	}

	// Save the sale and sale details to the database
	Sales sale = Sales.FirstOrDefault(s => s.SaleID == salesView.SaleID);
	if (sale == null)
	{
		sale = new Sales
		{
			SaleDate = DateTime.Now,
			EmployeeID = salesView.EmployeeID,
			PaymentType = salesView.PaymentType,
			TaxAmount = 0,
			SubTotal = 0,
			CouponID = salesView.CouponID,
			PaymentToken = salesView.PaymentToken,
			RemoveFromViewFlag = salesView.RemoveFromViewFlag
		};
	} 
	else
	{
		sale.SaleDate = salesView.SaleDate;
		sale.EmployeeID = salesView.EmployeeID;
		sale.PaymentType = salesView.PaymentType;
		sale.CouponID = salesView.CouponID;
		sale.PaymentToken = salesView.PaymentToken;
		sale.RemoveFromViewFlag = salesView.RemoveFromViewFlag;
	}

	// Process each sale detail
	foreach (var saleDetailView in salesView.saleDetails)
	{
		// Retrieve the sale detail from the database or create a new one if it doesn't exist
		SaleDetails saleDetail = SaleDetails
									.FirstOrDefault(x => x.SaleDetailID == saleDetailView.SaleDetailID
														 && x.StockItemID == saleDetailView.StockItemID);
		if (saleDetail == null)
		{
			saleDetail = new SaleDetails
			{
				StockItemID = saleDetailView.StockItemID,
				Quantity = saleDetailView.Quantity,
				SellingPrice = saleDetailView.SellingPrice
			};
			sale.SaleDetails.Add(saleDetail); // Add new sale details
		}
		else
		{
			// Update existing sale detail
			saleDetail.Quantity = saleDetailView.Quantity;
			saleDetail.SellingPrice = saleDetailView.SellingPrice;
		}

		// Update totals if the sale detail is valid
		sale.SubTotal += saleDetail.Quantity * saleDetail.SellingPrice;
		sale.TaxAmount += saleDetail.Quantity * saleDetail.SellingPrice * 0.05m; 
	}

	// Save changes to the database
	if (sale.SaleID == 0)
	{
		Sales.Add(sale); // Add new sale
	}
	else
	{
		Sales.Update(sale); // Update existing sale
	}

	SaveChanges(); 
	
}


public void SaveRefund(SaleRefundsView saleRefundsView)
{

	List<Exception> errorList = new List<Exception>();

	// Validate 
	if (saleRefundsView == null)
	{
		errorList.Add(new ArgumentNullException("SaleRefund cannot be null."));
	}
	else
	{
		if (saleRefundsView.SaleRefundID <= 0)
		{
			errorList.Add(new ArgumentException("Invalid SaleRefund ID."));
		}
		if (saleRefundsView.SaleID <= 0)
		{
			errorList.Add(new ArgumentException("Invalid Sale ID."));
		}
		if (saleRefundsView.EmployeeID <= 0)
		{
			errorList.Add(new ArgumentException("Invalid Employee ID."));
		}
		if (saleRefundsView.SubTotal <= 0)
		{
			errorList.Add(new ArgumentException("SubTotal must be greater than zero."));
		}
	}

	// Validate sale details
	if (saleRefundsView.saleRefundDetails == null || saleRefundsView.saleRefundDetails.Count() == 0)
	{
		errorList.Add(new ArgumentException("Sale refund details cannot be null or empty."));
	}
	else
	{
		foreach (var saleRefundDetail in saleRefundsView.saleRefundDetails)
		{
			if (saleRefundDetail.Quantity <= 0)
			{
				errorList.Add(new ArgumentException($"Invalid quantity for item ID {saleRefundDetail.StockItemID}."));
			}
			if (saleRefundDetail.SellingPrice <= 0)
			{
				errorList.Add(new ArgumentException($"Invalid selling price for item ID {saleRefundDetail.StockItemID}."));
			}
		}
	}

	// Any errors, throw an AggregateException
	if (errorList.Count() > 0)
	{
		ChangeTracker.Clear();
		string errorMsg = "Unable to save.";
		errorMsg += " Please check error message(s)";
		throw new AggregateException(errorMsg, errorList);
	}

	// Save the sale and sale details to the database
	SaleRefunds saleRefunds = SaleRefunds.FirstOrDefault(sr => sr.SaleRefundID == saleRefundsView.SaleRefundID);
	if (saleRefunds == null)
	{
		saleRefunds = new SaleRefunds
		{
			SaleRefundDate = DateTime.Now,
			SaleID = saleRefundsView.SaleID,
			EmployeeID = saleRefundsView.EmployeeID,
			TaxAmount = 0,
			SubTotal = 0,
			RemoveFromViewFlag = saleRefundsView.RemoveFromViewFlag
		};
	}
	else
	{
		saleRefunds.SaleRefundDate = saleRefundsView.SaleRefundDate;
		saleRefunds.EmployeeID = saleRefundsView.EmployeeID;
		saleRefunds.RemoveFromViewFlag = saleRefundsView.RemoveFromViewFlag;
	}

	// Process each sale refund detail
	foreach (var saleRefundDetailView in saleRefundsView.saleRefundDetails)
	{
		SaleRefundDetails saleRefundDetails = SaleRefundDetails
			.FirstOrDefault(x => x.SaleRefundDetailID == saleRefundDetailView.SaleRefundDetailID
								 && x.StockItemID == saleRefundDetailView.StockItemID);
		if (saleRefundDetails == null)
		{
			saleRefundDetails = new SaleRefundDetails
			{
				StockItemID = saleRefundDetailView.StockItemID,
				Quantity = saleRefundDetailView.Quantity,
				SellingPrice = saleRefundDetailView.SellingPrice
			};
			saleRefunds.SaleRefundDetails.Add(saleRefundDetails); // Add new sale refund details
		}
		else
		{
			// Update existing sale refund detail
			saleRefundDetails.Quantity = saleRefundDetailView.Quantity;
			saleRefundDetails.SellingPrice = saleRefundDetailView.SellingPrice;
		}

		// Update totals if the sale refund detail is valid
		saleRefunds.SubTotal += saleRefundDetails.Quantity * saleRefundDetails.SellingPrice;
		saleRefunds.TaxAmount += saleRefundDetails.Quantity * saleRefundDetails.SellingPrice * 0.05m; 
	}

	// Save changes to the database
	if (saleRefunds.SaleRefundID == 0)
	{
		SaleRefunds.Add(saleRefunds); // Add new sale refund
	}
	else
	{
		SaleRefunds.Update(saleRefunds); // Update existing sale refund
	}

	SaveChanges(); 

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
	public List<SaleDetailsView> saleDetails { get; set; } = new List<SaleDetailsView>();
} 

public class SaleDetailsView
{
	public int SaleDetailID { get; set; }
	public int SaleID { get; set; }
	public int StockItemID{ get; set; }
	public decimal SellingPrice { get; set; }
	public int Quantity { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}

public class SaleRefundsView
{
	public int SaleRefundID { get; set; }
	public DateTime SaleRefundDate { get; set; }
	public int SaleID { get; set; }
	public int EmployeeID { get; set; }
	public decimal TaxAmount { get; set; }
	public decimal SubTotal { get; set; }
	public bool RemoveFromViewFlag { get; set; }
	public List<SaleRefundDetailsView> saleRefundDetails { get; set; } = new List<SaleRefundDetailsView>();
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

