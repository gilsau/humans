use Humans
go

print '*** Function: DistanceBetween ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.ROUTINES where routine_name = N'DistanceBetween')
begin
	drop function dbo.DistanceBetween
end
go
CREATE FUNCTION [dbo].DistanceBetween (@Lat1 as real, @Long1 as real, @Lat2 as real, @Long2 as real)
RETURNS real
AS
BEGIN

DECLARE @dLat1InRad as float(53);
SET @dLat1InRad = @Lat1 * (PI()/180.0);
DECLARE @dLong1InRad as float(53);
SET @dLong1InRad = @Long1 * (PI()/180.0);
DECLARE @dLat2InRad as float(53);
SET @dLat2InRad = @Lat2 * (PI()/180.0);
DECLARE @dLong2InRad as float(53);
SET @dLong2InRad = @Long2 * (PI()/180.0);

DECLARE @dLongitude as float(53);
SET @dLongitude = @dLong2InRad - @dLong1InRad;
DECLARE @dLatitude as float(53);
SET @dLatitude = @dLat2InRad - @dLat1InRad;
/* Intermediate result a. */
DECLARE @a as float(53);
SET @a = SQUARE (SIN (@dLatitude / 2.0)) + COS (@dLat1InRad) 
                 * COS (@dLat2InRad) 
                 * SQUARE(SIN (@dLongitude / 2.0));
/* Intermediate result c (great circle distance in Radians). */
DECLARE @c as real;
SET @c = 2.0 * ATN2 (SQRT (@a), SQRT (1.0 - @a));
DECLARE @kEarthRadius as real;
SET @kEarthRadius = 3956.0; /*miles*/
--SET @kEarthRadius = 6376.5;        /* kms */

DECLARE @dDistance as real;
SET @dDistance = @kEarthRadius * @c;
return (@dDistance);
END
go

print '*** Table: State ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'State')
begin
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_Venue_State')
	begin
		alter table Venue drop constraint fk_Venue_State
	end

	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserProfile_State_Company')
	begin
		alter table UserProfile drop constraint fk_UserProfile_State_Company
	end

	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserProfile_State')
	begin
		alter table UserProfile drop constraint fk_UserProfile_State
	end

	drop table dbo.[State]
end
go
create table dbo.[State]
(
	Id int identity(1,1) not null primary key,
	Abbrev nvarchar(5) not null,
	Name nvarchar(50) not null
)
insert into [State]
select 'AL', 'Alabama'
union select 'AK', 'Alaska'
union select 'AZ', 'Arizona'
union select 'AR', 'Arkansas'
union select 'CA', 'California'
union select 'CO', 'Colorado'
union select 'CT', 'Connecticut'
union select 'DE', 'Delaware'
union select 'DC', 'District Of Columbia'
union select 'FL', 'Florida'
union select 'GA', 'Georgia'
union select 'HI', 'Hawaii'
union select 'ID', 'Idaho'
union select 'IL', 'Illinois'
union select 'IN', 'Indiana'
union select 'IA', 'Iowa'
union select 'KS', 'Kansas'
union select 'KY', 'Kentucky'
union select 'LA', 'Louisiana'
union select 'ME', 'Maine'
union select 'MD', 'Maryland'
union select 'MA', 'Massachusetts'
union select 'MI', 'Michigan'
union select 'MN', 'Minnesota'
union select 'MS', 'Mississippi'
union select 'MO', 'Missouri'
union select 'MT', 'Montana'
union select 'NE', 'Nebraska'
union select 'NV', 'Nevada'
union select 'NH', 'New Hampshire'
union select 'NJ', 'New Jersey'
union select 'NM', 'New Mexico'
union select 'NY', 'New York'
union select 'NC', 'North Carolina'
union select 'ND', 'North Dakota'
union select 'OH', 'Ohio'
union select 'OK', 'Oklahoma'
union select 'OR', 'Oregon'
union select 'PA', 'Pennsylvania'
union select 'RI', 'Rhode Island'
union select 'SC', 'South Carolina'
union select 'SD', 'South Dakota'
union select 'TN', 'Tennessee'
union select 'TX', 'Texas'
union select 'UT', 'Utah'
union select 'VT', 'Vermont'
union select 'VA', 'Virginia'
union select 'WA', 'Washington'
union select 'WV', 'West Virginia'
union select 'WI', 'Wisconsin'
union select 'WY', 'Wyoming'
go

