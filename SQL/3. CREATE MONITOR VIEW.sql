USE PriceMonitor;
GO

CREATE VIEW MonitoredProductsView AS
	SELECT 
		pro.ProductTitle 'Product', 
		pro.Price 'Subtotal',
		pro.Shipping,
		Website.WebsiteName 'Obtained From', 
		pro.[URL] 'Link', 
		Account.Username AS Account, 
		FORMAT(DateMonitored, 'dd/MM/yyyy') 'Date Monitored'

	FROM project.MonitorProduct monitor
	JOIN project.Product pro
		ON (monitor.ProductID = pro.ProductID)
	JOIN project.Website website
		ON (monitor.WebsiteID = website.WebsiteID)
	JOIN project.Account account
		ON (monitor.AccountID = account.AccountID);
