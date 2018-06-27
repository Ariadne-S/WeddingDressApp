CREATE TABLE Dresses (
	Id int PRIMARY KEY NOT NULL,
	SequentialId int identity NOT NULL,
    DressName varchar(25) NOT NULL,  
	DressWebpage nvarchar (500),
    Price money,  
    ProductDescription nvarchar (max),
	DressType varchar (11) NOT NULL CHECK (DressType IN('Bride', 'Bridesmaids')),
	Recommended varchar (25),
	Approval varchar (8) NOT NULL CHECK (Approval IN('Yes', 'No', 'Required')),
	Rating int,
	ShopId uniqueidentifier,
	ImageId uniqueidentifier
)



