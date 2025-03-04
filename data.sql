USE [Finquix]
GO
/****** Object:  Table [dbo].[CryptoAssets]    Script Date: 2/24/2025 9:26:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CryptoAssets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Symbol] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[CurrentPrice] [decimal](18, 2) NOT NULL,
	[MarketCap] [decimal](18, 2) NOT NULL,
	[ChangePercent] [decimal](18, 2) NOT NULL,
	[LastUpdated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_CryptoAssets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FinancialSignals]    Script Date: 2/24/2025 9:26:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FinancialSignals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Ticker] [nvarchar](max) NULL,
	[CompanyName] [nvarchar](max) NULL,
	[CurrentPrice] [decimal](18, 2) NOT NULL,
	[ChangePercent] [decimal](18, 2) NOT NULL,
	[SignalDate] [datetime2](7) NOT NULL,
	[Recommendation] [nvarchar](max) NULL,
 CONSTRAINT [PK_FinancialSignals] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockAssets]    Script Date: 2/24/2025 9:26:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockAssets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Symbol] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[CurrentPrice] [decimal](18, 2) NOT NULL,
	[ChangePercent] [decimal](18, 2) NOT NULL,
	[LastUpdated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_StockAssets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
