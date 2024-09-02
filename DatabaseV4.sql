USE [TruckRental]
GO
/****** Object:  Table [dbo].[Achievement]    Script Date: 8/26/2024 6:44:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Achievement](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[Description] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_Achievement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AchievementDetails]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AchievementDetails](
	[Id] [int] NOT NULL,
	[AchievementId] [int] NULL,
	[BookingId] [int] NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_AchievementDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AchievementSetting]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AchievementSetting](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Tier] [int] NULL,
	[AwardWinningHook] [nvarchar](255) NULL,
	[IsActived] [bit] NULL,
 CONSTRAINT [PK_AchievementSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Booking]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booking](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[Deposit] [float] NULL,
	[Status] [nvarchar](255) NULL,
	[PickupAddress] [nvarchar](255) NULL,
	[PickupPoint] [nvarchar](255) NULL,
	[DeliveryAddress] [nvarchar](255) NULL,
	[DeliveryPoint] [nvarchar](255) NULL,
	[IsUseBox] [bit] NULL,
	[BoxType] [nvarchar](255) NULL,
	[EstimatedDistance] [nvarchar](255) NULL,
	[Total] [float] NULL,
	[TotalReal] [float] NULL,
	[EstimatedDeliveryTime] [nvarchar](255) NULL,
	[IsDeposited] [bit] NULL,
	[IsBonus] [bit] NULL,
	[IsReported] [bit] NULL,
	[ReportedReason] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[Review] [nvarchar](255) NULL,
	[Bonus] [nvarchar](255) NULL,
	[TypeBooking] [nvarchar](255) NULL,
	[EstimatedAcreage] [nvarchar](255) NULL,
	[RoomNumber] [nvarchar](255) NULL,
	[FloorsNumber] [nvarchar](255) NULL,
	[IsManyItems] [bit] NULL,
	[EstimatedTotalWeight] [nvarchar](255) NULL,
	[IsCancel] [bit] NULL,
	[CancelReason] [nvarchar](255) NULL,
	[EstimatedWeight] [nvarchar](255) NULL,
	[EstimatedHeight] [nvarchar](255) NULL,
	[EstimatedWidth] [nvarchar](255) NULL,
	[EstimatedLength] [nvarchar](255) NULL,
	[EstimatedVolume] [nvarchar](255) NULL,
	[IsPorter] [bit] NULL,
	[IsRoundTrip] [bit] NULL,
	[Note] [nvarchar](255) NULL,
	[TotalFee] [float] NULL,
	[FeeInfo] [nvarchar](255) NULL,
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingDetails]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingDetails](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[BookingId] [int] NULL,
	[Status] [nvarchar](255) NULL,
	[Price] [float] NULL,
	[StaffType] [nvarchar](255) NULL,
 CONSTRAINT [PK_BookingDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingItem]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingItem](
	[Id] [int] NOT NULL,
	[ItemId] [int] NULL,
	[BookingId] [int] NULL,
	[Status] [nvarchar](255) NULL,
	[Quantity] [int] NULL,
	[EstimatedWeight] [nvarchar](255) NULL,
	[EstimatedLenght] [nvarchar](255) NULL,
	[EstimatedWidth] [nvarchar](255) NULL,
	[EstimatedHeight] [nvarchar](255) NULL,
	[EstimatedVolume] [nvarchar](255) NULL,
 CONSTRAINT [PK_BookingItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingStaffDaily]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingStaffDaily](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[Status] [nvarchar](255) NULL,
	[AddressCurrent] [nvarchar](255) NULL,
	[IsActived] [bit] NULL,
	[CreatedAt] [date] NULL,
	[UpdatedAt] [date] NULL,
	[DurationTimeActived] [int] NULL,
 CONSTRAINT [PK_BookingStaffDaily] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingTracker]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingTracker](
	[Id] [int] NOT NULL,
	[BookingId] [int] NULL,
	[Time] [nvarchar](255) NULL,
	[Type] [nvarchar](255) NULL,
	[Location] [nvarchar](255) NULL,
	[Point] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Status] [nvarchar](255) NULL,
 CONSTRAINT [PK_BookingTracker] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FeeDetails]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeeDetails](
	[Id] [int] NOT NULL,
	[BookingId] [int] NULL,
	[FeeSettingId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Amount] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FeeSetting]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeeSetting](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Amount] [float] NULL,
	[IsActived] [bit] NULL,
	[Type] [nvarchar](255) NULL,
 CONSTRAINT [PK_FeeSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GroupRolePermission]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupRolePermission](
	[Id] [int] NOT NULL,
	[RoleId] [int] NULL,
	[PermissionId] [int] NULL,
 CONSTRAINT [PK_GroupRolePermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HouseType]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HouseType](
	[Id] [int] NOT NULL,
	[BookingId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Item]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Item](
	[Id] [int] NOT NULL,
	[ItemCategoryId] [int] NULL,
	[Price] [float] NULL,
	[Name] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
	[Description] [nvarchar](255) NULL,
	[Tier] [int] NULL,
	[ImgUrl] [nvarchar](255) NULL,
 CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ItemCategory]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemCategory](
	[Id] [int] NOT NULL,
	[Type] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[ImgUrl] [nvarchar](255) NULL,
 CONSTRAINT [PK_ItemCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[SentFrom] [nvarchar](255) NULL,
	[Receive] [nvarchar](255) NULL,
	[DeviceId] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Topic] [nvarchar](255) NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [int] NOT NULL,
	[BookingId] [int] NULL,
	[BankCode] [nvarchar](255) NULL,
	[BankTransNo] [nvarchar](255) NULL,
	[CardType] [nvarchar](255) NULL,
	[Amount] [float] NULL,
	[Token] [nvarchar](255) NULL,
	[ResponseCode] [nvarchar](255) NULL,
	[Success] [bit] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permission](
	[Id] [int] NOT NULL,
	[Src] [nvarchar](255) NULL,
	[TypePermission] [nvarchar](255) NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromotionCategory]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PromotionCategory](
	[Id] [int] NOT NULL,
	[IsPublic] [bit] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DiscountRate] [float] NULL,
	[DiscountMax] [float] NULL,
	[DiscountPrice] [float] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Type] [nvarchar](255) NULL,
	[Quantity] [int] NULL,
	[StartBookingTime] [datetime] NULL,
	[EndBookingTime] [datetime] NULL,
	[IsInfinite] [bit] NULL,
 CONSTRAINT [PK_PromotionCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromotionDetails]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PromotionDetails](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[BookingId] [int] NULL,
	[PromotionCategoryId] [int] NULL,
	[Price] [float] NULL,
	[Code] [nvarchar](255) NULL,
	[IsActived] [bit] NULL,
 CONSTRAINT [PK_PromotionDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Schedule]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedule](
	[Id] [int] NOT NULL,
	[IsActived] [bit] NULL,
	[IsDefault] [bit] NULL,
	[WorkOvertime] [int] NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleDetails]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleDetails](
	[Id] [int] NOT NULL,
	[BookingId] [int] NULL,
	[ScheduleId] [int] NULL,
	[StatisticalId] [int] NULL,
	[WorkingDays] [date] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DurationTime] [int] NULL,
	[Amount] [float] NULL,
 CONSTRAINT [PK_ScheduleDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Service]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[IsActived] [bit] NULL,
	[Tier] [int] NULL,
	[ImageUrl] [nvarchar](255) NULL,
 CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceBooking]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceBooking](
	[Id] [int] NOT NULL,
	[ServiceId] [int] NULL,
	[BookingId] [int] NULL,
	[Quantity] [int] NULL,
	[Total] [float] NULL,
 CONSTRAINT [PK_ServiceBooking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Statistical]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Statistical](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[Type] [nvarchar](255) NULL,
	[Shard] [nvarchar](255) NULL,
	[Week] [nvarchar](255) NULL,
	[Date] [date] NULL,
	[Total] [float] NULL,
	[Tier] [int] NULL,
 CONSTRAINT [PK_Statistical] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Token]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Token](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[Token] [nvarchar](255) NULL,
	[RefreshToken] [nvarchar](255) NULL,
	[TokenType] [nvarchar](255) NULL,
	[ExpirationDate] [date] NULL,
	[RefreshExpirationDate] [date] NULL,
	[IsMobile] [bit] NULL,
 CONSTRAINT [PK_Token] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrackerSource]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrackerSource](
	[Id] [int] NOT NULL,
	[BookingTrackerId] [int] NULL,
	[ResourceUrl] [nvarchar](255) NULL,
	[ResourceCode] [nvarchar](255) NULL,
	[Type] [nvarchar](255) NULL,
 CONSTRAINT [PK_TrackerImg] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transaction]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transaction](
	[Id] [int] NOT NULL,
	[PaymentId] [int] NULL,
	[WalletId] [int] NULL,
	[Resource] [nvarchar](255) NULL,
	[Amount] [float] NULL,
	[Status] [nvarchar](255) NULL,
	[Substance] [nvarchar](255) NULL,
	[PaymentMethod] [nvarchar](255) NULL,
	[TransactionCode] [nvarchar](255) NULL,
	[FailedReason] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Truck]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Truck](
	[Id] [int] NOT NULL,
	[TruckCategoryId] [int] NULL,
	[Model] [nvarchar](255) NULL,
	[NumberPlate] [nvarchar](255) NULL,
	[Capacity] [float] NULL,
	[IsAvailable] [bit] NULL,
	[Brand] [nvarchar](255) NULL,
	[Color] [nvarchar](255) NULL,
	[IsInsurrance] [bit] NULL,
	[UserId] [int] NULL,
 CONSTRAINT [PK_Vehicle] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TruckCategory]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TruckCategory](
	[Id] [int] NOT NULL,
	[CategoryName] [nvarchar](255) NULL,
	[MaxLoad] [float] NULL,
	[Description] [nvarchar](255) NULL,
	[ImgUrl] [nvarchar](255) NULL,
	[EstimatedLength] [nvarchar](255) NULL,
	[EstimatedWidth] [nvarchar](255) NULL,
	[EstimatedHeight] [nvarchar](255) NULL,
	[Summarize] [nvarchar](255) NULL,
	[Price] [float] NULL,
	[TotalTrips] [int] NULL,
 CONSTRAINT [PK_VehicleCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TruckImg]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TruckImg](
	[Id] [int] NOT NULL,
	[TruckId] [int] NULL,
	[ImageUrl] [nvarchar](255) NULL,
	[ImageCode] [nvarchar](255) NULL,
 CONSTRAINT [PK_VehicleImg] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] NOT NULL,
	[ScheduleId] [int] NULL,
	[RoleId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Phone] [nvarchar](255) NULL,
	[Password] [nvarchar](255) NULL,
	[Gender] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[AvatarUrl] [nvarchar](255) NULL,
	[Dob] [date] NULL,
	[IsBanned] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[ModifiedVersion] [int] NULL,
	[TotalTrips] [int] NULL,
	[IsDriver] [bit] NULL,
	[CodeIntroduce] [nvarchar](255) NULL,
	[NumberIntroduce] [nvarchar](255) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserInfo]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInfo](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[Type] [nvarchar](255) NULL,
	[ImgUrl] [nvarchar](255) NULL,
	[Code] [nvarchar](255) NULL,
	[Cavet] [nvarchar](255) NULL,
	[HealthInsurance] [nvarchar](255) NULL,
	[CitizenIdentification] [nvarchar](255) NULL,
	[HealthCertificate] [nvarchar](255) NULL,
	[License] [nvarchar](255) NULL,
	[PermanentAddress] [nvarchar](255) NULL,
	[TemporaryResidenceAddress] [nvarchar](255) NULL,
	[CurriculumVitae] [nvarchar](255) NULL,
 CONSTRAINT [PK_UserInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wallet]    Script Date: 8/26/2024 6:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wallet](
	[Id] [int] NOT NULL,
	[UserId] [int] NULL,
	[Balance] [float] NULL,
	[Tier] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[IsLocked] [bit] NULL,
	[LockReason] [nvarchar](255) NULL,
	[LockAmount] [float] NULL,
	[Type] [nvarchar](255) NULL,
 CONSTRAINT [PK_Wallet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Achievement]  WITH CHECK ADD  CONSTRAINT [FK_Achievement_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Achievement] CHECK CONSTRAINT [FK_Achievement_User]
GO
ALTER TABLE [dbo].[AchievementDetails]  WITH CHECK ADD  CONSTRAINT [FK_AchievementDetails_Achievement] FOREIGN KEY([AchievementId])
REFERENCES [dbo].[Achievement] ([Id])
GO
ALTER TABLE [dbo].[AchievementDetails] CHECK CONSTRAINT [FK_AchievementDetails_Achievement]
GO
ALTER TABLE [dbo].[AchievementDetails]  WITH CHECK ADD  CONSTRAINT [FK_AchievementDetails_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[AchievementDetails] CHECK CONSTRAINT [FK_AchievementDetails_Booking]
GO
ALTER TABLE [dbo].[AchievementSetting]  WITH CHECK ADD  CONSTRAINT [FK_AchievementSetting_AchievementSetting] FOREIGN KEY([Id])
REFERENCES [dbo].[AchievementSetting] ([Id])
GO
ALTER TABLE [dbo].[AchievementSetting] CHECK CONSTRAINT [FK_AchievementSetting_AchievementSetting]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_User]
GO
ALTER TABLE [dbo].[BookingDetails]  WITH CHECK ADD  CONSTRAINT [FK_BookingDetails_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[BookingDetails] CHECK CONSTRAINT [FK_BookingDetails_Booking]
GO
ALTER TABLE [dbo].[BookingDetails]  WITH CHECK ADD  CONSTRAINT [FK_BookingDetails_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BookingDetails] CHECK CONSTRAINT [FK_BookingDetails_User]
GO
ALTER TABLE [dbo].[BookingItem]  WITH CHECK ADD  CONSTRAINT [FK_BookingItem_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[BookingItem] CHECK CONSTRAINT [FK_BookingItem_Booking]
GO
ALTER TABLE [dbo].[BookingItem]  WITH CHECK ADD  CONSTRAINT [FK_BookingItem_Item] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Item] ([Id])
GO
ALTER TABLE [dbo].[BookingItem] CHECK CONSTRAINT [FK_BookingItem_Item]
GO
ALTER TABLE [dbo].[BookingStaffDaily]  WITH CHECK ADD  CONSTRAINT [FK_BookingStaffDaily_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BookingStaffDaily] CHECK CONSTRAINT [FK_BookingStaffDaily_User]
GO
ALTER TABLE [dbo].[BookingTracker]  WITH CHECK ADD  CONSTRAINT [FK_BookingTracker_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[BookingTracker] CHECK CONSTRAINT [FK_BookingTracker_Booking]
GO
ALTER TABLE [dbo].[FeeDetails]  WITH CHECK ADD  CONSTRAINT [FK_FeeDetails_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[FeeDetails] CHECK CONSTRAINT [FK_FeeDetails_Booking]
GO
ALTER TABLE [dbo].[FeeDetails]  WITH CHECK ADD  CONSTRAINT [FK_FeeDetails_FeeSetting] FOREIGN KEY([FeeSettingId])
REFERENCES [dbo].[FeeSetting] ([Id])
GO
ALTER TABLE [dbo].[FeeDetails] CHECK CONSTRAINT [FK_FeeDetails_FeeSetting]
GO
ALTER TABLE [dbo].[GroupRolePermission]  WITH CHECK ADD  CONSTRAINT [FK_GroupRolePermission_Permission] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([Id])
GO
ALTER TABLE [dbo].[GroupRolePermission] CHECK CONSTRAINT [FK_GroupRolePermission_Permission]
GO
ALTER TABLE [dbo].[GroupRolePermission]  WITH CHECK ADD  CONSTRAINT [FK_GroupRolePermission_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[GroupRolePermission] CHECK CONSTRAINT [FK_GroupRolePermission_Role]
GO
ALTER TABLE [dbo].[HouseType]  WITH CHECK ADD  CONSTRAINT [FK_HouseType_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[HouseType] CHECK CONSTRAINT [FK_HouseType_Booking]
GO
ALTER TABLE [dbo].[Item]  WITH CHECK ADD  CONSTRAINT [FK_Item_Item] FOREIGN KEY([Id])
REFERENCES [dbo].[Item] ([Id])
GO
ALTER TABLE [dbo].[Item] CHECK CONSTRAINT [FK_Item_Item]
GO
ALTER TABLE [dbo].[Item]  WITH CHECK ADD  CONSTRAINT [FK_Item_ItemCategory] FOREIGN KEY([ItemCategoryId])
REFERENCES [dbo].[ItemCategory] ([Id])
GO
ALTER TABLE [dbo].[Item] CHECK CONSTRAINT [FK_Item_ItemCategory]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_User]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Booking]
GO
ALTER TABLE [dbo].[PromotionDetails]  WITH CHECK ADD  CONSTRAINT [FK_PromotionDetails_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[PromotionDetails] CHECK CONSTRAINT [FK_PromotionDetails_Booking]
GO
ALTER TABLE [dbo].[PromotionDetails]  WITH CHECK ADD  CONSTRAINT [FK_PromotionDetails_PromotionCategory] FOREIGN KEY([PromotionCategoryId])
REFERENCES [dbo].[PromotionCategory] ([Id])
GO
ALTER TABLE [dbo].[PromotionDetails] CHECK CONSTRAINT [FK_PromotionDetails_PromotionCategory]
GO
ALTER TABLE [dbo].[PromotionDetails]  WITH CHECK ADD  CONSTRAINT [FK_PromotionDetails_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[PromotionDetails] CHECK CONSTRAINT [FK_PromotionDetails_User]
GO
ALTER TABLE [dbo].[ScheduleDetails]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleDetails_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[ScheduleDetails] CHECK CONSTRAINT [FK_ScheduleDetails_Booking]
GO
ALTER TABLE [dbo].[ScheduleDetails]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleDetails_Schedule] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[Schedule] ([Id])
GO
ALTER TABLE [dbo].[ScheduleDetails] CHECK CONSTRAINT [FK_ScheduleDetails_Schedule]
GO
ALTER TABLE [dbo].[ScheduleDetails]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleDetails_Statistical] FOREIGN KEY([StatisticalId])
REFERENCES [dbo].[Statistical] ([Id])
GO
ALTER TABLE [dbo].[ScheduleDetails] CHECK CONSTRAINT [FK_ScheduleDetails_Statistical]
GO
ALTER TABLE [dbo].[Service]  WITH CHECK ADD  CONSTRAINT [FK_Service_Service] FOREIGN KEY([Id])
REFERENCES [dbo].[Service] ([Id])
GO
ALTER TABLE [dbo].[Service] CHECK CONSTRAINT [FK_Service_Service]
GO
ALTER TABLE [dbo].[ServiceBooking]  WITH CHECK ADD  CONSTRAINT [FK_ServiceBooking_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[ServiceBooking] CHECK CONSTRAINT [FK_ServiceBooking_Booking]
GO
ALTER TABLE [dbo].[ServiceBooking]  WITH CHECK ADD  CONSTRAINT [FK_ServiceBooking_Service] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Service] ([Id])
GO
ALTER TABLE [dbo].[ServiceBooking] CHECK CONSTRAINT [FK_ServiceBooking_Service]
GO
ALTER TABLE [dbo].[Statistical]  WITH CHECK ADD  CONSTRAINT [FK_Statistical_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Statistical] CHECK CONSTRAINT [FK_Statistical_User]
GO
ALTER TABLE [dbo].[Token]  WITH CHECK ADD  CONSTRAINT [FK_Token_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Token] CHECK CONSTRAINT [FK_Token_User]
GO
ALTER TABLE [dbo].[TrackerSource]  WITH CHECK ADD  CONSTRAINT [FK_TrackerSource_BookingTracker] FOREIGN KEY([BookingTrackerId])
REFERENCES [dbo].[BookingTracker] ([Id])
GO
ALTER TABLE [dbo].[TrackerSource] CHECK CONSTRAINT [FK_TrackerSource_BookingTracker]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Payment] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payment] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_Payment]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Wallet] FOREIGN KEY([WalletId])
REFERENCES [dbo].[Wallet] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_Wallet]
GO
ALTER TABLE [dbo].[Truck]  WITH CHECK ADD  CONSTRAINT [FK_Truck_TruckCategory] FOREIGN KEY([TruckCategoryId])
REFERENCES [dbo].[TruckCategory] ([Id])
GO
ALTER TABLE [dbo].[Truck] CHECK CONSTRAINT [FK_Truck_TruckCategory]
GO
ALTER TABLE [dbo].[Truck]  WITH CHECK ADD  CONSTRAINT [FK_Truck_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Truck] CHECK CONSTRAINT [FK_Truck_User]
GO
ALTER TABLE [dbo].[TruckImg]  WITH CHECK ADD  CONSTRAINT [FK_TruckImg_Truck] FOREIGN KEY([TruckId])
REFERENCES [dbo].[Truck] ([Id])
GO
ALTER TABLE [dbo].[TruckImg] CHECK CONSTRAINT [FK_TruckImg_Truck]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Role]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Schedule] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[Schedule] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Schedule]
GO
ALTER TABLE [dbo].[UserInfo]  WITH CHECK ADD  CONSTRAINT [FK_UserInfo_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserInfo] CHECK CONSTRAINT [FK_UserInfo_User]
GO
ALTER TABLE [dbo].[Wallet]  WITH CHECK ADD  CONSTRAINT [FK_Wallet_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Wallet] CHECK CONSTRAINT [FK_Wallet_User]
GO