print '*** Table: webpages_OAuthMembership ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'webpages_OAuthMembership')
begin
	drop table dbo.webpages_OAuthMembership
end
CREATE TABLE [dbo].[webpages_OAuthMembership](
	[Provider] [nvarchar](30) NOT NULL,
	[ProviderUserId] [nvarchar](100) NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Provider] ASC,
	[ProviderUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

print '*** Table: webpages_Membership ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'webpages_Membership')
begin
	drop table dbo.webpages_Membership
end
CREATE TABLE [dbo].[webpages_Membership](
	[UserId] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](128) NULL,
	[IsConfirmed] [bit] NULL,
	[LastPasswordFailureDate] [datetime] NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordChangedDate] [datetime] NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[PasswordVerificationToken] [nvarchar](128) NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [IsConfirmed]

ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [PasswordFailuresSinceLastSuccess]
go

print '*** Table: UserProfile ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'UserProfile')
begin
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserId')
	begin
		alter table webpages_UsersInRoles drop constraint fk_UserId
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserSports_UserProfile')
	begin
		alter table UserSports drop constraint fk_UserSports_UserProfile
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserUsers_User')
	begin
		alter table UserUsers drop constraint fk_UserUsers_User
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserUsers_Remembered')
	begin
		alter table UserUsers drop constraint fk_UserUsers_Remembered
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserVenues_UserProfile')
	begin
		alter table UserVenues drop constraint fk_UserVenues_UserProfile
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserMessages_From')
	begin
		alter table UserMessages drop constraint fk_UserMessages_From
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserMessages_To')
	begin
		alter table UserMessages drop constraint fk_UserMessages_To
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserNotifications_User')
	begin
		alter table UserNotifications drop constraint fk_UserNotifications_User
	end

	drop table dbo.UserProfile
