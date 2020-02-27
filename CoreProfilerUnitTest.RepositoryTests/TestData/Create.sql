CREATE TABLE [dbo].[YoubikeStation](
	[Id] [uniqueidentifier] NOT NULL,
	[StationNo] [nvarchar](10) NOT NULL,
	[StationName] [nvarchar](50) NOT NULL,
	[Total] [int] NOT NULL,
	[StationBikes] [int] NOT NULL,
	[StationArea] [nvarchar](10) NOT NULL,
	[ModifyTime] [nvarchar](20) NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[Address] [nvarchar](200) NOT NULL,
	[StationAreaEnglish] [nvarchar](50) NOT NULL,
	[StationNameEnglish] [nvarchar](100) NOT NULL,
	[AddressEnglish] [nvarchar](500) NOT NULL,
	[BikeEmpty] [int] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_YoubikeStation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

