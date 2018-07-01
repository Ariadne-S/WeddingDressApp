CREATE TABLE Dresses (
	DressId UniqueIdentifier PRIMARY KEY NONCLUSTERED NOT NULL,
	SequentialId Int identity NOT NULL,
    DressName nVarChar(25) NOT NULL,  
	DressWebpage nVarChar (500),
    Price money,  
    ProductDescription nVarChar (max),
	DressType VarChar (11) NOT NULL, --CHECK (DressType IN('Bride', 'Bridesmaids')),
	RecommendedBy UniqueIdentifier,
	Approval VarChar (8) NOT NULL, --CHECK (Approval IN('Yes', 'No', 'Required')),
	Rating Int,
	ShopId UniqueIdentifier,
	ImageId UniqueIdentifier
)
CREATE CLUSTERED INDEX DressesSeqId ON dbo.Dresses(SequentialId)

CREATE TABLE Shops (
	ShopId UniqueIdentifier PRIMARY KEY NONCLUSTERED NOT NULL,
	SequentialId Int identity NOT NULL,
    ShopName nVarChar(25) NOT NULL,  
	ShopWebpage nVarChar (500),
	ShopLocation nVarChar (30),
	ImageId UniqueIdentifier NOT NULL
)
CREATE CLUSTERED INDEX ShopsSeqId ON dbo.Shops(SequentialId)

CREATE TABLE DressImages (
	--ImageJoinId UniqueIdentifier PRIMARY KEY NONCLUSTERED NOT NULL,
	SequentialId Int identity Primary Key NOT NULL,
    DressId UniqueIdentifier NOT NULL,  
	ImageId UniqueIdentifier NOT NULL,
	Favourite bit NOT NULL
)

CREATE TABLE Images (
	ImageId UniqueIdentifier PRIMARY KEY NONCLUSTERED NOT NULL,
	SequentialId Int identity NOT NULL,
    [FileName] VarChar(128) NOT NULL,  
	FileExtension VarChar (32) NOT NULL,
	FileData VarBinary (max),
	[Hash] VarChar(256)
)
CREATE CLUSTERED INDEX ImagesSeqId ON dbo.Images(SequentialId)

CREATE TABLE Users (
	UserId UniqueIdentifier PRIMARY KEY NONCLUSTERED NOT NULL,
	SequentialId Int identity NOT NULL,
    Username VarChar (64) NOT NULL,  
	Email VarChar (150) NOT NULL,
	IsEmailValidated bit NOT NULL
)
CREATE CLUSTERED INDEX UsersSeqId ON dbo.Users(SequentialId)

CREATE TABLE Weddings (
	WeddingId UniqueIdentifier PRIMARY KEY NONCLUSTERED NOT NULL,
	SequentialId Int identity NOT NULL,
    EventName VarChar (64) NOT NULL,  
	EventDate DateTime2 NOT NULL
)
CREATE CLUSTERED INDEX WeddingsSeqId ON dbo.Weddings(SequentialId)

CREATE TABLE Guests (
	GuestId UniqueIdentifier PRIMARY KEY NONCLUSTERED NOT NULL,
	SequentialId Int identity NOT NULL,
    UserId UniqueIdentifier NOT NULL,  
	WeddingId UniqueIdentifier NOT NULL,
	GuestType VarChar (20)
)
CREATE CLUSTERED INDEX GuestsSeqId ON dbo.Guests(SequentialId)