end
CREATE TABLE [dbo].[UserProfile](
	UserId [int] IDENTITY(1,1) NOT NULL,
	UserName nvarchar(56) NOT NULL,
	FirstName nvarchar(50) NOT NULL,
	MiddleName nvarchar(50) NULL,
	LastName nvarchar(50) NULL,
	City nvarchar(50) NULL,
	StateId int NULL constraint fk_UserProfile_State foreign key references [State](Id),
	Zipcode nvarchar(15) NULL,
	ProfilePic nvarchar(250) NULL,
	Gender char(1) not null,
	Birthdate datetime not null,
	Title nvarchar(50) null,
	Company nvarchar(100) null,
	CompanyAddress1 nvarchar(50) NULL,
	CompanyAddress2 nvarchar(50) NULL,
	CompanyCity nvarchar(50) null,
	CompanyStateId int null constraint fk_UserProfile_State_Company foreign key references [State](Id),
	CompanyZipcode nvarchar(15) null,
	CompanyPhone nvarchar(15) null,
	PricePerHrMin decimal null,
	PricePerHrMax decimal null,
	Paypal nvarchar(250) NULL,
	NameOnCard nvarchar(50) null,
	CreditCardNo nvarchar(16) null,
	ExpDateMo int null,
	ExpDateYr int null,
	CardType nvarchar(10) null,
	Lat float null,
	Lng float null,
	FB bit null
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

print '*** Table: webpages_Roles ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'webpages_Roles')
begin
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_RoleId')
	begin
		alter table webpages_UsersInRoles drop constraint fk_RoleId
	end
	
	drop table dbo.webpages_Roles
end
CREATE TABLE [dbo].[webpages_Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

print '*** Table: webpages_UsersInRoles ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'webpages_UsersInRoles')
begin
	drop table dbo.webpages_UsersInRoles
end
CREATE TABLE [dbo].[webpages_UsersInRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

print '*** Table: Status ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'Status')
begin
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_Message_Status')
	begin
		alter table [Message] drop constraint fk_Message_Status
	end
	
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserMessages_Status')
	begin
		alter table UserMessages drop constraint fk_UserMessages_Status
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserNotifications_Status')
	begin
		alter table UserNotifications drop constraint fk_UserNotifications_Status
	end

	drop table dbo.[Status]
end
create table dbo.[Status]
(
	Id int identity(1,1) not null primary key,
	Name nvarchar(10) not null
)
go
insert into Status values('New')
go
insert into Status values('Pending')
go
insert into Status values('Completed')
go
insert into Status values('Archived')
go
insert into Status values('Accepted')
go
insert into Status values('Denied')
go
insert into Status values('Cancelled')
go

print '*** Table: NotificationType ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'NotificationType')
begin
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserNotifications_NotificationType')
	begin
		alter table UserNotifications drop constraint fk_UserNotifications_NotificationType
	end

	drop table dbo.NotificationType
end
create table dbo.NotificationType
(
	Id int identity(1,1) not null primary key,
	Name nvarchar(50) not null
)
go
insert into NotificationType values('VenueNear')
go
insert into NotificationType values('CommentForVenue')
go
insert into NotificationType values('ReplyToVenueComment')
go
insert into NotificationType values('ReviewForVenue')
go
insert into NotificationType values('PhotoForVenue')
go
insert into NotificationType values('VideoForVenue')
go
insert into NotificationType values('HumanNear')
go
insert into NotificationType values('CommentForHuman')
go
insert into NotificationType values('ReplyToHumanComment')
go
insert into NotificationType values('PhotoForHuman')
go
insert into NotificationType values('VideoForHuman')
go

print '*** Table: UserNotifications ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'UserNotifications')
begin
	drop table dbo.UserNotifications
end
create table dbo.UserNotifications
(
	Id int identity(1,1) not null primary key,
	UserId int not null constraint fk_UserNotifications_User foreign key references UserProfile(UserId),
	NotificationTypeId int not null constraint fk_UserNotifications_NotificationType foreign key references NotificationType(Id),
	ReferenceId int not null,
	StatusId int not null constraint fk_UserNotifications_Status foreign key references [Status](Id)
)
go

print '*** Table: UserUsers ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'UserUsers')
begin
	drop table dbo.UserUsers
end
create table dbo.UserUsers
(
	Id int identity(1,1) not null primary key,
	UserId int not null constraint fk_UserUsers_User foreign key references [UserProfile](UserId),
	RememberedId int not null constraint fk_UserUsers_Remembered foreign key references [UserProfile](UserId)
)
go

print '*** Table: UserMessages ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'UserMessages')
begin
	drop table dbo.UserMessages
end
create table dbo.UserMessages
(
	Id int identity(1,1) not null primary key,
	FromId int not null constraint fk_UserMessages_From foreign key references [UserProfile](UserId),
	ToId int not null constraint fk_UserMessages_To foreign key references [UserProfile](UserId),
	Msg nvarchar(2000) null,
	ConvoId uniqueidentifier null,
	StatusId int constraint fk_UserMessages_Status foreign key references [Status](Id),
	Created datetime not null
)
go



-----------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------
print '*** Table: SportCategory ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'SportCategory')
begin
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_Sport_SportCategory')
	begin
		alter table Sport drop constraint fk_Sport_SportCategory
	end

	drop table dbo.SportCategory
end
create table dbo.SportCategory
(
	Id int identity(1,1) not null primary key,
	Name nvarchar(50) not null
)

insert into SportCategory values('boardsports')
insert into SportCategory values('bikesports')
insert into SportCategory values('motorsports')
go

print '*** Table: Sport ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'Sport')
begin
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserSports_Sport')
	begin
		alter table UserSports drop constraint fk_UserSports_Sport
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_VenueSports_Sport')
	begin
		alter table VenueSports drop constraint fk_VenueSports_Sport
	end

	drop table dbo.Sport
end
create table dbo.Sport
(
	Id int identity(1,1) not null primary key,
	Name nvarchar(50) not null,
	Longname nvarchar(50) not null,
	SportCategoryId int not null constraint fk_Sport_SportCategory foreign key references SportCategory(Id)
)

insert into Sport values('skate', 'skateboard', 1)
insert into Sport values('snow', 'snowboard', 1)
insert into Sport values('wake', 'wakeboard', 1)
insert into Sport values('surf', 'surfboard', 1)
insert into Sport values('longboard', 'longboard', 1)
insert into Sport values('ski', 'ski', 1)
insert into Sport values('other board', 'other board', 1)
insert into Sport values('mountain', 'mountain biking', 2)
insert into Sport values('bmx', 'bmx biking', 2)
insert into Sport values('road', 'road biking', 2)
insert into Sport values('other bike', 'other bike', 2)
insert into Sport values('motorcycle', 'motorcycle', 3)
insert into Sport values('off-road', 'off road', 3)
insert into Sport values('snowmobile', 'snowmobiling', 3)
insert into Sport values('jetski', 'jetski', 3)
insert into Sport values('autox', 'autox', 3)
insert into Sport values('other motorsport', 'other motorsport', 3)
go

print '*** Table: Venue ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'Venue')
begin
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_VenueSports_Venue')
	begin
		alter table VenueSports drop constraint fk_VenueSports_Venue
	end
	if exists(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = N'fk_UserVenues_Venue')
	begin
		alter table UserVenues drop constraint fk_UserVenues_Venue
	end

	drop table dbo.Venue
end
create table dbo.Venue
(
	Id int identity(1,1) not null primary key,
	Name nvarchar(50) not null,
	[Address] nvarchar(250) not null,
	Address2 nvarchar(250) null,
	City nvarchar(150) not null,
	StateId int not null constraint fk_Venue_State foreign key references [State](Id),
	Zip nvarchar(15) null,
	[Description] text null,
	Lat float null,
	Lng float null,
	PlaceId nvarchar(200) null,
	Logo nvarchar(50) null
)
go

print '*** Table: UserVenues ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'UserVenues')
begin
	drop table dbo.UserVenues
end
create table dbo.UserVenues
(
	Id int identity(1,1) not null primary key,
	UserId int not null constraint fk_UserVenues_UserProfile foreign key references [UserProfile](UserId),
	VenueId int not null constraint fk_UserVenues_Venue foreign key references Venue(Id)
)
go

print '*** Table: VenueSports ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'VenueSports')
begin
	drop table dbo.VenueSports
end
create table dbo.VenueSports
(
	Id int identity(1,1) not null primary key,
	VenueId int not null constraint fk_VenueSports_Venue foreign key references Venue(Id),
	SportId int not null constraint fk_VenueSports_Sport foreign key references Sport(Id)
)
go

print '*** Table: UserSports ******************************************************'
if exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'UserSports')
begin
	drop table dbo.UserSports
end
create table dbo.UserSports
(
	Id int identity(1,1) not null primary key,
	UserId int not null constraint fk_UserSports_UserProfile foreign key references [UserProfile](UserId),
	SportId int not null constraint fk_UserSports_Sport foreign key references Sport(Id),
	Findable bit not null default 1,
	Note text null,
	SkillLevel int not null
)
go



-----------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------
print '*** DUMMY DATA ********************************************'
go

print '*** adding to webpages_Membership ***'
insert into [dbo].[webpages_Membership] values(1, '2015-05-02 02:09:12.887', '4x2h1HB1P-TUPfr8AmQwEA2', 1, NULL, 0, 'AKMWy6Jrm8Z8vxX4oFrkhbYY+f8bFhqv3viHPqi8LC6t30bu408xtrGnd+5MtFM/zA==', '2015-05-02 02:09:12.887', '', NULL, NULL)
insert into [dbo].[webpages_Membership] values(2, '2015-05-02 02:09:12.887', '4x2h1HB1P-TUPfr8AmQwEA2', 1, NULL, 0, 'AKMWy6Jrm8Z8vxX4oFrkhbYY+f8bFhqv3viHPqi8LC6t30bu408xtrGnd+5MtFM/zA==', '2015-05-02 02:09:12.887', '', NULL, NULL)
insert into [dbo].[webpages_Membership] values(3, '2015-05-02 02:09:12.887', '4x2h1HB1P-TUPfr8AmQwEA2', 1, NULL, 0, 'AKMWy6Jrm8Z8vxX4oFrkhbYY+f8bFhqv3viHPqi8LC6t30bu408xtrGnd+5MtFM/zA==', '2015-05-02 02:09:12.887', '', NULL, NULL)
insert into [dbo].[webpages_Membership] values(4, '2015-05-02 02:09:12.887', '4x2h1HB1P-TUPfr8AmQwEA2', 1, NULL, 0, 'AKMWy6Jrm8Z8vxX4oFrkhbYY+f8bFhqv3viHPqi8LC6t30bu408xtrGnd+5MtFM/zA==', '2015-05-02 02:09:12.887', '', NULL, NULL)
insert into [dbo].[webpages_Membership] values(5, '2015-05-02 02:09:12.887', '4x2h1HB1P-TUPfr8AmQwEA2', 1, NULL, 0, 'AKMWy6Jrm8Z8vxX4oFrkhbYY+f8bFhqv3viHPqi8LC6t30bu408xtrGnd+5MtFM/zA==', '2015-05-02 02:09:12.887', '', NULL, NULL)
insert into [dbo].[webpages_Membership] values(6, '2015-05-02 02:09:12.887', '4x2h1HB1P-TUPfr8AmQwEA2', 1, NULL, 0, 'AKMWy6Jrm8Z8vxX4oFrkhbYY+f8bFhqv3viHPqi8LC6t30bu408xtrGnd+5MtFM/zA==', '2015-05-02 02:09:12.887', '', NULL, NULL)
go

print '*** adding to userprofile ***'
insert into userprofile(Username, FirstName, ProfilePic, Gender, Birthdate, lat, lng) values('gil@gmail.com', 'Gil', 'profile_gil.jpg', 'M', '12/20/1974', 29.4824453, -98.5851413)
insert into userprofile(Username, FirstName, ProfilePic, Gender, Birthdate, lat, lng) values('gargoyo@hotmail.com', 'Gargoyle', 'profile_gargoyo.jpg', 'M', '12/20/1984', 29.5814305, -98.5144044)
insert into userprofile(Username, FirstName, ProfilePic, Gender, Birthdate, lat, lng) values('jinn@hotmail.com', 'Jinn', 'profile_jinn.jpg', 'M', '12/20/1994', 29.4334305, -98.5144044)
insert into userprofile(Username, FirstName, ProfilePic, Gender, Birthdate, lat, lng) values('cyberpunk@hotmail.com', 'Cyberpunk', 'profile_cyberpunk.jpg', 'M', '12/20/1964', 30.3028702, -97.7305704)
insert into userprofile(Username, FirstName, ProfilePic, Gender, Birthdate, lat, lng) values('hazard@hotmail.com', 'Hazard', 'profile_hazard.jpg', 'M', '12/20/1990', 30.3828702, -97.7305704)
insert into userprofile(Username, FirstName, ProfilePic, Gender, Birthdate, lat, lng) values('sheila@hotmail.com', 'Sheila', 'profile_sheila.jpg', 'F', '12/20/1999', 30.3928702, -97.7305704)
go

print '*** adding to UserSports ***'
insert into UserSports values(1,2,1,null,1)
insert into UserSports values(1,3,1,null,2)
insert into UserSports values(1,7,1,null,3)
insert into UserSports values(1,12,1,null,4)
insert into UserSports values(1,13,1,null,1)
insert into UserSports values(1,17,1,null,2)

insert into UserSports values(2,2,1,null,1)
insert into UserSports values(2,3,1,null,2)
insert into UserSports values(2,7,1,null,3)
insert into UserSports values(2,12,1,null,4)
insert into UserSports values(2,13,1,null,1)
insert into UserSports values(2,17,1,null,2)

insert into UserSports values(3,2,1,null,1)
insert into UserSports values(3,3,1,null,2)
insert into UserSports values(3,7,1,null,3)
insert into UserSports values(3,12,1,null,4)
insert into UserSports values(3,13,1,null,1)
insert into UserSports values(3,17,1,null,2)

insert into UserSports values(4,2,1,null,1)
insert into UserSports values(4,3,1,null,2)
insert into UserSports values(4,7,1,null,3)
insert into UserSports values(4,12,1,null,4)
insert into UserSports values(4,13,1,null,1)
insert into UserSports values(4,17,1,null,2)

insert into UserSports values(5,2,1,null,1)
insert into UserSports values(5,3,1,null,2)
insert into UserSports values(5,7,1,null,3)
insert into UserSports values(5,12,1,null,4)
insert into UserSports values(5,13,1,null,1)
insert into UserSports values(5,17,1,null,2)

insert into UserSports values(6,2,1,null,1)
insert into UserSports values(6,3,1,null,2)
insert into UserSports values(6,7,1,null,3)
insert into UserSports values(6,12,1,null,4)
insert into UserSports values(6,13,1,null,1)
insert into UserSports values(6,17,1,null,2)
go


print '*** adding to UserMessages ***'
declare @convo1 uniqueidentifier, @convo2 uniqueidentifier, @convo3 uniqueidentifier, @dateBase datetime
set @convo1 = newid()
set @convo2 = newid()
set @convo3 = newid()
set @dateBase = Cast('4/1/2015' as datetime)

insert into UserMessages values(1,2,'Hey gargie! What''s up!', @convo1, 3, @dateBase)
insert into UserMessages values(2,1,'Not much, just here convo1!', @convo1, 3, dateadd(day, 1, @dateBase))

insert into UserMessages values(1,3,'Hey! What''s up! U going to the party?', @convo2, 3, dateadd(day, 2, @dateBase))
insert into UserMessages values(3,1,'Not much, just here! convo2', @convo2, 1, dateadd(day, 3, @dateBase))

insert into UserMessages values(1,4,'Hey! What''s up! How the new bike!', @convo3, 3, dateadd(day, 4, @dateBase))
insert into UserMessages values(4,1,'Not much, just here! convo3', @convo3, 1, dateadd(day, 5, @dateBase))
go

/*
select * from venue
select * from userprofile

select	VenueId=v.Id
from venue v 
inner join VenueSports vs on vs.VenueId = v.Id 
inner join Sport s on s.Id = vs.SportId
where vs.SportId in (select sportid from usersports where userid = 3) 
	and v.Id not in (select ReferenceId from UserNotifications where UserId = 3 and ReferenceId = v.Id and NotificationTypeId = 1)
	and dbo.DistanceBetween(29.4825005, -98.5852444, v.Lat, v.Lng) < 100 
group by v.Id
order by v.Id

select	up.UserId
from UserProfile up
inner join UserSports us on us.UserId = up.UserId
inner join Sport s on s.Id = us.SportId
where us.SportId in (select sportid from usersports where userid = 1)
	and up.UserId not in (select ReferenceId from UserNotifications where UserId = 1 and ReferenceId = up.UserId and NotificationTypeId = 7)
	and dbo.DistanceBetween(29.4825005, -98.5852444, up.Lat, up.Lng) < 100 
	and up.UserId <> 1
group by up.UserId
order by up.UserId
*/

