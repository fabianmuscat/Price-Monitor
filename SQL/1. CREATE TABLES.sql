CREATE DATABASE PriceMonitor;
GO

CREATE SCHEMA project;
GO

USE PriceMonitor;
GO

CREATE TABLE project.Website
(
	[WebsiteID] INTEGER IDENTITY(1, 1) CONSTRAINT web_id_pk PRIMARY KEY,
	[WebsiteName] VARCHAR(20) NOT NULL CONSTRAINT web_name_unique UNIQUE,
	[URL] VARCHAR(255) NOT NULL CONSTRAINT url_unique UNIQUE
);

CREATE TABLE project.Account
(
	[AccountID] INTEGER IDENTITY(1, 1) CONSTRAINT account_id_pk PRIMARY KEY,
	[Username] VARCHAR(25) NOT NULL CONSTRAINT username_unique UNIQUE,
	[Password] VARCHAR(25) NOT NULL,
	[Email] VARCHAR(50)
);

CREATE TABLE project.Activity 
(
	[ActivityID] INTEGER IDENTITY(1, 1) CONSTRAINT activity_id_pk PRIMARY KEY,
	[Type] VARCHAR(30) NOT NULL CONSTRAINT type_unique UNIQUE,
);

CREATE TABLE project.Search
(
	[SearchID] INTEGER IDENTITY(1, 1) CONSTRAINT search_id_pk PRIMARY KEY,
	[SearchTerm] VARCHAR(20) NOT NULL CONSTRAINT term_unique UNIQUE,
	[Date] DATETIME NOT NULL
);

CREATE TABLE project.[Log]
(
	[LogID] INTEGER IDENTITY(1, 1) CONSTRAINT log_id_pk PRIMARY KEY,
	[AccountID] INTEGER NOT NULL CONSTRAINT account_id_log_fk FOREIGN KEY REFERENCES project.Account(AccountID),
	[ActivityID] INTEGER NOT NULL CONSTRAINT activity_id_log_fk FOREIGN KEY REFERENCES project.Activity(ActivityID),
	[SearchID] INTEGER,
	[Time] DATETIME NOT NULL 
);

CREATE TABLE project.Category 
(
	[CategoryID] INTEGER IDENTITY(1,1) CONSTRAINT category_id_pk PRIMARY KEY,
	[CategoryName] VARCHAR(50) NOT NULL,
	[WebsiteID] INTEGER CONSTRAINT web_id_pro_fk FOREIGN KEY REFERENCES project.Website(WebsiteID),
);

CREATE TABLE project.Product
(
	[ProductID] INTEGER IDENTITY(1, 1) CONSTRAINT product_id_pk PRIMARY KEY,
	[ProductTitle] VARCHAR(MAX) NOT NULL,
	[Price] NUMERIC(7,2) NOT NULL,
	[Shipping] NUMERIC(5,2),
	[URL] VARCHAR(MAX) NOT NULL,
	[CategoryID] INTEGER CONSTRAINT category_id_pro_fk FOREIGN KEY REFERENCES project.Category(CategoryID),
	[SearchID] INTEGER CONSTRAINT search_id_pro_fk FOREIGN KEY REFERENCES project.Search(SearchID),
	[WebsiteID] INTEGER CONSTRAINT website_id_pro_fk FOREIGN KEY REFERENCES project.Website(WebsiteID)
);

CREATE TABLE project.MonitorProduct
(
	[MonitorID] INTEGER IDENTITY(1, 1) CONSTRAINT mon_id_pk PRIMARY KEY,
	[ProductID] INTEGER CONSTRAINT product_id_mon_fk FOREIGN KEY REFERENCES project.Product(ProductID),
	[AccountID] INTEGER CONSTRAINT account_id_mon_fk FOREIGN KEY REFERENCES project.Account(AccountID),
	[WebsiteID] INTEGER CONSTRAINT web_id_mon_fk FOREIGN KEY REFERENCES project.Website(WebsiteID),
	[DateMonitored] DATETIME NOT NULL
);

CREATE UNIQUE INDEX email_ind_un
ON project.Account(Email)
WHERE [Email] IS NOT NULL;