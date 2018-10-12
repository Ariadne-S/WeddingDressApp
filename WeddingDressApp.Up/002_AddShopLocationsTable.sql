ALTER TABLE Shops 
ADD LocationId UniqueIdentifier NOT NULL;

ALTER TABLE Shops
DROP COLUMN ShopLocation;
