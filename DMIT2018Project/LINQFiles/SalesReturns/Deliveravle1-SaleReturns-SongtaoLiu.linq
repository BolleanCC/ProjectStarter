<Query Kind="Program">
  <Connection>
    <ID>244a53c2-9e90-46b1-8d14-62bf20573a96</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>SONGTAOLIU\SQLEXPRESS</Server>
    <Database>eTools2023</Database>
    <DisplayName>eTools2023-Entity</DisplayName>
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
	//	// Placeholder for pass or fail
		string passFail = string.Empty;
		
		#region Get Categories (TestGetCategories)
		// Header information
		Console.WriteLine("===================================");
		Console.WriteLine("           Get Categories          ");
		Console.WriteLine("===================================");
		Console.WriteLine();
		
		// Pass: Valid Category ID
		Console.WriteLine("----- Test: Valid Category ID -----");
		var categories = TestGetCategories(1);
		passFail = categories.Count() > 0 ? "Pass" : "Fail";
		Console.WriteLine($"{passFail} - Expected {categories.Count()} categories, found {categories.Count()}");
		// Fail: Invalid Category ID
		Console.WriteLine();
		Console.WriteLine("----- Test: Invalid Category ID -----");
		categories = TestGetCategories(-1);
		passFail = categories == null ? "Pass" : "Fail";
		Console.WriteLine($"{passFail} - Invalid category ID should return null");
		Console.WriteLine("                                   ");
		#endregion
		
		#region Get Items by Category (TestGetItemByCategoryID)
		// Header information
		Console.WriteLine("===================================");
		Console.WriteLine("     Get Items by Category ID      ");
		Console.WriteLine("===================================");
		Console.WriteLine();
		
		// Pass: Valid Category ID
		Console.WriteLine("----- Test: Valid Category ID -----");
		var items = TestGetItemByCategoryID(1);
		passFail = items.Count() > 0 ? "Pass" : "Fail";
		Console.WriteLine($"{passFail} - Expected {items.Count()} items, found {items.Count()}");
		// Fail: Invalid Category ID
		Console.WriteLine();
		Console.WriteLine("----- Test: Invalid Category ID -----");
		items = TestGetItemByCategoryID(-1);
		passFail = items == null ? "Pass" : "Fail";
		Console.WriteLine($"{passFail} - Invalid category ID should return null");
		Console.WriteLine("                                   ");
		#endregion
		
		#region Get Sale Refund (TestGetSaleRefund)
		// Header information
		Console.WriteLine("===================================");
		Console.WriteLine("           Get Sale Refund         ");
		Console.WriteLine("===================================");
		Console.WriteLine();
		
		// Pass: Valid Sale Refund ID
		Console.WriteLine("----- Test: Valid Sale Refund ID -----");
		var saleRefunds = TestGetSaleRefund(1);
		passFail = saleRefunds.Count() > 0 ? "Pass" : "Fail";
		Console.WriteLine($"{passFail} - Expected {saleRefunds.Count()} sale refunds, found {saleRefunds.Count()}");
		
		// Fail: Invalid Sale Refund ID
		Console.WriteLine();
		Console.WriteLine("----- Test: Invalid Sale Refund ID -----");
		saleRefunds = TestGetSaleRefund(0);
		passFail = saleRefunds == null ? "Pass" : "Fail";
		Console.WriteLine($"{passFail} - Sale refund ID cannot be 0 or invalid");
		Console.WriteLine("                                    ");
		#endregion
	
		#region Save Sales (TestSaveSales)
		// Header information
		Console.WriteLine("===================================");
		Console.WriteLine("              Save Sales           ");
		Console.WriteLine("===================================");
	
		// Use a valid StockItemID from the StockItems table
		var stockItem = StockItems.FirstOrDefault();
		if (stockItem == null)
		{
			Console.WriteLine("Fail - No valid StockItem found in the StockItems table.");
			return;
		}
	
		// Use the SellingPrice from the StockItem
		var sellingPrice = stockItem.SellingPrice;
		var quantity = 2; 
		var subTotal = sellingPrice * quantity;
	
		// Dump the tables before saving
		Console.WriteLine("----- Sales Before Save -----");
		Sales.Dump("Sales Table Before Save");
		SaleDetails.Dump("SaleDetails Table Before Save");
	
		// Setup a valid object
		var salesView = new SalesView
		{
			SaleID = 0,
			SaleDate = DateTime.Now,
			EmployeeID = 1, 
			PaymentType = "M", 
			SubTotal = subTotal, 
			saleDetails = new List<SaleDetailsView>
		{
			new SaleDetailsView { StockItemID = stockItem.StockItemID, Quantity = quantity, SellingPrice = sellingPrice }
		}
		};
	
		// Test: Save the sales
		Console.WriteLine("\n----- Test: Save Valid SalesView -----");
		TestSaveSales(salesView);
	
		var savedSale = Sales.FirstOrDefault(s => s.EmployeeID == salesView.EmployeeID);
		if (savedSale == null)
		{
			passFail = "Fail";
			Console.WriteLine($"{passFail} - Sale was not saved successfully");
		}
		else
		{
			passFail = "Pass";
			Console.WriteLine($"{passFail} - Sale was saved successfully");
	
			// Check if sale details were saved
			var savedSaleDetails = SaleDetails
				.Where(sd => sd.SaleID == savedSale.SaleID)
				.ToList();
			passFail = savedSaleDetails.Count > 0 ? "Pass" : "Fail";
			Console.WriteLine($"{passFail} - Sale details were saved successfully");
	
			// Dump the updated state of the tables after saving
			Console.WriteLine("\n----- Sales After Save -----");
			Sales.Dump("Sales Table After Save");
			SaleDetails.Dump("SaleDetails Table After Save");
		}
	
		Console.WriteLine("                                  ");
	#endregion


    #region Save Refund (TestSaveRefund)
	// Header information
	Console.WriteLine("===================================");
	Console.WriteLine("             Save Refund           ");
	Console.WriteLine("===================================");

	var stockItemForRefund = StockItems.FirstOrDefault();
	if (stockItemForRefund == null)
	{
		Console.WriteLine("Fail - No valid StockItem found in the StockItems table.");
		return;
	}
    // Test Case 1: Invalid SaleID
    Console.WriteLine("Test Case 1: Invalid SaleID");
    // Use the SellingPrice from the StockItem
	var sellingPriceForRefund = stockItemForRefund.SellingPrice;
	var quantityForRefund = 2;
	var subTotalForRefund = sellingPriceForRefund * quantityForRefund;
	var invalidSaleRefundsView = new SaleRefundsView
	{
		SaleRefundDate = DateTime.Now,
		SaleID = -1, // Invalid SaleID
		EmployeeID = 8,
		TaxAmount = subTotalForRefund * 0.05m,
		SubTotal = subTotalForRefund,
		saleRefundDetails = new List<SaleRefundDetailsView>
	{
		new SaleRefundDetailsView { StockItemID = stockItemForRefund.StockItemID, Quantity = quantityForRefund, SellingPrice = sellingPriceForRefund }
	}
	};

	// Attempt to add the refund with invalid SaleID
	TestSaveRefund(invalidSaleRefundsView);

	// Validate that the refund with an invalid SaleID was not added
	var failedSaleRefund = SaleRefunds.FirstOrDefault(sr => sr.EmployeeID == invalidSaleRefundsView.EmployeeID && sr.SaleID == invalidSaleRefundsView.SaleID);
	passFail = failedSaleRefund == null ? "Pass" : "Fail";
	Console.WriteLine($"{passFail} - SaleID must be greater than zero");


	// Test Case 2: Invalid EmployeeID
	Console.WriteLine("\nTest Case 2: Invalid EmployeeID");
	//Use the SellingPrice from the StockItem
	var invalidEmployeeRefundsView = new SaleRefundsView
	{
		SaleRefundDate = DateTime.Now,
		SaleID = 1, 
		EmployeeID = -1,// Invalid EmployeeID
		TaxAmount = subTotalForRefund * 0.05m,
		SubTotal = subTotalForRefund,
		saleRefundDetails = new List<SaleRefundDetailsView>
	{
		new SaleRefundDetailsView { StockItemID = stockItemForRefund.StockItemID, Quantity = quantityForRefund, SellingPrice = sellingPriceForRefund }
	}
	};

	// Attempt to add the refund with invalid SaleID
	TestSaveRefund(invalidEmployeeRefundsView);

	// Validate that the refund with an invalid SaleID was not added
	var failedEmployeeRefund = SaleRefunds.FirstOrDefault(sr => sr.EmployeeID == invalidEmployeeRefundsView.EmployeeID && sr.SaleID == invalidEmployeeRefundsView.SaleID);
	passFail = failedEmployeeRefund == null ? "Pass" : "Fail";
	Console.WriteLine($"{passFail} - EmployeeID must be greater than zero");

	// Test Case 3: Invalid Subtotal
	Console.WriteLine("\nTest Case 3: Invalid Subutotal");
	//Use the SellingPrice from the StockItem
	var sellingPriceForInvalidSubtotal= stockItemForRefund.SellingPrice;
	var quantityForInvalidSubtotal = 2;
	var invalidSubTotal = 0;
	var invalidSubtotalSaleRefundsView = new SaleRefundsView
	{
		SaleRefundDate = DateTime.Now,
		SaleID = 1, 
		EmployeeID = 8,
		TaxAmount = subTotalForRefund * 0.05m,
		SubTotal = invalidSubTotal,
		saleRefundDetails = new List<SaleRefundDetailsView>
	{
		new SaleRefundDetailsView { StockItemID = stockItemForRefund.StockItemID, Quantity = quantityForInvalidSubtotal, SellingPrice = sellingPriceForInvalidSubtotal }
	}
	};

	// Attempt to add the refund with invalid SaleID
	TestSaveRefund(invalidSubtotalSaleRefundsView);

	// Validate that the refund with an invalid SaleID was not added
	var failedSubtotalRefund = SaleRefunds.FirstOrDefault(sr => sr.EmployeeID ==  invalidSubtotalSaleRefundsView.EmployeeID && sr.SaleID ==  invalidSubtotalSaleRefundsView.SaleID);
	passFail = failedSaleRefund == null ? "Pass" : "Fail";
	Console.WriteLine($"{passFail} - Subtotal must be greater than zero");

	// Test Case 4: Empty saleRefundDetails
	Console.WriteLine("\nTest Case 4: Empty SaleRefundDetails");
	//Use the SellingPrice from the StockItem
	var invalidDatailsSaleRefundsView = new SaleRefundsView
	{
		SaleRefundDate = DateTime.Now,
		SaleID = 1,
		EmployeeID = 8,
		TaxAmount = subTotalForRefund * 0.05m,
		SubTotal = subTotalForRefund,
		saleRefundDetails = new List<SaleRefundDetailsView>
	{
		new SaleRefundDetailsView {}
	}
	};

	// Attempt to add the refund with invalid SaleID
	TestSaveRefund(invalidDatailsSaleRefundsView);

	// Validate that the refund with an invalid SaleID was not added
	var failedDetailsRefund = SaleRefunds.FirstOrDefault(sr => sr.EmployeeID == invalidDatailsSaleRefundsView.EmployeeID && sr.SaleID == invalidDatailsSaleRefundsView.SaleID);
	passFail = failedSaleRefund == null ? "Pass" : "Fail";
	Console.WriteLine($"{passFail} - Details cannot be empty");

	#endregion
    #region
	
	// Dump the tables before saving
	Console.WriteLine("\n----- SaleRefunds Before Save -----");
	SaleRefunds.Dump("SaleRefunds Table Before Save");
	SaleRefundDetails.Dump("SaleRefundDetails Table Before Save");
	
	var saleRefundsView = new SaleRefundsView
	{
		SaleRefundDate = DateTime.Now,
		SaleID = 6, 
		EmployeeID = 8,
		TaxAmount = subTotalForRefund * 0.05m, 
		SubTotal = subTotalForRefund,
		saleRefundDetails = new List<SaleRefundDetailsView>
		{
			new SaleRefundDetailsView {   StockItemID = stockItemForRefund.StockItemID, Quantity = quantityForRefund, SellingPrice = sellingPriceForRefund}
		}
	};

	// Add the refund
	TestSaveRefund(saleRefundsView);
	// Dump the updated tables after saving
	Console.WriteLine("\n----- SaleRefunds After Save -----");
	SaleRefunds.Dump("SaleRefunds Table After Save");
	SaleRefundDetails.Dump("SaleRefundDetails Table After Save");
	// Check if the sale refund was saved successfully
	var savedSaleRefund = SaleRefunds.FirstOrDefault(sr => sr.EmployeeID == saleRefundsView.EmployeeID);
	passFail = savedSaleRefund != null ? "Pass" : "Fail";
	Console.WriteLine($"{passFail} - Sale refund was saved successfully.");

	if (savedSaleRefund == null)
	{
		passFail = "Fail";
		Console.WriteLine($"{passFail} - Sale refund was not saved successfully.");
	}
	else
	{
		passFail = "Pass";
		Console.WriteLine($"{passFail} - Sale refund was saved successfully.");

		var savedSaleRefundDetails = SaleRefundDetails
			.Where(srd => srd.SaleRefundID == savedSaleRefund.SaleRefundID)
			.ToList();
		passFail = savedSaleRefundDetails.Count > 0 ? "Pass" : "Fail";
		Console.WriteLine($"{passFail} - Sale refund details were saved successfully.");
	}
	#endregion
}
#region Test Methods
public List<CategoriesView> TestGetCategories(int categoryID)
{
	try
	{
		return GetCategories(categoryID);
	}
	#region catch all exception
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
	return null;  //  Ensure a valid return value even on failure
}


