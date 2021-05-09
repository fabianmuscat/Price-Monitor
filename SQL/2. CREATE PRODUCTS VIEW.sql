USE PriceMonitor;
GO

CREATE VIEW ProductsObtainedView AS
	SELECT 
		product.ProductTitle AS 'Product Title',
		product.Price 'Subtotal',
		product.Shipping,
		product.[URL],
		website.WebsiteName 'Website',
		cat.CategoryName 'Category',
		search.SearchTerm

	FROM project.Product product
	JOIN project.Website website
		ON (product.WebsiteID = website.WebsiteID)
	JOIN project.Category cat
		ON (product.CategoryID = cat.CategoryID)
	JOIN project.Search search
		ON (product.SearchID = search.SearchID);