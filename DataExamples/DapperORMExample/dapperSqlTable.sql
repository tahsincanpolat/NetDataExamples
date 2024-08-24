create database ProductDBDapper
use ProductDBDapper
create table Categories(
	CategoryId int primary key identity,
	[Name] nvarchar(max) not null
)
create table Products(
	ProductId int primary key identity,
	[Name] nvarchar(max) not null,
	Price decimal(18,2) not null,
	CategoryId int,
	Foreign key (CategoryId) references Categories(CategoryId)
)