USE PriceMonitor;
GO

DELETE FROM project.MonitorProduct
DBCC CHECKIDENT ('PriceMonitor.project.MonitorProduct',RESEED, 0)

DELETE FROM project.Product
DBCC CHECKIDENT ('PriceMonitor.project.Product',RESEED, 0)

DELETE FROM project.Log
DBCC CHECKIDENT ('PriceMonitor.project.Log',RESEED, 0)

DELETE FROM project.Activity
DBCC CHECKIDENT ('PriceMonitor.project.Activity',RESEED, 0)

DELETE FROM project.Website
DBCC CHECKIDENT ('PriceMonitor.project.Website',RESEED, 0)

DELETE FROM project.Search
DBCC CHECKIDENT ('PriceMonitor.project.Search',RESEED, 0)

DELETE FROM project.Account
DBCC CHECKIDENT ('PriceMonitor.project.Account',RESEED, 0)

DELETE FROM project.Category
DBCC CHECKIDENT ('PriceMonitor.project.Category',RESEED, 0)

DROP VIEW MonitoredProductsView;
DROP VIEW ProductsObtainedView;

DROP TABLE project.MonitorProduct;
DROP TABLE project.Product;
DROP TABLE project.[Log];
DROP TABLE project.Activity;
DROP TABLE project.Website;
DROP TABLE project.Search;
DROP TABLE project.Account;
DROP TABLE project.Category;

USE master;
GO

DROP SCHEMA project;
DROP DATABASE PriceMonitor;