USE [master]
GO
/****** Object:  Database [PriceMonitor]    Script Date: 5/21/2020 10:36:04 AM ******/
CREATE DATABASE [PriceMonitor]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PriceMonitor', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\PriceMonitor.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PriceMonitor_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\PriceMonitor_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [PriceMonitor] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PriceMonitor].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PriceMonitor] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PriceMonitor] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PriceMonitor] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PriceMonitor] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PriceMonitor] SET ARITHABORT OFF 
GO
ALTER DATABASE [PriceMonitor] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [PriceMonitor] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PriceMonitor] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PriceMonitor] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PriceMonitor] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PriceMonitor] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PriceMonitor] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PriceMonitor] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PriceMonitor] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PriceMonitor] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PriceMonitor] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PriceMonitor] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PriceMonitor] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PriceMonitor] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PriceMonitor] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PriceMonitor] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PriceMonitor] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PriceMonitor] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [PriceMonitor] SET  MULTI_USER 
GO
ALTER DATABASE [PriceMonitor] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PriceMonitor] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PriceMonitor] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PriceMonitor] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PriceMonitor] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PriceMonitor] SET QUERY_STORE = OFF
GO
USE [PriceMonitor]
GO
/****** Object:  Schema [project]    Script Date: 5/21/2020 10:36:05 AM ******/
CREATE SCHEMA [project]
GO
/****** Object:  Table [project].[Website]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [project].[Website](
	[WebsiteID] [int] IDENTITY(1,1) NOT NULL,
	[WebsiteName] [varchar](20) NOT NULL,
	[URL] [varchar](255) NOT NULL,
 CONSTRAINT [web_id_pk] PRIMARY KEY CLUSTERED 
(
	[WebsiteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [project].[Account]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [project].[Account](
	[AccountID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](25) NOT NULL,
	[Password] [varchar](25) NOT NULL,
	[Email] [varchar](50) NULL,
 CONSTRAINT [account_id_pk] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [project].[Product]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [project].[Product](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductTitle] [varchar](max) NOT NULL,
	[Price] [numeric](7, 2) NOT NULL,
	[Shipping] [numeric](5, 2) NULL,
	[URL] [varchar](max) NOT NULL,
	[CategoryID] [int] NULL,
	[SearchID] [int] NULL,
	[WebsiteID] [int] NULL,
 CONSTRAINT [product_id_pk] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [project].[MonitorProduct]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [project].[MonitorProduct](
	[MonitorID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NULL,
	[AccountID] [int] NULL,
	[WebsiteID] [int] NULL,
	[DateMonitored] [datetime] NOT NULL,
 CONSTRAINT [mon_id_pk] PRIMARY KEY CLUSTERED 
(
	[MonitorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[MonitoredProductsView]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[MonitoredProductsView] AS
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
GO
/****** Object:  Table [project].[Search]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [project].[Search](
	[SearchID] [int] IDENTITY(1,1) NOT NULL,
	[SearchTerm] [varchar](20) NOT NULL,
	[Date] [datetime] NOT NULL,
 CONSTRAINT [search_id_pk] PRIMARY KEY CLUSTERED 
(
	[SearchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [project].[Category]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [project].[Category](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [varchar](50) NOT NULL,
	[WebsiteID] [int] NULL,
 CONSTRAINT [category_id_pk] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[ProductsObtainedView]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ProductsObtainedView] AS
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
GO
/****** Object:  Table [project].[Activity]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [project].[Activity](
	[ActivityID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](30) NOT NULL,
 CONSTRAINT [activity_id_pk] PRIMARY KEY CLUSTERED 
(
	[ActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [project].[Log]    Script Date: 5/21/2020 10:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [project].[Log](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[ActivityID] [int] NOT NULL,
	[SearchID] [int] NULL,
	[Time] [datetime] NOT NULL,
 CONSTRAINT [log_id_pk] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [project].[Activity] ON 

INSERT [project].[Activity] ([ActivityID], [Type]) VALUES (1, N'Logged In')
INSERT [project].[Activity] ([ActivityID], [Type]) VALUES (2, N'Logged Out')
INSERT [project].[Activity] ([ActivityID], [Type]) VALUES (4, N'Monitored Product')
INSERT [project].[Activity] ([ActivityID], [Type]) VALUES (5, N'Removed Monitored Product')
INSERT [project].[Activity] ([ActivityID], [Type]) VALUES (3, N'Searched')
SET IDENTITY_INSERT [project].[Activity] OFF
SET ANSI_PADDING ON
GO
/****** Object:  Index [username_unique]    Script Date: 5/21/2020 10:36:05 AM ******/
ALTER TABLE [project].[Account] ADD  CONSTRAINT [username_unique] UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [email_ind_un]    Script Date: 5/21/2020 10:36:05 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [email_ind_un] ON [project].[Account]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [type_unique]    Script Date: 5/21/2020 10:36:05 AM ******/
ALTER TABLE [project].[Activity] ADD  CONSTRAINT [type_unique] UNIQUE NONCLUSTERED 
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [term_unique]    Script Date: 5/21/2020 10:36:05 AM ******/
ALTER TABLE [project].[Search] ADD  CONSTRAINT [term_unique] UNIQUE NONCLUSTERED 
(
	[SearchTerm] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [url_unique]    Script Date: 5/21/2020 10:36:05 AM ******/
ALTER TABLE [project].[Website] ADD  CONSTRAINT [url_unique] UNIQUE NONCLUSTERED 
(
	[URL] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [web_name_unique]    Script Date: 5/21/2020 10:36:05 AM ******/
ALTER TABLE [project].[Website] ADD  CONSTRAINT [web_name_unique] UNIQUE NONCLUSTERED 
(
	[WebsiteName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [project].[Category]  WITH CHECK ADD  CONSTRAINT [web_id_pro_fk] FOREIGN KEY([WebsiteID])
REFERENCES [project].[Website] ([WebsiteID])
GO
ALTER TABLE [project].[Category] CHECK CONSTRAINT [web_id_pro_fk]
GO
ALTER TABLE [project].[Log]  WITH CHECK ADD  CONSTRAINT [account_id_log_fk] FOREIGN KEY([AccountID])
REFERENCES [project].[Account] ([AccountID])
GO
ALTER TABLE [project].[Log] CHECK CONSTRAINT [account_id_log_fk]
GO
ALTER TABLE [project].[Log]  WITH CHECK ADD  CONSTRAINT [activity_id_log_fk] FOREIGN KEY([ActivityID])
REFERENCES [project].[Activity] ([ActivityID])
GO
ALTER TABLE [project].[Log] CHECK CONSTRAINT [activity_id_log_fk]
GO
ALTER TABLE [project].[MonitorProduct]  WITH CHECK ADD  CONSTRAINT [account_id_mon_fk] FOREIGN KEY([AccountID])
REFERENCES [project].[Account] ([AccountID])
GO
ALTER TABLE [project].[MonitorProduct] CHECK CONSTRAINT [account_id_mon_fk]
GO
ALTER TABLE [project].[MonitorProduct]  WITH CHECK ADD  CONSTRAINT [product_id_mon_fk] FOREIGN KEY([ProductID])
REFERENCES [project].[Product] ([ProductID])
GO
ALTER TABLE [project].[MonitorProduct] CHECK CONSTRAINT [product_id_mon_fk]
GO
ALTER TABLE [project].[MonitorProduct]  WITH CHECK ADD  CONSTRAINT [web_id_mon_fk] FOREIGN KEY([WebsiteID])
REFERENCES [project].[Website] ([WebsiteID])
GO
ALTER TABLE [project].[MonitorProduct] CHECK CONSTRAINT [web_id_mon_fk]
GO
ALTER TABLE [project].[Product]  WITH CHECK ADD  CONSTRAINT [category_id_pro_fk] FOREIGN KEY([CategoryID])
REFERENCES [project].[Category] ([CategoryID])
GO
ALTER TABLE [project].[Product] CHECK CONSTRAINT [category_id_pro_fk]
GO
ALTER TABLE [project].[Product]  WITH CHECK ADD  CONSTRAINT [search_id_pro_fk] FOREIGN KEY([SearchID])
REFERENCES [project].[Search] ([SearchID])
GO
ALTER TABLE [project].[Product] CHECK CONSTRAINT [search_id_pro_fk]
GO
ALTER TABLE [project].[Product]  WITH CHECK ADD  CONSTRAINT [website_id_pro_fk] FOREIGN KEY([WebsiteID])
REFERENCES [project].[Website] ([WebsiteID])
GO
ALTER TABLE [project].[Product] CHECK CONSTRAINT [website_id_pro_fk]
GO
USE [master]
GO
ALTER DATABASE [PriceMonitor] SET  READ_WRITE 
GO
