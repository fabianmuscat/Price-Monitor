USE PriceMonitor;
GO

INSERT INTO project.Activity ([Type])
VALUES
	('Logged In'),
	('Logged Out'),
	('Searched'),
	('Monitored Product'),
	('Removed Monitored Product');