public List<StockItemsView> TestGetItemByCategoryID(int categoryID)
{
	try
	{
		return GetItemByCategoryID(categoryID);
	}
	#region catch all exception
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
	return null;  //  Ensure a valid return value even on failure
}



public List<SaleRefundsView> TestGetSaleRefund(int saleRefundID)
{
	try
	{
		return GetSaleRefund(saleRefundID);
	}
	#region catch all exception
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
	return null;  //  Ensure a valid return value even on failure
}

public void TestSaveSales(SalesView salesView)
{
	try
	{
		SaveSales(salesView);
	}
	#region catch all exception
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
}


public void TestSaveRefund(SaleRefundsView saleRefundsView)
{
	try
	{
		SaveRefund(saleRefundsView);
	}
	#region catch all exception
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
}



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

	// Any errors
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

	// Any errors
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

	// Validate input
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

	// Any errors
	if (errorList.Count() > 0)
	{
		ChangeTracker.Clear();
		string errorMsg = "Unable to save.";
		errorMsg += " Please check error message(s)";
		throw new AggregateException(errorMsg, errorList);
	}

	// Debugging
	Console.WriteLine($"SaleID: {salesView.SaleID}, EmployeeID: {salesView.EmployeeID}, PaymentType: {salesView.PaymentType}, SubTotal: {salesView.SubTotal}");

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
		// Debugging
		Console.WriteLine($"Processing SaleDetail: StockItemID: {saleDetailView.StockItemID}, Quantity: {saleDetailView.Quantity}, SellingPrice: {saleDetailView.SellingPrice}");

		// Retrieve the sale detail from the database or create a new one if it doesn't exist
		SaleDetails saleDetail = SaleDetails
			.FirstOrDefault(x => x.SaleDetailID == saleDetailView.SaleDetailID && x.StockItemID == saleDetailView.StockItemID);
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
	
	// Validate input
	if (saleRefundsView == null)
	{
		errorList.Add(new ArgumentException("saleRefund cannot be null"));
	}
	else
	{
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
			errorList.Add(new ArgumentException("Invalid Subtotal."));
		}
	}
		
	// Validate sale refund details
	if (saleRefundsView.saleRefundDetails == null || !saleRefundsView.saleRefundDetails.Any())
	{
		errorList.Add(new ArgumentException("Sale refund details cannot be null or empty."));
	}
	else
	{
		foreach (var detail in saleRefundsView.saleRefundDetails)
	    {
			if (detail.Quantity <= 0)
			{
				errorList.Add(new ArgumentException($"Invalid quantity for item ID {detail.StockItemID}."));
			}
			if (detail.SellingPrice <= 0)
			{
				errorList.Add(new ArgumentException($"Invalid selling price for item ID {detail.StockItemID}."));
			}
			// Check if the StockItemID exists in the database
			var stockItem = StockItems.FirstOrDefault(si => si.StockItemID == detail.StockItemID);
			if (stockItem == null)
			{
				errorList.Add(new ArgumentException($"StockItemID {detail.StockItemID} does not exist in the database."));
			}
		}
	}
			
	// If any validation errors were found, throw an exception
	if (errorList.Any())
	{
		ChangeTracker.Clear();
		throw new AggregateException("Unable to save. Please check error messages.", errorList);
    }

	// Create a new SaleRefunds object
	SaleRefunds saleRefunds = new SaleRefunds
	{
		SaleRefundDate = saleRefundsView.SaleRefundDate,
		SaleID = saleRefundsView.SaleID,
		EmployeeID = saleRefundsView.EmployeeID,
		TaxAmount = saleRefundsView.TaxAmount,
		SubTotal = saleRefundsView.SubTotal,
		RemoveFromViewFlag = saleRefundsView.RemoveFromViewFlag
	};

	// Add SaleRefundDetails
	foreach (var saleRefundDetailView in saleRefundsView.saleRefundDetails)
	{
		SaleRefundDetails saleRefundDetails = new SaleRefundDetails
		{
			StockItemID = saleRefundDetailView.StockItemID,
			Quantity = saleRefundDetailView.Quantity,
			SellingPrice = saleRefundDetailView.SellingPrice
		};
		saleRefunds.SaleRefundDetails.Add(saleRefundDetails);
	}

	// Add new SaleRefunds to the database
	SaleRefunds.Add(saleRefunds);
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

