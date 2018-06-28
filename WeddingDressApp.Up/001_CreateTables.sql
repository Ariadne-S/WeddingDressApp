CREATE TABLE Dresses (
	DressId int PRIMARY KEY NOT NULL,
	SequentialId int identity NOT NULL,
    DressName nvarchar(25) NOT NULL,  
	DressWebpage nvarchar (500),
    Price money,  
    ProductDescription nvarchar (max),
	DressType varchar (11) NOT NULL CHECK (DressType IN('Bride', 'Bridesmaids')),
	RecommendedBy uniqueidentifier,
	Approval varchar (8) NOT NULL CHECK (Approval IN('Yes', 'No', 'Required')),
	Rating int,
	ShopId uniqueidentifier,
	ImageId uniqueidentifier
)

CREATE TABLE Shops (
	ShopId int PRIMARY KEY NOT NULL,
	SequentialId int identity NOT NULL,
    ShopName nvarchar(25) NOT NULL,  
	ShopWebpage nvarchar (500),
	ShopLocation nvarchar (30),
	Logo varbinary(max) 
)

CREATE TABLE JoinImage (
	ImageJoinId int PRIMARY KEY NOT NULL,
	SequentialId int identity NOT NULL,
    DressId int NOT NULL,  
	ImageId int NOT NULL,
	Favourite bit NOT NULL
)

CREATE TABLE Images (
	ImageId int PRIMARY KEY NOT NULL,
	SequentialId int identity NOT NULL,
    FileName varchar(15) NOT NULL,  
	FileExtension varchar (5) NOT NULL,
	FileData varbinary (max),
	Hash var256
)

CREATE TABLE Users (
	UserId int PRIMARY KEY NOT NULL,
	SequentialId int identity NOT NULL,
    Username varchar (20) NOT NULL,  
	Email varchar (150) NOT NULL,
	IsEmailValidated bit NOT NULL
)

CREATE TABLE Weddings (
	WeddingId int PRIMARY KEY NOT NULL,
	SequentialId int identity NOT NULL,
    Eventname varchar (20) NOT NULL,  
	EventDate varchar (150) NOT NULL
)

CREATE TABLE Guests (
	GuestId int PRIMARY KEY NOT NULL,
	SequentialId int identity NOT NULL,
    UserId int NOT NULL,  
	WeddingId int NOT NULL,
	GuestType varchar (20)
)