USE [MoveMate-Dev]
GO
/****** Object:  Table [dbo].[Assignment]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Assignment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[BookingDetailsId] [int] NULL,
	[TruckId] [int] NULL,
	[BookingId] [int] NULL,
	[ScheduleBookingId] [int] NULL,
	[Status] [nvarchar](255) NULL,
	[Price] [float] NULL,
	[StaffType] [nvarchar](255) NULL,
	[IsResponsible] [bit] NULL,
	[AddressCurrent] [nvarchar](255) NULL,
	[FailedReason] [nvarchar](255) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DurationTime] [float] NULL,
	[IsRoundTripCompleted] [bit] NULL,
	[StarReview] [float] NULL,
	[Review] [nvarchar](255) NULL,
	[Bonus] [float] NULL,
 CONSTRAINT [PK_Assignment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Booking]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[HouseTypeId] [int] NULL,
	[Deposit] [float] NULL,
	[Status] [nvarchar](255) NULL,
	[PickupAddress] [nvarchar](255) NULL,
	[PickupPoint] [nvarchar](255) NULL,
	[DeliveryAddress] [nvarchar](255) NULL,
	[DeliveryPoint] [nvarchar](255) NULL,
	[EstimatedDistance] [nvarchar](255) NULL,
	[Total] [float] NULL,
	[TotalReal] [float] NULL,
	[EstimatedDeliveryTime] [float] NULL,
	[IsDeposited] [bit] NULL,
	[IsReported] [bit] NULL,
	[ReportedReason] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[Review] [nvarchar](255) NULL,
	[TypeBooking] [nvarchar](255) NULL,
	[RoomNumber] [nvarchar](255) NULL,
	[FloorsNumber] [nvarchar](255) NULL,
	[IsManyItems] [nvarchar](255) NULL,
	[IsCancel] [bit] NULL,
	[CancelReason] [nvarchar](255) NULL,
	[IsPorter] [bit] NULL,
	[IsRoundTrip] [bit] NULL,
	[Note] [nvarchar](255) NULL,
	[TotalFee] [float] NULL,
	[BookingAt] [datetime] NULL,
	[IsReviewOnline] [bit] NULL,
	[IsUserConfirm] [bit] NULL,
	[DriverNumber] [int] NULL,
	[PorterNumber] [int] NULL,
	[TruckNumber] [int] NULL,
	[ReviewAt] [datetime] NULL,
	[EstimatedEndTime] [datetime] NULL,
	[IsStaffReviewed] [bit] NULL,
	[IsUpdated] [bit] NULL,
	[IsCredit] [bit] NULL,
	[IsInsurance] [bit] NULL,
	[IsRefunded] [bit] NULL,
	[ReasonRefundFailed] [nvarchar](255) NULL,
	[RefundAt] [datetime] NULL,
	[TotalRefund] [float] NULL,
	[Shard] [nvarchar](255) NULL,
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingDetails]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServiceId] [int] NULL,
	[BookingId] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Price] [float] NULL,
	[Quantity] [int] NULL,
	[Type] [nvarchar](255) NULL,
	[Status] [nvarchar](255) NULL,
	[IsRoundTripCompleted] [bit] NULL,
 CONSTRAINT [PK_BookingDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingTracker]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingTracker](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BookingId] [int] NULL,
	[Time] [nvarchar](255) NULL,
	[Type] [nvarchar](255) NULL,
	[Location] [nvarchar](255) NULL,
	[Point] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Status] [nvarchar](255) NULL,
	[Title] [nvarchar](255) NULL,
	[EstimatedAmount] [float] NULL,
	[RealAmount] [float] NULL,
	[IsInsurance] [bit] NULL,
	[FailedReason] [nvarchar](255) NULL,
	[IsCompensation] [bit] NULL,
 CONSTRAINT [PK_BookingTracker] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FeeDetails]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeeDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BookingId] [int] NULL,
	[FeeSettingId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Amount] [float] NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_FeeDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FeeSetting]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeeSetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServiceId] [int] NULL,
	[HouseTypeId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Amount] [float] NULL,
	[IsActived] [bit] NULL,
	[Type] [nvarchar](255) NULL,
	[Unit] [nvarchar](255) NULL,
	[RangeMin] [float] NULL,
	[RangeMax] [float] NULL,
	[DiscountRate] [nvarchar](255) NULL,
	[FloorPercentage] [float] NULL,
 CONSTRAINT [PK_FeeSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Group]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Group](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[IsActived] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[DurationTimeActived] [int] NULL,
 CONSTRAINT [PK_BookingStaffDaily] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HolidaySetting]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HolidaySetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Day] [date] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_HolidaySetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HouseType]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HouseType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[IsActived] [bit] NULL,
 CONSTRAINT [PK_HouseType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[SentFrom] [nvarchar](255) NULL,
	[Receive] [nvarchar](255) NULL,
	[DeviceId] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Topic] [nvarchar](255) NULL,
	[FcmToken] [nvarchar](255) NULL,
	[IsRead] [bit] NULL,
	[BookingId] [int] NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
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
	[Date] [datetime] NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromotionCategory]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PromotionCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IsPublic] [bit] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DiscountRate] [float] NULL,
	[DiscountMax] [float] NULL,
	[RequireMin] [float] NULL,
	[DiscountMin] [float] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Type] [nvarchar](255) NULL,
	[Quantity] [int] NULL,
	[StartBookingTime] [datetime] NULL,
	[EndBookingTime] [datetime] NULL,
	[IsInfinite] [bit] NULL,
	[ServiceId] [int] NULL,
	[IsDeleted] [bit] NULL,
	[Shard] [nvarchar](255) NULL,
	[Amount] [float] NULL,
 CONSTRAINT [PK_PromotionCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Schedule]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedule](
	[Id] [int] NOT NULL,
	[Date] [date] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleBooking]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleBooking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IsActived] [bit] NULL,
	[Shard] [nvarchar](255) NULL,
 CONSTRAINT [PK_ScheduleBooking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleWorking]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleWorking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[IsActived] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[DurationTimeActived] [int] NULL,
	[Type] [nvarchar](255) NULL,
	[StartDate] [time](7) NULL,
	[EndDate] [time](7) NULL,
	[GroupId] [int] NULL,
	[ExtentStartDate] [time](7) NULL,
	[ExtentEndDate] [time](7) NULL,
	[ScheduleId] [int] NULL,
 CONSTRAINT [PK_ScheduleWorking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Service]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[IsActived] [bit] NULL,
	[Tier] [int] NULL,
	[ImageUrl] [nvarchar](255) NULL,
	[DiscountRate] [float] NULL,
	[Amount] [float] NULL,
	[ParentServiceId] [int] NULL,
	[Type] [nvarchar](255) NULL,
	[IsQuantity] [bit] NULL,
	[QuantityMax] [int] NULL,
	[TruckCategoryId] [int] NULL,
 CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrackerSource]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrackerSource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BookingTrackerId] [int] NULL,
	[ResourceUrl] [nvarchar](255) NULL,
	[ResourceCode] [nvarchar](255) NULL,
	[Type] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_TrackerImg] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transaction]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentId] [int] NULL,
	[WalletId] [int] NULL,
	[Resource] [nvarchar](255) NULL,
	[Amount] [float] NULL,
	[Status] [nvarchar](255) NULL,
	[Substance] [nvarchar](255) NULL,
	[PaymentMethod] [nvarchar](255) NULL,
	[TransactionCode] [nvarchar](255) NULL,
	[TransactionType] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
	[IsCredit] [bit] NULL,
	[Shard] [nvarchar](255) NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Truck]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Truck](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TruckCategoryId] [int] NULL,
	[Model] [nvarchar](255) NULL,
	[NumberPlate] [nvarchar](255) NULL,
	[Capacity] [float] NULL,
	[IsAvailable] [bit] NULL,
	[Brand] [nvarchar](255) NULL,
	[Color] [nvarchar](255) NULL,
	[IsInsurrance] [bit] NULL,
	[UserId] [int] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Truck] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TruckCategory]    Script Date: 12/7/2024 2:44:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TruckCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](255) NULL,
	[MaxLoad] [float] NULL,
	[Description] [nvarchar](255) NULL,
	[Summarize] [nvarchar](255) NULL,
	[ImageUrl] [nvarchar](255) NULL,
	[Price] [float] NULL,
	[TotalTrips] [int] NULL,
	[EstimatedLenght] [nvarchar](255) NULL,
	[EstimatedWidth] [nvarchar](255) NULL,
	[EstimatedHeight] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_TruckCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TruckImg]    Script Date: 12/7/2024 2:44:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TruckImg](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TruckId] [int] NULL,
	[ImageUrl] [nvarchar](255) NULL,
	[ImageCode] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 12/7/2024 2:44:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Phone] [nvarchar](255) NULL,
	[Password] [nvarchar](255) NULL,
	[Gender] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[AvatarUrl] [nvarchar](255) NULL,
	[Dob] [datetime] NULL,
	[IsBanned] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[ModifiedVersion] [int] NULL,
	[IsInitUsed] [bit] NULL,
	[IsDriver] [bit] NULL,
	[CodeIntroduce] [nvarchar](255) NULL,
	[NumberIntroduce] [nvarchar](255) NULL,
	[GroupId] [int] NULL,
	[Shard] [nvarchar](255) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserInfo]    Script Date: 12/7/2024 2:44:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Type] [nvarchar](255) NULL,
	[ImageUrl] [nvarchar](255) NULL,
	[Value] [nvarchar](255) NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Voucher]    Script Date: 12/7/2024 2:44:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Voucher](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[PromotionCategoryId] [int] NULL,
	[BookingId] [int] NULL,
	[Price] [float] NULL,
	[Code] [nvarchar](255) NULL,
	[IsActived] [bit] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Voucher] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wallet]    Script Date: 12/7/2024 2:44:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wallet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Balance] [float] NULL,
	[Tier] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[IsLocked] [bit] NULL,
	[LockReason] [nvarchar](255) NULL,
	[LockAmount] [float] NULL,
	[Type] [nvarchar](255) NULL,
	[FixedSalary] [float] NULL,
	[BankNumber] [nvarchar](255) NULL,
	[BankName] [nvarchar](255) NULL,
	[ExpirdAt] [nvarchar](255) NULL,
	[CardHolderName] [nvarchar](255) NULL,
 CONSTRAINT [PK_Wallet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FeeSetting] ON 
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (1, 12, NULL, N'Phí di chuyển dành cho xe tải 1000kg', NULL, 148500, 1, N'TRUCK', N'KM', 0, 4, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (2, 12, NULL, N'Phí di chuyển dành cho xe tải 1000kg từ 10km đến 15km', NULL, 9720, 1, N'TRUCK', N'KM', 10, 15, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (3, 12, NULL, N'Phí di chuyển dành cho xe tải 1000kg từ 4km đến 10km', NULL, 12420, 1, N'TRUCK', N'KM', 4, 10, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (4, 3, 2, N'Phí bốc vác bởi người hỗ trợ cho nhà riêng hoặc biệt thự', NULL, 400000, 1, N'PORTER', N'FLOOR', 0, 3, N'0', 0)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (5, 3, 2, N'Phí bốc vác bởi người hỗ trợ cho nhà riêng hoặc biệt thự có 3 đến 5 tầng', NULL, 400000, 1, N'PORTER', N'FLOOR', 3, 5, N'0', 20)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (6, 3, 2, N'Phí bốc vác bởi người hỗ trợ cho nhà riêng hoặc biệt thự có 5 tầng trở lên', NULL, 400000, 1, N'PORTER', N'FLOOR', 5, NULL, N'0', 25)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (7, 2, 2, N'Phí bốc vác bởi tài xế cho nhà riêng hoặc biệt thự', NULL, 120000, 1, N'DRIVER', N'FLOOR', 0, 3, N'0', 0)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (8, 2, 2, N'Phí bốc vác bởi tài xế cho nhà riêng hoặc biệt thự có 3 đến 5 tầng', NULL, 120000, 1, N'DRIVER', N'FLOOR', 3, 5, N'0', 20)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (9, 2, 2, N'Phí bốc vác bởi tài xế cho nhà riêng hoặc biệt thự có 5 tầng', NULL, 168000, 1, N'DRIVER', N'FLOOR', 5, NULL, N'0', 25)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (10, NULL, NULL, N'Phí di chuyển của người hỗ trợ', NULL, 100000, 1, N'REVIEWER', NULL, NULL, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (11, NULL, NULL, N'Phí cuối tuần', N'Phí cuối tuần.', 100000, 1, N'WEEKEND', NULL, NULL, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (12, NULL, NULL, N'Phí ngoài giờ hành chánh', N'Phí ngoài giờ hành chánh', 50000, 1, N'OUTSIDE_BUSINESS_HOURS', NULL, NULL, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (13, NULL, NULL, N'Phí khứ hồi', N'Phí khứ hồi', 70, 1, N'SYSTEM', N'PERCENT', NULL, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (14, 16, NULL, N'Phí di chuyển dành cho xe van 500kg', NULL, 111780, 1, N'TRUCK', N'KM', 0, 4, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (15, 16, NULL, N'Phí di chuyển dành cho xe van 500kg từ 4km đến 10km', NULL, 10800, 1, N'TRUCK', N'KM', 4, 10, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (16, 16, NULL, N'Phí di chuyển dành cho xe van 500kg từ 10km đến 15km', NULL, 7560, 1, N'TRUCK', N'KM', 10, 15, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (17, 16, NULL, N'Phí di chuyển dành cho xe van 500kg từ 15km đến 45km', NULL, 5940, 1, N'TRUCK', N'KM', 15, 45, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (18, 16, NULL, N'Phí di chuyển dành cho xe van 500kg từ 45km', NULL, 4860, 1, N'TRUCK', N'KM', 45, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (19, 17, NULL, N'Phí di chuyển dành cho xe van 1000kg', NULL, 148500, 1, N'TRUCK', N'KM', 0, 4, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (20, 17, NULL, N'Phí di chuyển dành cho xe van 1000kg từ 4km đến 10km', NULL, 12420, 1, N'TRUCK', N'KM', 4, 10, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (21, 17, NULL, N'Phí di chuyển dành cho xe van 1000kg từ 10km đến 15km', NULL, 9720, 1, N'TRUCK', N'KM', 10, 15, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (22, 17, NULL, N'Phí di chuyển dành cho xe van 1000kg từ 15km đến 45km', NULL, 7020, 1, N'TRUCK', N'KM', 15, 45, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (23, 17, NULL, N'Phí di chuyển dành cho xe van 1000kg từ 45km', NULL, 5400, 1, N'TRUCK', N'KM', 45, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (24, 15, NULL, N'Phí di chuyển dành cho xe tải 500kg', NULL, 111780, 1, N'TRUCK', N'KM', 0, 4, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (25, 15, NULL, N'Phí di chuyển dành cho xe tải 500kg từ 4km đến 10km', NULL, 10800, 1, N'TRUCK', N'KM', 4, 10, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (26, 15, NULL, N'Phí di chuyển dành cho xe tải 500kg từ 10km đến 15km', NULL, 7560, 1, N'TRUCK', N'KM', 10, 15, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (27, 15, NULL, N'Phí di chuyển dành cho xe tải 500kg từ 15km đến 45km', NULL, 5940, 1, N'TRUCK', N'KM', 15, 45, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (28, 15, NULL, N'Phí di chuyển dành cho xe tải 500kg từ 45km', NULL, 4860, 1, N'TRUCK', N'KM', 45, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (29, 13, NULL, N'Phí di chuyển dành cho xe tải 2000kg', NULL, 260280, 1, N'TRUCK', N'KM', 0, 4, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (30, 13, NULL, N'Phí di chuyển dành cho xe tải 2000kg từ 4km đến 10km', NULL, 13500, 1, N'TRUCK', N'KM', 4, 10, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (31, 13, NULL, N'Phí di chuyển dành cho xe tải 2000kg từ 10km đến 15km', NULL, 10800, 1, N'TRUCK', N'KM', 10, 15, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (32, 13, NULL, N'Phí di chuyển dành cho xe tải 2000kg từ 15km đến 45km', NULL, 7020, 1, N'TRUCK', N'KM', 15, 45, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (33, 13, NULL, N'Phí di chuyển dành cho xe tải 2000kg từ 45km', NULL, 5940, 1, N'TRUCK', N'KM', 45, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (34, 14, NULL, N'Phí di chuyển dành cho xe tải 2500kg', NULL, 355860, 1, N'TRUCK', N'KM', 0, 4, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (35, 14, NULL, N'Phí di chuyển dành cho xe tải 2500kg từ 4km đến 10km', NULL, 15120, 1, N'TRUCK', N'KM', 4, 10, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (36, 14, NULL, N'Phí di chuyển dành cho xe tải 2500kg từ 10km đến 30km', NULL, 12420, 1, N'TRUCK', N'KM', 10, 30, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (37, 14, NULL, N'Phí di chuyển dành cho xe tải 2500kg từ 30km đến 50km', NULL, 9720, 1, N'TRUCK', N'KM', 30, 50, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (38, 14, NULL, N'Phí di chuyển dành cho xe tải 2500kg từ 50km đến 100km', NULL, 9180, 1, N'TRUCK', N'KM', 50, 100, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (39, 14, NULL, N'Phí di chuyển dành cho xe tải 2500kg trên 100km', NULL, 7020, 1, N'TRUCK', N'KM', 100, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (40, 3, 3, N'Phí bốc vác bởi người hỗ trợ cho căn hộ hoặc chung cư', NULL, 400000, 1, N'PORTER', N'FLOOR', 0, 3, N'0', 0)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (41, 3, 3, N'Phí bốc vác bởi người hỗ trợ cho căn hộ hoặc chung cư có 3 đến 5 tầng', NULL, 400000, 1, N'PORTER', N'FLOOR', 3, 5, N'0', 20)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (42, 3, 3, N'Phí bốc vác bởi người hỗ trợ cho căn hộ hoặc chung cư có 5 tầng', NULL, 400000, 1, N'PORTER', N'FLOOR', 5, NULL, N'0', 25)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (43, 2, 3, N'Phí bốc vác bởi tài xế cho căn hộ hoặc chung cư', NULL, 120000, 1, N'DRIVER', N'FLOOR', 0, 3, N'0', 0)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (44, 2, 3, N'Phí bốc vác bởi tài xế cho căn hộ hoặc chung cư có 3 đến 5 tầng', NULL, 120000, 1, N'DRIVER', N'FLOOR', 3, 5, N'0', 20)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (45, 2, 3, N'Phí bốc vác bởi tài xế cho căn hộ hoặc chung cư có 5 tầng', NULL, 120000, 1, N'DRIVER', N'FLOOR', 5, NULL, N'0', 25)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (46, 3, 4, N'Phí bốc vác bởi người hỗ trợ cho công ty', NULL, 420000, 1, N'PORTER ', N'FLOOR', 0, 3, N'0', 0)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (47, 3, 4, N'Phí bốc vác bởi người hỗ trợ cho công ty có 3 đến 5 tầng', NULL, 420000, 1, N'PORTER', N'FLOOR', 3, 5, N'0', 20)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (48, 3, 4, N'Phí bốc vác bởi người hỗ trợ cho công ty có 5 tầng', NULL, 420000, 1, N'PORTER', N'FLOOR', 5, NULL, N'0', 25)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (49, 2, 4, N'Phí bốc vác bởi tài xế cho công ty ', NULL, 140000, 1, N'DRIVER', N'FLOOR', 0, 3, N'0', 0)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (50, 2, 4, N'Phí bốc vác bởi tài xế cho công ty có 3 đến 5 tầng', NULL, 140000, 1, N'DRIVER', N'FLOOR', 3, 5, N'0', 20)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (51, 2, 4, N'Phí bốc vác bởi tài xế cho công ty có 5 tầng', NULL, 140000, 1, N'DRIVER', N'FLOOR', 5, NULL, N'0', 25)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (52, 3, 1, N'Phí bốc vác bởi người hỗ trợ cho phòng trọ', NULL, 400000, 1, N'PORTER', N'FLOOR', 0, 3, N'0', 0)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (53, 3, 1, N'Phí bốc vác bởi người hỗ trợ cho phòng trọ có 3 đến 5 tầng', NULL, 400000, 1, N'PORTER', N'FLOOR', 3, 5, N'0', 20)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (54, 3, 1, N'Phí bốc vác bởi người hỗ trợ cho phòng trọ có 5 tầng', NULL, 400000, 1, N'PORTER', N'FLOOR', 5, NULL, N'0', 25)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (55, 2, 1, N'Phí bốc vác bởi tài xế cho phòng trọ', NULL, 120000, 1, N'DRIVER', N'FLOOR', 0, 3, N'0', 0)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (56, 2, 1, N'Phí bốc vác bởi tài xế cho phòng trọ có 3 đến 5 tầng', NULL, 120000, 1, N'DRIVER', N'FLOOR', 3, 5, N'0', 20)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (57, 2, 1, N'Phí bốc vác bởi tài xế cho phòng trọ có 5 tầng', NULL, 120000, 1, N'DRIVER', N'FLOOR', 5, NULL, N'0', 25)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (62, 12, NULL, N'Phí di chuyển dành cho xe tải 1000kg từ 15km đến 45km', NULL, 7020, 1, N'TRUCK', N'KM', 15, 45, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (63, 12, NULL, N'Phí di chuyển dành cho xe tải 1000kg trên 45km', NULL, 5400, 1, N'TRUCK', N'KM', 45, NULL, N'0', NULL)
GO
INSERT [dbo].[FeeSetting] ([Id], [ServiceId], [HouseTypeId], [Name], [Description], [Amount], [IsActived], [Type], [Unit], [RangeMin], [RangeMax], [DiscountRate], [FloorPercentage]) VALUES (64, NULL, NULL, N'Phí ngày lễ', N'Phí ngày lễ', 20, 1, N'HOLIDAY', N'PERCENT', NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[FeeSetting] OFF
GO
SET IDENTITY_INSERT [dbo].[Group] ON 
GO
INSERT [dbo].[Group] ([Id], [Name], [IsActived], [CreatedAt], [UpdatedAt], [DurationTimeActived]) VALUES (1, N'Tổ 1', 1, CAST(N'2024-11-13T19:38:44.017' AS DateTime), CAST(N'2024-12-04T20:58:03.143' AS DateTime), NULL)
GO
INSERT [dbo].[Group] ([Id], [Name], [IsActived], [CreatedAt], [UpdatedAt], [DurationTimeActived]) VALUES (2, N'Tổ 2', 1, CAST(N'2024-11-13T19:38:44.017' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Group] OFF
GO
SET IDENTITY_INSERT [dbo].[HolidaySetting] ON 
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (205, CAST(N'2024-01-01' AS Date), N'Tết Dương lịch', N'Ngày đầu năm dương lịch.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (206, CAST(N'2024-02-09' AS Date), N'Tết Nguyên Đán', N'Mùng 1 Tết Nguyên Đán, ngày đầu năm mới âm lịch.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (207, CAST(N'2024-02-10' AS Date), N'Tết Nguyên Đán', N'Mùng 2 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (208, CAST(N'2024-02-11' AS Date), N'Tết Nguyên Đán', N'Mùng 3 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (209, CAST(N'2024-02-12' AS Date), N'Tết Nguyên Đán', N'Mùng 4 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (210, CAST(N'2024-02-13' AS Date), N'Tết Nguyên Đán', N'Mùng 5 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (211, CAST(N'2024-02-14' AS Date), N'Tết Nguyên Đán', N'Mùng 6 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (212, CAST(N'2024-04-18' AS Date), N'Giỗ tổ Hùng Vương', N'Ngày tưởng niệm các vua Hùng.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (213, CAST(N'2024-04-30' AS Date), N'Ngày Thống nhất đất nước', N'Kỷ niệm ngày thống nhất Việt Nam.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (214, CAST(N'2024-05-01' AS Date), N'Ngày Quốc tế Lao động', N'Ngày quốc tế lao động.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (215, CAST(N'2024-09-02' AS Date), N'Ngày Quốc khánh Việt Nam', N'Ngày kỷ niệm thành lập nước Việt Nam.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (217, CAST(N'2025-01-01' AS Date), N'Tết Dương lịch', N'Ngày đầu năm dương lịch.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (218, CAST(N'2025-01-28' AS Date), N'Tết Nguyên Đán', N'Mùng 1 Tết Nguyên Đán, ngày đầu năm mới âm lịch.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (219, CAST(N'2025-01-29' AS Date), N'Tết Nguyên Đán', N'Mùng 2 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (220, CAST(N'2025-01-30' AS Date), N'Tết Nguyên Đán', N'Mùng 3 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (221, CAST(N'2025-01-31' AS Date), N'Tết Nguyên Đán', N'Mùng 4 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (222, CAST(N'2025-02-01' AS Date), N'Tết Nguyên Đán', N'Mùng 5 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (223, CAST(N'2025-02-02' AS Date), N'Tết Nguyên Đán', N'Mùng 6 Tết Nguyên Đán.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (224, CAST(N'2025-04-07' AS Date), N'Giỗ tổ Hùng Vương', N'Ngày tưởng niệm các vua Hùng.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (225, CAST(N'2025-04-30' AS Date), N'Ngày Thống nhất đất nước', N'Kỷ niệm ngày thống nhất Việt Nam.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (226, CAST(N'2025-05-01' AS Date), N'Ngày Quốc tế Lao động', N'Ngày quốc tế lao động.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (227, CAST(N'2025-09-01' AS Date), N'Ngày Quốc khánh Việt Nam', N'Ngày kỷ niệm thành lập nước Việt Nam.')
GO
INSERT [dbo].[HolidaySetting] ([Id], [Day], [Name], [Description]) VALUES (228, CAST(N'2025-09-02' AS Date), N'Ngày Quốc khánh Việt Nam', N'Ngày kỷ niệm thành lập nước Việt Nam.')
GO
SET IDENTITY_INSERT [dbo].[HolidaySetting] OFF
GO
SET IDENTITY_INSERT [dbo].[HouseType] ON 
GO
INSERT [dbo].[HouseType] ([Id], [Name], [Description], [IsActived]) VALUES (1, N'Phòng trọ', N'Phòng trọ ', 1)
GO
INSERT [dbo].[HouseType] ([Id], [Name], [Description], [IsActived]) VALUES (2, N'Nhà riêng / Biệt thự', N'Nhà riêng hoặc biệt thự', 1)
GO
INSERT [dbo].[HouseType] ([Id], [Name], [Description], [IsActived]) VALUES (3, N'Căn hộ / Chung cư', N'Căn hộ hoặc chung cư', 1)
GO
INSERT [dbo].[HouseType] ([Id], [Name], [Description], [IsActived]) VALUES (4, N'Công ty ', N'Công ty', 1)
GO
SET IDENTITY_INSERT [dbo].[HouseType] OFF
GO
SET IDENTITY_INSERT [dbo].[Notification] ON 
GO
INSERT [dbo].[Notification] ([Id], [UserId], [SentFrom], [Receive], [DeviceId], [Name], [Description], [Topic], [FcmToken], [IsRead], [BookingId]) VALUES (43, 3, NULL, NULL, NULL, NULL, NULL, NULL, N'd3NSF437QXWb4eLCRXtA-R:APA91bEQkqj61QrFP3Z08ZhasAup36EW0g8kSOLrBtFoDbWnupwNstE9NlEOJKYB7FgnXJTIy862ZEJqbbn60foyYtkuWsJA9pExKBOGsu4jOng_2VNS7vg', NULL, NULL)
GO
INSERT [dbo].[Notification] ([Id], [UserId], [SentFrom], [Receive], [DeviceId], [Name], [Description], [Topic], [FcmToken], [IsRead], [BookingId]) VALUES (44, 4, N'System', N'Phước Vinh', NULL, N'Driver Shortage Notification in bookingId: 3', N'Only 0 drivers available for 1 bookings in bookingId: 3.', N'DriverAssignment', NULL, 0, NULL)
GO
INSERT [dbo].[Notification] ([Id], [UserId], [SentFrom], [Receive], [DeviceId], [Name], [Description], [Topic], [FcmToken], [IsRead], [BookingId]) VALUES (45, 4, N'System', N'Phước Vinh', NULL, N'Driver Shortage Notification in bookingId: 8', N'Only 0 drivers available for 1 bookings in bookingId: 8.', N'DriverAssignment', NULL, 0, NULL)
GO
INSERT [dbo].[Notification] ([Id], [UserId], [SentFrom], [Receive], [DeviceId], [Name], [Description], [Topic], [FcmToken], [IsRead], [BookingId]) VALUES (46, 2, NULL, NULL, NULL, NULL, NULL, NULL, N'd3NSF437QXWb4eLCRXtA-R:APA91bEQkqj61QrFP3Z08ZhasAup36EW0g8kSOLrBtFoDbWnupwNstE9NlEOJKYB7FgnXJTIy862ZEJqbbn60foyYtkuWsJA9pExKBOGsu4jOng_2VNS7vg', NULL, NULL)
GO
INSERT [dbo].[Notification] ([Id], [UserId], [SentFrom], [Receive], [DeviceId], [Name], [Description], [Topic], [FcmToken], [IsRead], [BookingId]) VALUES (47, 4, N'System', N'Phước Vinh', NULL, N'Driver Shortage Notification in bookingId: 2', N'Only 0 drivers available for 4 bookings in bookingId: 2.', N'DriverAssignment', NULL, 0, NULL)
GO
INSERT [dbo].[Notification] ([Id], [UserId], [SentFrom], [Receive], [DeviceId], [Name], [Description], [Topic], [FcmToken], [IsRead], [BookingId]) VALUES (48, 4, N'System', N'Phước Vinh', NULL, N'Driver Shortage Notification in bookingId: 3', N'Only 0 drivers available for 4 bookings in bookingId: 3.', N'DriverAssignment', NULL, 0, NULL)
GO
INSERT [dbo].[Notification] ([Id], [UserId], [SentFrom], [Receive], [DeviceId], [Name], [Description], [Topic], [FcmToken], [IsRead], [BookingId]) VALUES (49, 4, N'System', N'Phước Vinh', NULL, N'Driver Shortage Notification in bookingId: 4', N'Only 0 drivers available for 4 bookings in bookingId: 4.', N'DriverAssignment', NULL, 0, NULL)
GO
INSERT [dbo].[Notification] ([Id], [UserId], [SentFrom], [Receive], [DeviceId], [Name], [Description], [Topic], [FcmToken], [IsRead], [BookingId]) VALUES (50, 4, N'System', N'Phước Vinh', NULL, N'Driver Shortage Notification in bookingId: 12', N'Only 0 drivers available for 1 bookings in bookingId: 12.', N'DriverAssignment', NULL, 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[Notification] OFF
GO
SET IDENTITY_INSERT [dbo].[PromotionCategory] ON 
GO
INSERT [dbo].[PromotionCategory] ([Id], [IsPublic], [StartDate], [EndDate], [DiscountRate], [DiscountMax], [RequireMin], [DiscountMin], [Name], [Description], [Type], [Quantity], [StartBookingTime], [EndBookingTime], [IsInfinite], [ServiceId], [IsDeleted], [Shard], [Amount]) VALUES (4, 1, CAST(N'2025-01-01T00:00:00.000' AS DateTime), CAST(N'2025-01-15T00:00:00.000' AS DateTime), 0.25, 75, 150, 25, N'End of Season Sale', N'End of season discounts', N'Clearance', 67, CAST(N'2024-11-10T00:00:00.000' AS DateTime), CAST(N'2024-12-15T00:00:00.000' AS DateTime), 1, 3, 0, NULL, NULL)
GO
INSERT [dbo].[PromotionCategory] ([Id], [IsPublic], [StartDate], [EndDate], [DiscountRate], [DiscountMax], [RequireMin], [DiscountMin], [Name], [Description], [Type], [Quantity], [StartBookingTime], [EndBookingTime], [IsInfinite], [ServiceId], [IsDeleted], [Shard], [Amount]) VALUES (5, 1, CAST(N'2025-01-20T00:00:00.000' AS DateTime), CAST(N'2025-01-25T00:00:00.000' AS DateTime), 0.05, 20, 25, 5, N'Loyalty Reward', N'Reward for loyal customers', N'Loyalty', 291, CAST(N'2024-10-01T00:00:00.000' AS DateTime), CAST(N'2024-11-25T00:00:00.000' AS DateTime), 0, 4, 0, NULL, NULL)
GO
INSERT [dbo].[PromotionCategory] ([Id], [IsPublic], [StartDate], [EndDate], [DiscountRate], [DiscountMax], [RequireMin], [DiscountMin], [Name], [Description], [Type], [Quantity], [StartBookingTime], [EndBookingTime], [IsInfinite], [ServiceId], [IsDeleted], [Shard], [Amount]) VALUES (22, 1, CAST(N'2024-11-29T00:00:00.000' AS DateTime), CAST(N'2024-12-30T23:59:59.000' AS DateTime), 10, 50, 100, 10, N'Winter Sale', N'Special winter discount', N'Seasonal', 3, CAST(N'2024-10-29T00:00:00.000' AS DateTime), CAST(N'2024-10-29T23:59:59.000' AS DateTime), 0, 8, 1, NULL, NULL)
GO
INSERT [dbo].[PromotionCategory] ([Id], [IsPublic], [StartDate], [EndDate], [DiscountRate], [DiscountMax], [RequireMin], [DiscountMin], [Name], [Description], [Type], [Quantity], [StartBookingTime], [EndBookingTime], [IsInfinite], [ServiceId], [IsDeleted], [Shard], [Amount]) VALUES (23, 1, CAST(N'2024-11-29T00:00:00.000' AS DateTime), CAST(N'2024-12-30T23:59:59.000' AS DateTime), 10, 50, 100, 10, N'Winter Sale', N'Special winter discount', N'Seasonal', 4, CAST(N'2024-10-29T01:00:00.000' AS DateTime), CAST(N'2024-10-29T23:59:59.000' AS DateTime), 0, 2, 0, NULL, NULL)
GO
INSERT [dbo].[PromotionCategory] ([Id], [IsPublic], [StartDate], [EndDate], [DiscountRate], [DiscountMax], [RequireMin], [DiscountMin], [Name], [Description], [Type], [Quantity], [StartBookingTime], [EndBookingTime], [IsInfinite], [ServiceId], [IsDeleted], [Shard], [Amount]) VALUES (24, 1, CAST(N'2024-10-29T00:00:00.000' AS DateTime), CAST(N'2024-11-30T23:59:59.000' AS DateTime), 10, 50, 100, 10, N'Winter Sale', N'Special winter discount', N'Seasonal', 10, CAST(N'2024-10-29T00:00:00.000' AS DateTime), CAST(N'2024-11-30T23:59:59.000' AS DateTime), 0, 12, 0, NULL, NULL)
GO
INSERT [dbo].[PromotionCategory] ([Id], [IsPublic], [StartDate], [EndDate], [DiscountRate], [DiscountMax], [RequireMin], [DiscountMin], [Name], [Description], [Type], [Quantity], [StartBookingTime], [EndBookingTime], [IsInfinite], [ServiceId], [IsDeleted], [Shard], [Amount]) VALUES (32, 1, CAST(N'2024-10-29T00:00:00.000' AS DateTime), CAST(N'2024-11-30T23:59:59.000' AS DateTime), 10, 50, 100, 10, N'Winter Sale', N'Special winter discount', N'Seasonal', 5, CAST(N'2024-10-29T00:00:00.000' AS DateTime), CAST(N'2024-11-30T23:59:59.000' AS DateTime), 0, 1, 0, NULL, NULL)
GO
INSERT [dbo].[PromotionCategory] ([Id], [IsPublic], [StartDate], [EndDate], [DiscountRate], [DiscountMax], [RequireMin], [DiscountMin], [Name], [Description], [Type], [Quantity], [StartBookingTime], [EndBookingTime], [IsInfinite], [ServiceId], [IsDeleted], [Shard], [Amount]) VALUES (33, NULL, CAST(N'2024-12-14T17:00:00.000' AS DateTime), CAST(N'2024-12-20T17:00:00.000' AS DateTime), NULL, NULL, NULL, NULL, N'test', N'aaaaaaaaaaaaaaaa', N'', 1, NULL, NULL, NULL, 9, 0, NULL, NULL)
GO
INSERT [dbo].[PromotionCategory] ([Id], [IsPublic], [StartDate], [EndDate], [DiscountRate], [DiscountMax], [RequireMin], [DiscountMin], [Name], [Description], [Type], [Quantity], [StartBookingTime], [EndBookingTime], [IsInfinite], [ServiceId], [IsDeleted], [Shard], [Amount]) VALUES (34, 1, CAST(N'2024-10-29T00:00:00.000' AS DateTime), CAST(N'2024-11-30T23:59:59.000' AS DateTime), 10, 50, 100, 10, N'Winter Sale', N'Special winter discount', N'Seasonal', 100, CAST(N'2024-10-29T00:00:00.000' AS DateTime), CAST(N'2024-11-30T23:59:59.000' AS DateTime), 0, 2, 0, NULL, 100)
GO
SET IDENTITY_INSERT [dbo].[PromotionCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (1, N'Admin')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (2, N'Reviewer')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (3, N'Customer')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (4, N'Driver')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (5, N'Porter')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (6, N'Manager')
GO
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
INSERT [dbo].[Schedule] ([Id], [Date], [IsDeleted]) VALUES (1, CAST(N'2024-12-05' AS Date), 0)
GO
INSERT [dbo].[Schedule] ([Id], [Date], [IsDeleted]) VALUES (2, CAST(N'2024-12-06' AS Date), 0)
GO
SET IDENTITY_INSERT [dbo].[ScheduleBooking] ON 
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (1, 1, N'20241101')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (2, 1, N'20241031')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (3, 1, N'20241102')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (4, 1, N'20241130')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (5, 1, N'20241103')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (6, 1, N'20241104')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (7, 1, N'20241105')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (9, 1, N'20241115')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (11, 1, N'20241116')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (12, 1, N'20241107')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (13, 1, N'20241106')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (14, 1, N'20241106')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (15, 1, N'20241109')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (16, 1, N'20241108')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (17, 1, N'20241110')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (18, 1, N'20241111')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (19, 1, N'20241128')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (20, 1, N'20241112')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (21, 1, N'20241113')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (22, 1, N'20241113')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (23, 1, N'20241114')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (24, 1, N'20241121')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (25, 1, N'20241230')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (26, 1, N'20241117')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (27, 1, N'20241118')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (28, 1, N'20241119')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (29, 1, N'20241120')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (30, 1, N'20241122')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (31, 1, N'20241123')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (32, 1, N'20241124')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (33, 1, N'20241125')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (34, 1, N'20241126')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (35, 1, N'20241127')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (36, 1, N'20241129')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (37, 1, N'20241201')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (38, 1, N'20241202')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (39, 1, N'20241203')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (40, 1, N'20241204')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (41, 1, N'20241205')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (42, 1, N'20241206')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (43, 1, N'20241207')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (44, 1, N'20241208')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (45, 1, N'20241209')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (46, 1, N'20241210')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (47, 1, N'20241211')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (48, 1, N'20241212')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (49, 1, N'20241213')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (50, 1, N'20241214')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (51, 1, N'20241216')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (52, 1, N'20241217')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (53, 1, N'20241218')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (54, 1, N'20241219')
GO
INSERT [dbo].[ScheduleBooking] ([Id], [IsActived], [Shard]) VALUES (55, 1, N'20241220')
GO
SET IDENTITY_INSERT [dbo].[ScheduleBooking] OFF
GO
SET IDENTITY_INSERT [dbo].[ScheduleWorking] ON 
GO
INSERT [dbo].[ScheduleWorking] ([Id], [Name], [IsActived], [CreatedAt], [UpdatedAt], [DurationTimeActived], [Type], [StartDate], [EndDate], [GroupId], [ExtentStartDate], [ExtentEndDate], [ScheduleId]) VALUES (1, N'Ca 1', 1, CAST(N'2024-11-14T04:18:34.607' AS DateTime), CAST(N'2024-11-14T04:18:34.607' AS DateTime), 3, N'SHIFT', CAST(N'04:00:00' AS Time), CAST(N'07:00:00' AS Time), 2, NULL, NULL, 1)
GO
INSERT [dbo].[ScheduleWorking] ([Id], [Name], [IsActived], [CreatedAt], [UpdatedAt], [DurationTimeActived], [Type], [StartDate], [EndDate], [GroupId], [ExtentStartDate], [ExtentEndDate], [ScheduleId]) VALUES (2, N'Ca 2', 1, CAST(N'2024-11-14T04:18:34.607' AS DateTime), CAST(N'2024-11-14T04:18:34.607' AS DateTime), 5, N'SHIFT', CAST(N'07:00:00' AS Time), CAST(N'12:00:00' AS Time), 1, NULL, NULL, 1)
GO
INSERT [dbo].[ScheduleWorking] ([Id], [Name], [IsActived], [CreatedAt], [UpdatedAt], [DurationTimeActived], [Type], [StartDate], [EndDate], [GroupId], [ExtentStartDate], [ExtentEndDate], [ScheduleId]) VALUES (3, N'Ca 3', 1, CAST(N'2024-11-14T04:18:34.607' AS DateTime), CAST(N'2024-11-14T04:18:34.607' AS DateTime), 5, N'SHIFT', CAST(N'12:00:00' AS Time), CAST(N'17:00:00' AS Time), 1, NULL, NULL, NULL)
GO
INSERT [dbo].[ScheduleWorking] ([Id], [Name], [IsActived], [CreatedAt], [UpdatedAt], [DurationTimeActived], [Type], [StartDate], [EndDate], [GroupId], [ExtentStartDate], [ExtentEndDate], [ScheduleId]) VALUES (4, N'Ca 4', 1, CAST(N'2024-11-14T04:18:34.607' AS DateTime), CAST(N'2024-11-14T04:18:34.607' AS DateTime), 5, N'SHIFT', CAST(N'17:00:00' AS Time), CAST(N'22:00:00' AS Time), 1, NULL, NULL, NULL)
GO
INSERT [dbo].[ScheduleWorking] ([Id], [Name], [IsActived], [CreatedAt], [UpdatedAt], [DurationTimeActived], [Type], [StartDate], [EndDate], [GroupId], [ExtentStartDate], [ExtentEndDate], [ScheduleId]) VALUES (5, N'Ca 5', 1, CAST(N'2024-11-14T04:18:34.607' AS DateTime), CAST(N'2024-11-14T04:18:34.607' AS DateTime), 6, N'SHIFT', CAST(N'22:00:00' AS Time), CAST(N'04:00:00' AS Time), 1, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[ScheduleWorking] OFF
GO
SET IDENTITY_INSERT [dbo].[Service] ON 
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (1, N'Dịch vụ bốc xếp', N'Dịch vụ bốc xếp', 1, 0, N'https://utfs.io/f/yYv3QHy7AjsNjsu7CV283C62KdHlsVPyZNpfiOM4Wu9RAeUo', 0, 0, NULL, N'PORTER', 0, NULL, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (2, N'Bốc xếp (Bởi tài xế)', N'120.000đ/người/xe (3 tầng), tăng theo tầng (20% cho 4-5 tầng, 25% cho >5 tầng), không bao gồm bốc xếp hàng hóa chuyên nghiệp.', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNiG3b4dl0e4Z2KXx1WvABI9Tj38wqRoQlsPLO', 0, 0, 1, N'PORTER', 1, 10, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (3, N'Bốc xếp (Bởi nhân viên bốc xếp)', N'400.000đ/người (3 tầng), tăng theo tầng (20% cho 4-5 tầng, 25% cho >5 tầng), có nhân viên bốc xếp chuyên nghiệp.', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 0, 1, N'PORTER', 1, 20, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (4, N'Bốc xếp sử dụng thang máy nhỏ hoặc không có thang máy', N'1 tầng = 100.000đ', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 100000, 1, N'PORTER', 1, 10, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (5, N'Bốc xếp tại địa điểm khó tiếp cận', N'Giá phụ thu theo khoảng cách: 10-500m
Bao gồm: Phí này áp dụng cho những địa điểm mà xe tải không thể đến gần, cần di chuyển hàng hóa bằng tay hoặc xe đẩy qua những quãng đường dài.', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 50000, 1, N'PORTER', 1, 50, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (8, N'Dịch vụ tháo lắp', N'Dịch vụ tháo lắp', 1, 0, N'https://utfs.io/f/yYv3QHy7AjsN5Vq4xMDaRS8WIfhB20u5l7nJweOHDEMZCUYL', 0, 0, NULL, N'DISASSEMBLE', 0, NULL, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (9, N'Tháo lắp, đóng gói máy lạnh', N'Phí: 300.000đ/bộ. Bao gồm: Dịch vụ tháo lắp và đóng gói điều hòa máy lạnh chuyên nghiệp, đảm bảo an toàn trong suốt quá trình di chuyển.', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsN4MLraTd32PUFqu15wSWQjXv7BZs0aOcAnVLg', 0, 300000, 8, N'DISASSEMBLE', 1, 30, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (10, N'Tháo lắp, đóng gói cho các đồ vật nhỏ (< hoặc = 10kg)', N'Phí: 150.000đ/bộ cho các đồ vật nhỏ <=5kg, 300.000đ/bộ cho các đồ vật lớn >6kg.                                        
Bao gồm: Dịch vụ tháo lắp và đóng gói các đồ vật khác như tủ, kệ, đồ điện tử, đảm bảo an toàn khi vận chuyển.', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 150000, 8, N'DISASSEMBLE', 1, 30, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (11, N'Dịch vụ thuê xe', N'Dịch vụ thuê xe tải', 1, 0, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 10, 500, NULL, N'TRUCK', 0, NULL, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (12, N'Xe tải 1000kg', N'Cấm tải 6-9h & 16-20h', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 600, 11, N'TRUCK', 1, 50, 4)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (13, N'Xe tải 2000kg', N'Cấm tải 6-9h & 16-20h', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 700, 11, N'TRUCK', 1, 30, 5)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (14, N'Xe tải 2500kg', N'Cấm tải 6-9h & 16-20h', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 800, 11, N'TRUCK', 1, 20, 6)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (15, N'Xe tải 500kg', N'Cấm tải 6-9h & 16-20h', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 900, 11, N'TRUCK', 1, 100, 3)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (16, N'Xe van 500kg', N'Hoạt động tất cả khung giờ', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 1000, 11, N'TRUCK', 1, 75, 1)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (17, N'Xe van 1000kg', N'Hoạt động tất cả khung giờ', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 1100, 11, N'TRUCK', 1, NULL, 2)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (18, N'Phí chờ (60.000đ/giờ)', N'Phí chờ 1h', 1, 0, N'https://utfs.io/f/yYv3QHy7AjsNQKNwVjnmasOWeLbNifvoYtlFB7D40RAygQUh', 0, 60000, NULL, N'SYSTEM', 1, 5, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (19, N'Hỗ trợ tài xế', N'Hỗ trợ tài xế', 1, 0, N'https://utfs.io/f/yYv3QHy7AjsNldFfM0yWxAjNQpIeCGHnFMbcfuLP32V4J9kZ', 0, 10000, NULL, N'SYSTEM', 1, 10, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (20, N'Chứng từ điện tử ', N'Chứng từ điện tử', 1, 0, N'https://utfs.io/f/yYv3QHy7AjsNEyyPoCHQvlfArcBaZO3pKTuSqP59XMYw6beJ', 0, 5000, NULL, N'SYSTEM', 0, 1, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (27, N'Tháo lắp, đóng gói cho các đồ vật lớn (>10kg) ', N'Phí: 300.000đ/bộ cho các đồ vật nhỏ <=5kg, 300.000đ/bộ cho các đồ vật lớn >10kg.                                        
Bao gồm: Dịch vụ tháo lắp và đóng gói các đồ vật khác như tủ, kệ, đồ điện tử, đảm bảo an toàn khi vận chuyển.', 1, 1, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 300000, 8, N'DISASSEMBLE', 1, 30, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (32, N'Hỗ trợ nhân viên bốc xếp', N'Hỗ trợ nhân viên bốc xếp', 1, 0, N'https://utfs.io/f/yYv3QHy7AjsN5ZFQCsDaRS8WIfhB20u5l7nJweOHDEMZCUYL', 0, 10000, NULL, N'SYSTEM', 1, 10, NULL)
GO
INSERT [dbo].[Service] ([Id], [Name], [Description], [IsActived], [Tier], [ImageUrl], [DiscountRate], [Amount], [ParentServiceId], [Type], [IsQuantity], [QuantityMax], [TruckCategoryId]) VALUES (34, N'Vận chuyển đồ vật giá trị cao (>50 triệu)', N'Bảo vệ đồ vật giá trị của bạn trước những thiệt hại do sự cố, mất cắp/hư hỏnga', 1, 0, N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 0, 100000, NULL, N'INSURANCE', 1, 100, NULL)
GO
SET IDENTITY_INSERT [dbo].[Service] OFF
GO
SET IDENTITY_INSERT [dbo].[Truck] ON 
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (1, 1, N'string', N'string', 0, 0, N'string', N'string', 0, 3, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (3, 3, N'string', N'string', 0, 1, N'string', N'string', 1, 2, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (4, 1, N'string', N'string', 0, 1, N'string', N'string', 1, 4, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (5, 3, N'Honda', N'23A-123 56', 3, 1, N'Honda', N'Yellow', 1, 9, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (6, 1, N'string', N'string', 0, 1, N'string', N'string', 1, 10, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (7, 1, N'string', N'string', 0, 1, N'string', N'string', 1, 11, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (38, 1, N'Xe van 500kg Model 1', N'XX5001', 500, 1, N'Brand 1', N'White', 1, 16, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (40, 1, N'Xe van 500kg Model 2', N'XX5002', 500, 1, N'Brand 2', N'Black', 1, 17, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (41, 1, N'Xe van 500kg Model 3', N'XX5003', 500, 1, N'Brand 3', N'Blue', 1, 18, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (42, 1, N'Xe van 500kg Model 4', N'XX5004', 500, 1, N'Brand 4', N'Red', 1, 19, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (43, 1, N'Xe van 500kg Model 5', N'XX5005', 500, 1, N'Brand 5', N'Green', 1, 20, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (44, 1, N'Xe van 500kg Model 6', N'XX5006', 500, 1, N'Brand 6', N'Yellow', 1, 21, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (45, 1, N'Xe van 500kg Model 7', N'XX5007', 500, 1, N'Brand 7', N'Gray', 1, 22, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (46, 1, N'Xe van 500kg Model 8', N'XX5008', 500, 1, N'Brand 8', N'Purple', 1, 23, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (47, 5, N'Xe tải 2000kg Model 11', N'XX5009', 500, 1, N'Brand 9', N'Orange', 1, 24, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (48, 5, N'Xe tải 2000kg Model 12', N'XX5010', 500, 1, N'Brand 10', N'Pink', 1, 25, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (49, 5, N'Xe tải 2000kg Model 13', N'XX5011', 500, 1, N'Brand 11', N'Brown', 1, 26, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (50, 5, N'Xe tải 2000kg Model 14', N'XX5012', 500, 1, N'Brand 12', N'Gray', 1, 27, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (51, 5, N'Xe tải 2000kg Model 15', N'XX5013', 500, 1, N'Brand 13', N'Black', 1, 28, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (52, 5, N'Xe tải 2000kg Model 16', N'XX5014', 500, 1, N'Brand 14', N'Blue', 1, 29, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (53, 5, N'Xe tải 2000kg Model 17', N'XX5015', 500, 1, N'Brand 15', N'Red', 1, 30, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (55, 2, N'Xe van 1000kg Model 1', N'XX10001', 1000, 1, N'Brand 1', N'White', 1, 31, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (56, 2, N'Xe van 1000kg Model 2', N'XX10002', 1000, 1, N'Brand 2', N'Black', 1, 32, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (57, 2, N'Xe van 1000kg Model 3', N'XX10003', 1000, 1, N'Brand 3', N'Blue', 1, 33, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (58, 2, N'Xe van 1000kg Model 4', N'XX10004', 1000, 1, N'Brand 4', N'Red', 1, 34, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (59, 2, N'Xe van 1000kg Model 5', N'XX10005', 1000, 1, N'Brand 5', N'Green', 1, 35, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (60, 2, N'Xe van 1000kg Model 6', N'XX10006', 1000, 1, N'Brand 6', N'Yellow', 1, 36, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (61, 2, N'Xe van 1000kg Model 7', N'XX10007', 1000, 1, N'Brand 7', N'Gray', 1, 37, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (62, 2, N'Xe van 1000kg Model 8', N'XX10008', 1000, 1, N'Brand 8', N'Purple', 1, 38, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (63, 5, N'Xe tải 2000kg Model 27', N'XX10009', 1000, 1, N'Brand 9', N'Orange', 1, 39, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (64, 2, N'Xe van 1000kg Model 10', N'XX10010', 1000, 1, N'Brand 10', N'Pink', 1, 40, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (65, 2, N'Xe van 1000kg Model 11', N'XX10011', 1000, 1, N'Brand 11', N'Brown', 1, 41, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (66, 2, N'Xe van 1000kg Model 12', N'XX10012', 1000, 1, N'Brand 12', N'Gray', 1, 42, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (67, 2, N'Xe van 1000kg Model 13', N'XX10013', 1000, 1, N'Brand 13', N'Black', 1, 43, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (68, 2, N'Xe van 1000kg Model 14', N'XX10014', 1000, 1, N'Brand 14', N'Blue', 1, 44, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (69, 2, N'Xe van 1000kg Model 15', N'XX10015', 1000, 1, N'Brand 15', N'Red', 1, 45, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (70, 3, N'Xe tải 500kg Model 1', N'XX5001', 500, 1, N'Brand 1', N'White', 1, 46, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (71, 3, N'Xe tải 500kg Model 2', N'XX5002', 500, 1, N'Brand 2', N'Black', 1, 47, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (72, 3, N'Xe tải 500kg Model 3', N'XX5003', 500, 1, N'Brand 3', N'Blue', 1, 48, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (73, 3, N'Xe tải 500kg Model 4', N'XX5004', 500, 1, N'Brand 4', N'Red', 1, 49, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (74, 3, N'Xe tải 500kg Model 5', N'XX5005', 500, 1, N'Brand 5', N'Green', 1, 50, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (75, 3, N'Xe tải 500kg Model 6', N'XX5006', 500, 1, N'Brand 6', N'Yellow', 1, 51, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (76, 3, N'Xe tải 500kg Model 7', N'XX5007', 500, 1, N'Brand 7', N'Gray', 1, 52, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (77, 3, N'Xe tải 500kg Model 8', N'XX5008', 500, 1, N'Brand 8', N'Purple', 1, 53, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (78, 3, N'Xe tải 500kg Model 9', N'XX5009', 500, 1, N'Brand 9', N'Orange', 1, 54, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (79, 3, N'Xe tải 500kg Model 10', N'XX5010', 500, 1, N'Brand 10', N'Pink', 1, 55, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (80, 3, N'Xe tải 500kg Model 11', N'XX5011', 500, 1, N'Brand 11', N'Brown', 1, 56, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (81, 3, N'Xe tải 500kg Model 12', N'XX5012', 500, 1, N'Brand 12', N'Gray', 1, 57, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (82, 3, N'Xe tải 500kg Model 13', N'XX5013', 500, 1, N'Brand 13', N'Black', 1, 58, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (83, 3, N'Xe tải 500kg Model 14', N'XX5014', 500, 1, N'Brand 14', N'Blue', 1, 59, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (84, 3, N'Xe tải 500kg Model 15', N'XX5015', 500, 1, N'Brand 15', N'Red', 1, 60, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (85, 4, N'Xe tải 1000kg Model 1', N'XX10001', 1000, 1, N'Brand 1', N'White', 1, 61, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (86, 4, N'Xe tải 1000kg Model 2', N'XX10002', 1000, 1, N'Brand 2', N'Black', 1, 62, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (87, 3, N'Xe tải 1000kg Model 3', N'XX10003', 1000, 1, N'Brand 3', N'Blue', 1, 63, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (88, 3, N'Xe tải 1000kg Model 4', N'XX10004', 1000, 1, N'Brand 4', N'Red', 1, 64, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (89, 3, N'Xe tải 1000kg Model 5', N'XX10005', 1000, 1, N'Brand 5', N'Green', 1, 65, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (90, 3, N'Xe tải 1000kg Model 6', N'XX10006', 1000, 1, N'Brand 6', N'Yellow', 1, 66, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (91, 3, N'Xe tải 1000kg Model 7', N'XX10007', 1000, 1, N'Brand 7', N'Gray', 1, 67, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (92, 3, N'Xe tải 1000kg Model 8', N'XX10008', 1000, 1, N'Brand 8', N'Purple', 1, 68, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (93, 3, N'Xe tải 1000kg Model 9', N'XX10009', 1000, 1, N'Brand 9', N'Orange', 1, 69, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (94, 3, N'Xe tải 1000kg Model 10', N'XX10010', 1000, 1, N'Brand 10', N'Pink', 1, 70, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (125, 5, N'Xe tải 1500kg Model 1', N'XX15001', 1500, 1, N'Brand 1', N'White', 1, 76, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (126, 5, N'Xe tải 2000kg Model 18', N'XX15002', 1500, 1, N'Brand 2', N'Black', 1, 77, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (127, 5, N'Xe tải 2000kg Model 19', N'XX15003', 1500, 1, N'Brand 3', N'Blue', 1, 78, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (128, 5, N'Xe tải 2000kg Model 20', N'XX15004', 1500, 1, N'Brand 4', N'Red', 1, 79, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (129, 5, N'Xe tải 2000kg Model 21', N'XX15005', 1500, 1, N'Brand 5', N'Green', 1, 80, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (130, 5, N'Xe tải 2000kg Model 22', N'XX15006', 1500, 1, N'Brand 6', N'Yellow', 1, 81, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (131, 5, N'Xe tải 2000kg Model 23', N'XX15007', 1500, 1, N'Brand 7', N'Gray', 1, 82, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (132, 5, N'Xe tải 2000kg Model 24', N'XX15008', 1500, 1, N'Brand 8', N'Purple', 1, 83, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (133, 5, N'Xe tải 2000kg Model 25', N'XX15009', 1500, 1, N'Brand 9', N'Orange', 1, 84, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (134, 6, N'Xe tải 2500kg Model 1', N'XX20001', 2000, 1, N'Brand 1', N'White', 1, 71, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (135, 6, N'Xe tải 2500kg Model 2', N'XX20002', 2000, 1, N'Brand 2', N'Black', 1, 72, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (136, 6, N'Xe tải 2500kg Model 3', N'XX20003', 2000, 1, N'Brand 3', N'Blue', 1, 73, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (137, 6, N'Xe tải 2500kg Model 4', N'XX20004', 2000, 1, N'Brand 4', N'Red', 1, 74, 0)
GO
INSERT [dbo].[Truck] ([Id], [TruckCategoryId], [Model], [NumberPlate], [Capacity], [IsAvailable], [Brand], [Color], [IsInsurrance], [UserId], [IsDeleted]) VALUES (138, 6, N'Xe tải 2500kg Model 5', N'XX20005', 2000, 1, N'Brand 5', N'Green', 1, 75, 0)
GO
SET IDENTITY_INSERT [dbo].[Truck] OFF
GO
SET IDENTITY_INSERT [dbo].[TruckCategory] ON 
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (1, N'Xe van 500kg', 500, N'Xe van 500kg là dòng xe tải nhỏ gọn, dễ di chuyển trong đô thị, tiết kiệm nhiên liệu, phù hợp vận chuyển hàng hóa nhẹ. Khoang chứa hàng rộng, chi phí thấp, phù hợp cho doanh nghiệp nhỏ.', N'Xe van 500kg là dòng xe tải nhỏ gọn, dễ di chuyển trong đô thị, tiết kiệm nhiên liệu, phù hợp vận chuyển hàng hóa nhẹ. Khoang chứa hàng rộng, chi phí thấp, phù hợp cho doanh nghiệp nhỏ.', N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 111780, NULL, N'1.7m', N'1.2m', N'1.2m', 0)
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (2, N'Xe van 1000kg', 850, N'Xe van 1000kg là xe tải cỡ trung, chở hàng tối đa 1000kg, thiết kế kín bảo vệ hàng hóa, phù hợp cho vận chuyển thương mại và dễ di chuyển trong đô thị.', N'
Xe van 1000kg là loại xe có sức chở tối đa 1000kg, thiết kế kín đáo, bảo vệ hàng hóa tốt, linh hoạt di chuyển trong thành phố, phù hợp cho vận chuyển hàng hóa vừa và nhỏ.', N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 148500, NULL, N'2.3m', N'1.5m', N'1.4m', 0)
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (3, N'Xe tải 500kg', 500, N'Xe tải 500kg là dòng xe nhỏ gọn, chở hàng tối đa 500kg, tiết kiệm nhiên liệu, di chuyển linh hoạt trong đô thị, phù hợp cho cá nhân và doanh nghiệp nhỏ.', N'Xe tải 500kg là xe tải nhẹ, chở hàng tối đa 500kg, linh hoạt, tiết kiệm nhiên liệu, phù hợp cho vận chuyển hàng hóa nhỏ trong đô thị và doanh nghiệp nhỏ.', N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 111780, NULL, N'1.8m', N'1.3m', N'1.3m', 0)
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (4, N'Xe tải 1000kg', 1000, N'Xe tải 1000 kg nhỏ gọn, mạnh mẽ, chở hàng tối đa 1 tấn, phù hợp di chuyển trong đô thị, tiết kiệm nhiên liệu.', N'
Xe tải 1000 kg nhỏ gọn, chở tối đa 1 tấn, mạnh mẽ và tiết kiệm nhiên liệu, phù hợp cho vận chuyển hàng hóa vừa trong đô thị và tuyến đường ngắn.', N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 148500, NULL, N'3.0m', N'1.6m', N'1.7m', 0)
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (5, N'Xe tải 2000kg', 2000, N'Xe tải 2000 kg chở tối đa 2 tấn, bền bỉ, rộng rãi, phù hợp vận chuyển hàng lớn trong đô thị và đường dài.', N'Xe tải 2000 kg mạnh mẽ, chở tối đa 2 tấn, bền bỉ và rộng rãi, thích hợp cho vận chuyển hàng hóa lớn trong nhiều điều kiện đường xá.', N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 260280, NULL, N'4.2m', N'1.75m', N'1.8m', 0)
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (6, N'Xe tải 2500kg', 2500, N'Xe tải 2500 kg chở tối đa 2,5 tấn, thiết kế chắc chắn và thùng rộng, phù hợp vận chuyển hàng lớn. Động cơ mạnh mẽ, tiết kiệm nhiên liệu, lý tưởng cho doanh nghiệp cần giao hàng thường xuyên.', N'Xe tải 2500 kg chở tối đa 2,5 tấn, thiết kế chắc chắn, tiết kiệm nhiên liệu, phù hợp cho vận chuyển hàng lớn trong đô thị và trên đường dài.', N'https://utfs.io/f/yYv3QHy7AjsNa1n5qlcN0WIECY7qV85KdQoXSGufv9LwUlHF', 355860, NULL, N'4.3m', N'1.8m', N'2.0m', 0)
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (9, N'Heavy Duty Truck', 12000, N'A truck suitable for heavy loads.', N'Designed for heavy-duty transport.', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1728489912/movemate/vs174go4uz7uw1g9js2e.jpg', 15000, 100, N'12m', N'2.5m', N'3.0m', 1)
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (10, N'Heavy Duty Truck', 12000, N'A truck suitable for heavy loads.', N'Designed for heavy-duty transport.', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1728489912/movemate/vs174go4uz7uw1g9js2e.jpg', 15000, 100, N'7.5m', N'2.5m', N'3.0m', 1)
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (11, N'Heavy Duty Truck', 12000, N'A truck suitable for heavy loads.', N'Designed for heavy-duty transport.', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1728489912/movemate/vs174go4uz7uw1g9js2e.jpg', 15000, 100, N'7.5m', N'2.5m', N'3.0m', 1)
GO
INSERT [dbo].[TruckCategory] ([Id], [CategoryName], [MaxLoad], [Description], [Summarize], [ImageUrl], [Price], [TotalTrips], [EstimatedLenght], [EstimatedWidth], [EstimatedHeight], [IsDeleted]) VALUES (12, N'test', 1, N'test', NULL, N'https://utfs.io/f/yYv3QHy7AjsNAuNjOwff7kSYTJB8IjMR4uWqmgVs0i5rCHlP', NULL, NULL, N'4.4m', N'2.5m', N'3.4m', 1)
GO
SET IDENTITY_INSERT [dbo].[TruckCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[TruckImg] ON 
GO
INSERT [dbo].[TruckImg] ([Id], [TruckId], [ImageUrl], [ImageCode], [IsDeleted]) VALUES (3, 38, N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1731334497/xe-van-1000kg_cnl8px.png', N'IMG001', 0)
GO
INSERT [dbo].[TruckImg] ([Id], [TruckId], [ImageUrl], [ImageCode], [IsDeleted]) VALUES (4, 40, N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1731334494/xe-tai-500kg_hesizk.jpg', N'IMG002', 0)
GO
INSERT [dbo].[TruckImg] ([Id], [TruckId], [ImageUrl], [ImageCode], [IsDeleted]) VALUES (5, 41, N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1731334491/xe-tai-1000_nbdehb.jpg', N'IMG003', 0)
GO
INSERT [dbo].[TruckImg] ([Id], [TruckId], [ImageUrl], [ImageCode], [IsDeleted]) VALUES (6, 42, N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1731334491/xe_tai_2000kg_rubaj0.jpg', N'IMG004', 1)
GO
INSERT [dbo].[TruckImg] ([Id], [TruckId], [ImageUrl], [ImageCode], [IsDeleted]) VALUES (7, 43, N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1731334492/xe_tai_2500kg_fpisan.jpg', N'IMG005', 0)
GO
SET IDENTITY_INSERT [dbo].[TruckImg] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (1, 1, N'Admin', N'0123456789', N'hashedPassword123', N'Male', N'johndoe@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (2, 2, N'Phương Phương', N'0123356789', N'@Thanhvinh2002', N'Male', N'a@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, 1, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (3, 3, N'Thành Vinh', N'+84382703626', N'@Thanhvinh2002', N'Male', N'vinh2002@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (4, 6, N'Phước Vinh', N'0123444444', N'@Thanhvinh2002', N'Male', N'manager@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (5, 3, N'Nguyễn Vinh', NULL, N'@Thanhvinh2002', N'Male', N'vinhntse161950@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (6, 3, N'Hinh Bi', N'0913932923', N'1', N'Male', N'bibi@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (8, 2, N'Hoàng Quốc Trung', N'0676084871', N'@Thanhvinh2002', N'Male', N'han@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (9, 4, N'Phan Anh Nhung', N'0860498354', N'@Thanhvinh2002', N'Male', N'driver1@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (10, 4, N'Vũ Văn Anh', N'0622031572', N'@Thanhvinh2002', N'Male', N'driver2@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (11, 4, N'Hồ Thị Lan', N'0571073487', N'@Thanhvinh2002', N'Male', N'driver3@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 1, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (12, 3, N'Đặng Đức Tâm', N'0867428387', N'string', NULL, N'user@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (13, 3, N'Ngô Hồng Hạnh', N'0948150124', N'@Thanhvinh2002', NULL, N'cophuocvinh@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (14, 3, N'Đặng Hồng Anh', N'0970512040', N'string', NULL, N'us11er@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20240809')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (16, 4, N'Võ Anh Nam', N'0565336231', N'@Thanhvinh2002', N'Male', N'user1@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1990-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code1', N'Number1', 1, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (17, 4, N'Trần Quốc Lan', N'0646697794', N'@Thanhvinh2002', N'Female', N'user2@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1991-02-02T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code2', N'Number2', 1, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (18, 4, N'Trần Minh Quyên', N'0668047185', N'@Thanhvinh2002', N'Male', N'lehananh06@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1992-03-03T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code3', N'Number3', 1, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (19, 4, N'Nguyễn Anh Hạnh', N'0445722706', N'@Thanhvinh2002', N'Female', N'user4@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1993-04-04T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code4', N'Number4', 1, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (20, 4, N'Bùi Đức Trung', N'0832548287', N'@Thanhvinh2002', N'Male', N'user5@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1994-05-05T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code5', N'Number5', 2, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (21, 4, N'Trần Văn Anh', N'0391555900', N'@Thanhvinh2002', N'Female', N'user6@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1995-06-06T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code6', N'Number6', 2, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (22, 4, N'Trần Duy Quyên', N'0610322940', N'@Thanhvinh2002', N'Male', N'user7@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1996-07-07T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code7', N'Number7', 2, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (23, 4, N'Võ Hữu Hạnh', N'0579596834', N'@Thanhvinh2002', N'Female', N'user8@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1997-08-08T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code8', N'Number8', 2, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (24, 4, N'Phan Văn Nam', N'0770989392', N'@Thanhvinh2002', N'Male', N'user9@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1998-09-09T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code9', N'Number9', 1, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (25, 4, N'Ngô Quốc Hoàng', N'0894022363', N'@Thanhvinh2002', N'Female', N'user10@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code10', N'Number10', 1, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (26, 4, N'Lê Hữu Hưng', N'0342718074', N'@Thanhvinh2002', N'Male', N'user11@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2000-11-11T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code11', N'Number11', 1, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (27, 4, N'Bùi Hồng Nam', N'0329735862', N'@Thanhvinh2002', N'Female', N'user12@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2001-12-12T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code12', N'Number12', 1, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (28, 4, N'Ngô Đức Tâm', N'0941613253', N'@Thanhvinh2002', N'Male', N'user13@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2002-01-13T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code13', N'Number13', 1, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (29, 4, N'Vũ Kim Hưng', N'0982974621', N'@Thanhvinh2002', N'Female', N'user14@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2003-02-14T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code14', N'Number14', 1, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (30, 4, N'Phan Thị Lan', N'0617067011', N'@Thanhvinh2002', N'Male', N'user15@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2004-03-15T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code15', N'Number15', 1, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (31, 4, N'Bùi Thanh Nam', N'0740635872', N'@Thanhvinh2002', N'Female', N'user16@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2005-04-16T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code16', N'Number16', 1, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (32, 4, N'Vũ Gia Quyên', N'0451693094', N'@Thanhvinh2002', N'Male', N'user17@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2006-05-17T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code17', N'Number17', 1, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (33, 4, N'Nguyễn Tuấn Anh', N'0386566167', N'@Thanhvinh2002', N'Female', N'user18@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2007-06-18T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code18', N'Number18', 1, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (34, 4, N'Bùi Thị Trung', N'0481682810', N'@Thanhvinh2002', N'Male', N'user19@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2008-07-19T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code19', N'Number19', 1, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (35, 4, N'Trần Quốc Anh', N'0837786543', N'@Thanhvinh2002', N'Female', N'user20@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2009-08-20T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code20', N'Number20', 2, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (36, 4, N'Hồ Hồng Tâm', N'0883861662', N'@Thanhvinh2002', N'Male', N'user21@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2010-09-21T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code21', N'Number21', 2, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (37, 4, N'Đặng Thị Nhung', N'0579065999', N'@Thanhvinh2002', N'Female', N'user22@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2011-10-22T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code22', N'Number22', 2, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (38, 4, N'Hoàng Thị Hạnh', N'0585226468', N'@Thanhvinh2002', N'Male', N'user23@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2012-11-23T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code23', N'Number23', 2, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (39, 4, N'Phan Thanh Anh', N'0362998139', N'@Thanhvinh2002', N'Female', N'user24@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2013-12-24T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code24', N'Number24', 2, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (40, 4, N'Đỗ Đức Trung', N'0546884701', N'@Thanhvinh2002', N'Male', N'user25@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2014-01-25T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code25', N'Number25', NULL, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (41, 4, N'Vũ Quốc Hạnh', N'0472809966', N'@Thanhvinh2002', N'Female', N'user26@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2015-02-26T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code26', N'Number26', NULL, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (42, 4, N'Vũ Gia Nhung', N'0361973900', N'@Thanhvinh2002', N'Male', N'user27@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2016-03-27T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code27', N'Number27', NULL, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (43, 4, N'Đỗ Văn Trung', N'0728902811', N'@Thanhvinh2002', N'Female', N'user28@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2017-04-28T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code28', N'Number28', NULL, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (44, 4, N'Phan Tuấn Hoàng', N'0340515230', N'@Thanhvinh2002', N'Male', N'user29@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2018-05-29T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code29', N'Number29', NULL, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (45, 4, N'Trần Hồng Nam', N'0442662270', N'@Thanhvinh2002', N'Female', N'user30@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2019-06-30T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:15:24.017' AS DateTime), CAST(N'2024-11-07T14:15:24.017' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code30', N'Number30', NULL, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (46, 4, N'Hồ Hữu Tuấn', N'0775438459', N'@Thanhvinh2002', N'Male', N'user31@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1990-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code1', N'Number31', 1, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (47, 4, N'Trần Đức Lan', N'0372490905', N'@Thanhvinh2002', N'Female', N'user32@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1991-02-02T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code2', N'Number32', 1, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (48, 4, N'Hồ Thanh Hạnh', N'0948907907', N'@Thanhvinh2002', N'Male', N'user33@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1992-03-03T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code3', N'Number33', 1, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (49, 4, N'Nguyễn Đức Quyên', N'0464902523', N'@Thanhvinh2002', N'Female', N'user34@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1993-04-04T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code4', N'Number34', 1, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (50, 4, N'Vũ Hồng Trung', N'0960940174', N'@Thanhvinh2002', N'Male', N'user35@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1994-05-05T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code5', N'Number35', 2, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (51, 4, N'Nguyễn Văn Trung', N'0547170181', N'@Thanhvinh2002', N'Female', N'user36@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1995-06-06T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code6', N'Number36', 2, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (52, 4, N'Bùi Anh Hạnh', N'0371485409', N'@Thanhvinh2002', N'Male', N'user37@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1996-07-07T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code7', N'Number37', 2, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (53, 4, N'Hồ Gia Hiếu', N'0348522815', N'@Thanhvinh2002', N'Female', N'user38@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1997-08-08T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code8', N'Number38', 2, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (54, 4, N'Bùi Hồng Hưng', N'0932126550', N'@Thanhvinh2002', N'Male', N'user39@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1998-09-09T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code9', N'Number39', NULL, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (55, 4, N'Phạm Văn Ngọc', N'0881709245', N'@Thanhvinh2002', N'Female', N'user40@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code10', N'Number40', NULL, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (56, 4, N'Hồ Quốc Tuấn', N'0716447961', N'@Thanhvinh2002', N'Male', N'user41@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2000-11-11T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code11', N'Number41', NULL, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (57, 4, N'Hồ Thị Hưng', N'0355876240', N'@Thanhvinh2002', N'Female', N'user42@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2001-12-12T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code12', N'Number42', NULL, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (58, 4, N'Ngô Đức Hạnh', N'0950165472', N'@Thanhvinh2002', N'Male', N'user43@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2002-01-13T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code13', N'Number43', NULL, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (59, 4, N'Ngô Duy Hạnh', N'0556700894', N'@Thanhvinh2002', N'Female', N'user44@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2003-02-14T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code14', N'Number44', NULL, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (60, 4, N'Võ Minh Hạnh', N'0454388073', N'@Thanhvinh2002', N'Male', N'user45@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2004-03-15T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code15', N'Number45', NULL, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (61, 4, N'Vũ Anh Hạnh', N'0325385675', N'@Thanhvinh2002', N'Female', N'user46@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2005-04-16T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code16', N'Number46', 1, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (62, 4, N'Lê Tuấn Ngọc', N'0779574819', N'@Thanhvinh2002', N'Male', N'user47@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2006-05-17T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code17', N'Number47', 2, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (63, 4, N'Võ Anh Nhung', N'0912005509', N'@Thanhvinh2002', N'Female', N'user48@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2007-06-18T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code18', N'Number48', NULL, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (64, 4, N'Bùi Duy Hưng', N'0323150917', N'@Thanhvinh2002', N'Male', N'user49@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2008-07-19T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code19', N'Number49', NULL, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (65, 4, N'Phạm Đức Trung', N'0914126630', N'@Thanhvinh2002', N'Female', N'user50@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2009-08-20T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code20', N'Number50', NULL, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (66, 4, N'Đặng Thị Tâm', N'0393474758', N'@Thanhvinh2002', N'Male', N'user51@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2010-09-21T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code21', N'Number51', NULL, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (67, 4, N'Đỗ Minh Nam', N'0971618670', N'@Thanhvinh2002', N'Female', N'user52@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2011-10-22T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code22', N'Number52', NULL, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (68, 4, N'Hoàng Quốc Ngọc', N'0911739220', N'@Thanhvinh2002', N'Male', N'user53@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2012-11-23T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code23', N'Number53', NULL, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (69, 4, N'Vũ Quốc Nhung', N'0552623904', N'@Thanhvinh2002', N'Female', N'user54@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2013-12-24T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code24', N'Number54', NULL, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (70, 4, N'Bùi Hồng Hạnh', N'0996254315', N'@Thanhvinh2002', N'Male', N'user55@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2014-01-25T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code25', N'Number55', NULL, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (71, 4, N'Đặng Hồng Hiếu', N'0951535010', N'@Thanhvinh2002', N'Female', N'user56@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2015-02-26T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code26', N'Number56', 1, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (72, 4, N'Phạm Quốc Trung', N'0483575473', N'@Thanhvinh2002', N'Male', N'user57@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2016-03-27T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code27', N'Number57', 1, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (73, 4, N'Trần Anh Hạnh', N'0395870762', N'@Thanhvinh2002', N'Female', N'user58@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2017-04-28T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code28', N'Number58', 2, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (74, 4, N'Vũ Thanh Hạnh', N'0548126953', N'@Thanhvinh2002', N'Male', N'user59@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2018-05-29T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code29', N'Number59', 2, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (75, 4, N'Võ Hồng Lan', N'0651444121', N'@Thanhvinh2002', N'Female', N'user60@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2019-06-30T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:20:05.190' AS DateTime), CAST(N'2024-11-07T14:20:05.190' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code30', N'Number60', NULL, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (76, 4, N'Hoàng Anh Nhung', N'0427729127', N'@Thanhvinh2002', N'Male', N'user61@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1990-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code1', N'Number61', NULL, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (77, 4, N'Bùi Đức Lan', N'0874213047', N'@Thanhvinh2002', N'Female', N'user62@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1991-02-02T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code2', N'Number62', 1, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (78, 4, N'Nguyễn Minh Nhung', N'0679279223', N'@Thanhvinh2002', N'Male', N'user63@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1992-03-03T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code3', N'Number63', 2, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (79, 4, N'Bùi Hữu Nhung', N'0375690725', N'@Thanhvinh2002', N'Female', N'user64@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1993-04-04T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code4', N'Number64', 2, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (80, 4, N'Phạm Văn Nam', N'0443242972', N'@Thanhvinh2002', N'Male', N'user65@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1994-05-05T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code5', N'Number65', 2, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (81, 4, N'Hoàng Anh Hưng', N'0397435227', N'@Thanhvinh2002', N'Female', N'user66@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1995-06-06T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code6', N'Number66', 2, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (82, 4, N'Phạm Gia Anh', N'0321933540', N'@Thanhvinh2002', N'Male', N'user67@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1996-07-07T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code7', N'Number67', 2, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (83, 4, N'Đỗ Văn Anh', N'0517099145', N'@Thanhvinh2002', N'Female', N'user68@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1997-08-08T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code8', N'Number68', 2, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (84, 4, N'Nguyễn Hữu Hưng', N'0717870755', N'@Thanhvinh2002', N'Male', N'user69@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1998-09-09T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code9', N'Number69', 2, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (85, 4, N'Ngô Thị Quyên', N'0594045930', N'@Thanhvinh2002', N'Female', N'user70@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T14:21:48.103' AS DateTime), CAST(N'2024-11-07T14:21:48.103' AS DateTime), N'Admin', N'Admin', 1, 0, 1, N'Code10', N'Number70', NULL, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (86, 5, N'Hoàng Gia Ngọc', N'0513303358', N'@Thanhvinh2002', N'Male', N'Porter1@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1990-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code1', N'Number1', 1, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (87, 5, N'Nguyễn Hồng Nhung', N'0520526215', N'@Thanhvinh2002', N'Female', N'Porter2@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1991-02-02T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code2', N'Number2', 1, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (88, 5, N'Trần Hồng Trung', N'0818431753', N'@Thanhvinh2002', N'Male', N'Porter3@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1992-03-03T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code3', N'Number3', 1, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (89, 5, N'Phan Thị Nam', N'0830713035', N'@Thanhvinh2002', N'Female', N'Porter4@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1993-04-04T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code4', N'Number4', 1, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (90, 5, N'Phan Gia Tâm', N'0627529611', N'@Thanhvinh2002', N'Male', N'Porter5@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1994-05-05T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code5', N'Number5', 1, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (91, 5, N'Nguyễn Minh Anh', N'0679042645', N'@Thanhvinh2002', N'Female', N'Porter6@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1995-06-06T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code6', N'Number6', 1, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (92, 5, N'Phan Thanh Hưng', N'0874639836', N'@Thanhvinh2002', N'Male', N'Porter7@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1996-07-07T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code7', N'Number7', 1, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (93, 5, N'Ngô Hồng Hoàng', N'0521971844', N'@Thanhvinh2002', N'Female', N'Porter8@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1997-08-08T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code8', N'Number8', 1, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (94, 5, N'Bùi Văn Hoàng', N'0465961187', N'@Thanhvinh2002', N'Male', N'Porter9@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1998-09-09T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code9', N'Number9', 1, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (95, 5, N'Vũ Thanh Anh', N'0440009081', N'@Thanhvinh2002', N'Female', N'Porter10@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-07T16:54:31.073' AS DateTime), CAST(N'2024-11-07T16:54:31.073' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code10', N'Number10', 1, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (96, 3, N'Vũ Gia Hoàng', N'0759775987', N'string', NULL, N'user12233@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (97, 3, N'Phạm Hồng Tuấn', N'0317768736', N'string', NULL, N'user1w2233@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (98, 2, N'Võ Văn Quyên', N'0863535376', N'@Thanhvinh2002', N'Male', N'reviewer3@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (99, 2, N'Vũ Thanh Ngọc', N'0630438682', N'@Thanhvinh2002', N'Male', N'reviewer4@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (100, 2, N'Nguyễn Minh Trung', N'0690862228', N'@Thanhvinh2002', N'Male', N'reviewer5@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (101, 2, N'Vũ Văn Hạnh', N'0421696577', N'@Thanhvinh2002', N'Male', N'reviewer6@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (102, 2, N'Phan Tuấn Hạnh', N'0467411983', N'@Thanhvinh2002', N'Male', N'reviewer7@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, 2, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (103, 2, N'Đặng Hồng Nam', N'0892674306', N'@Thanhvinh2002', N'Male', N'reviewer8@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, 2, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (104, 2, N'Đỗ Tuấn Anh', N'0642622363', N'@Thanhvinh2002', N'Male', N'reviewer9@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-10-10T00:00:00.000' AS DateTime), 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, 2, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (105, 2, N'Đỗ Tuấn Tâm', N'0350898989', N'@Thanhvinh2002', N'Male', N'reviewer10@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, 2, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (106, 2, N'Võ Anh Nam', N'0749056555', N'@Thanhvinh2002', N'Male', N'reviewer11@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, 2, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (107, 2, N'Hoàng Kim Trung', N'0357589788', N'@Thanhvinh2002', N'Male', N'reviewer12@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, 2, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (108, 5, N'Lê Duy Lan', N'0575990773', N'@Thanhvinh2002', N'Male', N'porter11@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1990-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code1', N'Number1', 1, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (109, 5, N'Lê Duy Lan', N'0575990773', N'@Thanhvinh2002', N'Female', N'porter12@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1991-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code2', N'Number2', 1, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (110, 5, N'Hoàng Kim Tuấn', N'0695800092', N'@Thanhvinh2002', N'Male', N'porter13@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1992-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code3', N'Number3', 1, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (111, 5, N'Trần Hữu Nam', N'0690222482', N'@Thanhvinh2002', N'Female', N'porter14@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1993-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code4', N'Number4', 1, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (112, 5, N'Lê Gia Ngọc', N'0487922749', N'@Thanhvinh2002', N'Male', N'porter15@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1994-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code5', N'Number5', 1, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (113, 5, N'Nguyễn Đức Ngọc', N'0784257825', N'@Thanhvinh2002', N'Female', N'porter16@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1995-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code6', N'Number6', 1, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (114, 5, N'Đặng Thanh Hiếu', N'0463995975', N'@Thanhvinh2002', N'Male', N'porter17@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1996-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code7', N'Number7', 1, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (115, 5, N'Trần Hữu Tuấn', N'0850301879', N'@Thanhvinh2002', N'Female', N'porter18@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1997-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code8', N'Number8', 1, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (116, 5, N'Võ Tuấn Lan', N'0383153608', N'@Thanhvinh2002', N'Male', N'porter19@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1998-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code9', N'Number9', 1, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (117, 5, N'Hồ Quốc Trung', N'0627049279', N'@Thanhvinh2002', N'Female', N'porter20@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code10', N'Number10', 1, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (118, 5, N'Nguyễn Quốc Anh', N'0955347292', N'@Thanhvinh2002', N'Male', N'porter21@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2000-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code11', N'Number11', 1, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (119, 5, N'Vũ Đức Hiếu', N'0810736529', N'@Thanhvinh2002', N'Female', N'porter22@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2001-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code12', N'Number12', 1, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (120, 5, N'Đỗ Gia Hưng', N'0769115867', N'@Thanhvinh2002', N'Male', N'porter23@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2002-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code13', N'Number13', 1, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (121, 5, N'Võ Hữu Nhung', N'0765295834', N'@Thanhvinh2002', N'Female', N'porter24@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2003-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code14', N'Number14', 1, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (122, 5, N'Phạm Hữu Tuấn', N'0630328588', N'@Thanhvinh2002', N'Male', N'porter25@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2004-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code15', N'Number15', 1, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (123, 5, N'Trần Duy Quyên', N'0727631589', N'@Thanhvinh2002', N'Female', N'porter26@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2005-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code16', N'Number16', 1, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (124, 5, N'Phạm Minh Hạnh', N'0476497923', N'@Thanhvinh2002', N'Male', N'porter27@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2006-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code17', N'Number17', 1, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (125, 5, N'Hồ Thanh Nam', N'0970350134', N'@Thanhvinh2002', N'Female', N'porter28@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2007-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code18', N'Number18', 1, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (126, 5, N'Đỗ Văn Hiếu', N'0531004621', N'@Thanhvinh2002', N'Male', N'porter29@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2008-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code19', N'Number19', 1, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (127, 5, N'Bùi Gia Tâm', N'0437706976', N'@Thanhvinh2002', N'Female', N'porter30@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2009-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code20', N'Number20', 1, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (128, 5, N'Lê Anh Quyên', N'0551879260', N'@Thanhvinh2002', N'Male', N'porter31@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2000-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code11', N'Number11', 1, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (129, 5, N'Trần Duy Nhung', N'0973305506', N'@Thanhvinh2002', N'Female', N'porter32@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2001-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code12', N'Number12', 1, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (130, 5, N'Vũ Văn Lan', N'0856717732', N'@Thanhvinh2002', N'Male', N'porter33@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2002-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code13', N'Number13', 1, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (131, 5, N'Hồ Quốc Lan', N'0661782107', N'@Thanhvinh2002', N'Female', N'porter34@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2003-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code14', N'Number14', 1, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (132, 5, N'Bùi Kim Trung', N'0791200392', N'@Thanhvinh2002', N'Male', N'porter35@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2004-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code15', N'Number15', 1, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (133, 5, N'Bùi Gia Trung', N'0887388338', N'@Thanhvinh2002', N'Female', N'porter36@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2005-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code16', N'Number16', 1, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (134, 5, N'Võ Quốc Trung', N'0838668137', N'@Thanhvinh2002', N'Male', N'porter37@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2006-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code17', N'Number17', 1, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (135, 5, N'Hoàng Hồng Ngọc', N'0614074008', N'@Thanhvinh2002', N'Female', N'porter38@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2007-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code18', N'Number18', 1, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (136, 5, N'Đặng Hữu Anh', N'0726871571', N'@Thanhvinh2002', N'Male', N'porter39@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2008-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code19', N'Number19', 1, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (137, 5, N'Hoàng Văn Tâm', N'0327376232', N'@Thanhvinh2002', N'Female', N'porter40@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2009-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:04:13.263' AS DateTime), CAST(N'2024-11-13T19:04:13.263' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code20', N'Number20', 1, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (138, 5, N'Vũ Quốc Tâm', N'0827472959', N'@Thanhvinh2002', N'Male', N'porter41@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1990-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code1', N'Number1', 2, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (139, 5, N'Nguyễn Kim Tâm', N'0467506356', N'@Thanhvinh2002', N'Female', N'porte42@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1991-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code2', N'Number2', 2, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (140, 5, N'Bùi Duy Hưng', N'0436259529', N'@Thanhvinh2002', N'Male', N'porter43@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1992-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code3', N'Number3', 2, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (141, 5, N'Phạm Quốc Quyên', N'0449457559', N'@Thanhvinh2002', N'Female', N'porter44@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1993-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code4', N'Number4', 2, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (142, 5, N'Lê Minh Tâm', N'0411003254', N'@Thanhvinh2002', N'Male', N'porter45@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1994-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code5', N'Number5', 2, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (143, 5, N'Lê Quốc Lan', N'0930014370', N'@Thanhvinh2002', N'Female', N'porter46@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1995-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code6', N'Number6', 2, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (144, 5, N'Vũ Văn Lan', N'0571497412', N'@Thanhvinh2002', N'Male', N'porter47@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1996-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code7', N'Number7', 2, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (145, 5, N'Ngô Quốc Hạnh', N'0754360953', N'@Thanhvinh2002', N'Female', N'porter48@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1997-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code8', N'Number8', 2, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (146, 5, N'Đỗ Hồng Anh', N'0740060629', N'@Thanhvinh2002', N'Male', N'porter49@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1998-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code9', N'Number9', 2, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (147, 5, N'Nguyễn Duy Nam', N'0996631633', N'@Thanhvinh2002', N'Female', N'porter50@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'1999-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code10', N'Number10', 2, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (148, 5, N'Bùi Thị Nhung', N'0853282776', N'@Thanhvinh2002', N'Male', N'porter51@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2000-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code11', N'Number11', 2, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (149, 5, N'Hồ Anh Quyên', N'0929343361', N'@Thanhvinh2002', N'Female', N'porter52@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2001-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code12', N'Number12', 2, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (150, 5, N'Vũ Kim Hiếu', N'0316216107', N'@Thanhvinh2002', N'Male', N'porter53@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2002-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code13', N'Number13', 2, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (151, 5, N'Nguyễn Đức Hoàng', N'0368205120', N'@Thanhvinh2002', N'Female', N'porter54@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2003-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code14', N'Number14', 2, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (152, 5, N'Phạm Thanh Hiếu', N'0520224705', N'@Thanhvinh2002', N'Male', N'porter55@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2004-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code15', N'Number15', 2, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (153, 5, N'Võ Văn Hạnh', N'0953232280', N'@Thanhvinh2002', N'Female', N'porter56@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2005-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code16', N'Number16', 2, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (154, 5, N'Phan Anh Nam', N'0692045435', N'@Thanhvinh2002', N'Male', N'porter57@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2006-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code17', N'Number17', 2, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (155, 5, N'Võ Minh Hiếu', N'0962982524', N'@Thanhvinh2002', N'Female', N'porter58@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2007-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code18', N'Number18', 2, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (156, 5, N'Phạm Minh Hiếu', N'0836735976', N'@Thanhvinh2002', N'Male', N'porter59@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2008-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code19', N'Number19', 2, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (157, 5, N'Ngô Văn Hiếu', N'0380067128', N'@Thanhvinh2002', N'Female', N'porter60@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2009-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code20', N'Number20', 2, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (158, 5, N'Trần Hồng Tâm', N'0619527713', N'@Thanhvinh2002', N'Male', N'porter61@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2000-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code11', N'Number11', 2, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (159, 5, N'Đặng Hữu Hạnh', N'0444845508', N'@Thanhvinh2002', N'Female', N'porter62@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2001-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code12', N'Number12', 2, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (160, 5, N'Ngô Thị Hiếu', N'0797648448', N'@Thanhvinh2002', N'Male', N'porter63@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2002-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code13', N'Number13', 2, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (161, 5, N'Đặng Quốc Quyên', N'0898003259', N'@Thanhvinh2002', N'Female', N'porter64@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2003-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code14', N'Number14', 2, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (162, 5, N'Nguyễn Kim Hoàng', N'0679898111', N'@Thanhvinh2002', N'Male', N'porter65@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2004-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code15', N'Number15', 2, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (163, 5, N'Đặng Anh Trung', N'0478896972', N'@Thanhvinh2002', N'Female', N'porter66@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2005-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code16', N'Number16', 2, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (164, 5, N'Nguyễn Đức Nhung', N'0754217730', N'@Thanhvinh2002', N'Male', N'porter67@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2006-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code17', N'Number17', 2, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (165, 5, N'Phạm Hữu Hạnh', N'0948439555', N'@Thanhvinh2002', N'Female', N'porter68@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2007-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code18', N'Number18', 2, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (166, 5, N'Đỗ Thanh Nam', N'0771779499', N'@Thanhvinh2002', N'Male', N'porter69@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2008-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code19', N'Number19', 2, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (167, 5, N'Võ Quốc Nhung', N'0635678528', N'@Thanhvinh2002', N'Female', N'porter70@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2009-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code20', N'Number20', 2, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (168, 5, N'Bùi Minh Lan', N'0669744000', N'@Thanhvinh2002', N'Male', N'porter71@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2000-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code11', N'Number11', 2, N'20240909')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (169, 5, N'Trần Duy Hưng', N'0492872657', N'@Thanhvinh2002', N'Female', N'porter72@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2001-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code12', N'Number12', 2, N'20241010')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (170, 5, N'Phan Tuấn Nam', N'0588936566', N'@Thanhvinh2002', N'Male', N'porter73@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2002-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code13', N'Number13', 2, N'20241111')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (171, 5, N'Phạm Đức Tâm', N'0790637094', N'@Thanhvinh2002', N'Female', N'porter74@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2003-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code14', N'Number14', 2, N'20241212')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (172, 5, N'Võ Quốc Trung', N'0613408278', N'@Thanhvinh2002', N'Male', N'porter75@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2004-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code15', N'Number15', 2, N'20240101')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (173, 5, N'Vũ Tuấn Ngọc', N'0822344929', N'@Thanhvinh2002', N'Female', N'porter76@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2005-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code16', N'Number16', 2, N'20240202')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (174, 5, N'Phạm Duy Hạnh', N'0465042298', N'@Thanhvinh2002', N'Male', N'porter77@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2006-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code17', N'Number17', 2, N'20240303')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (175, 5, N'Nguyễn Thanh Tâm', N'0378267566', N'@Thanhvinh2002', N'Female', N'porter78@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2007-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code18', N'Number18', 2, N'20240404')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (176, 5, N'Phạm Thị Ngọc', N'0523028906', N'@Thanhvinh2002', N'Male', N'porter79@example.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', CAST(N'2008-01-01T00:00:00.000' AS DateTime), 0, 0, CAST(N'2024-11-13T19:06:57.617' AS DateTime), CAST(N'2024-11-13T19:06:57.617' AS DateTime), N'Admin', N'Admin', 1, 0, 0, N'Code19', N'Number19', 2, N'20240505')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (194, 3, N'Trần Thị Tuấn', N'0919735009', N'@Thanhvinh2002', N'Male', N'vinh1234@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20240606')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (198, 3, N'phương', N'+84374027409', N'Ps@12345', N'Male', N'phuong25@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20240707')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (202, 3, N'shun', N'+84394581438', N'@Thanhvinh2002', N'Male', N'vinh9999@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20240808')
GO
INSERT [dbo].[User] ([Id], [RoleId], [Name], [Phone], [Password], [Gender], [Email], [AvatarUrl], [Dob], [IsBanned], [IsDeleted], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModifiedVersion], [IsInitUsed], [IsDriver], [CodeIntroduce], [NumberIntroduce], [GroupId], [Shard]) VALUES (204, 3, N'vinh nguyễn', N'+84382703625', N'@Thanhvinh2002', N'Male', N'vinhnguyen@gmail.com', N'https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'20240909')
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO
SET IDENTITY_INSERT [dbo].[UserInfo] ON 
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (1, 1, N'Cavet xe', N'https://lambangcapgia.info/wp-content/uploads/2021/03/dich-vu-lam-cavet-xe-1.jpg', N'1', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (2, 1, N'Bảo hiểm y tế', N'https://thaison.vn/pic/general/images/su-dung-the-BHYT-kham-chua-benh-1.jpg', N'2', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (3, 1, N'Giấy khám sức khỏe', N'https://vinaucare.com/wp-content/uploads/2018/02/M%E1%BA%ABu-gi%E1%BA%A5y-kh%C3%A1m-s%E1%BB%A9c-kh%E1%BA%BBo-theo-tt14-kh%C3%A1m-s%E1%BB%A9c-kh%E1%BB%8Fe-th%E1%BA%BB-xanh.jpg', N'3', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (4, 1, N'CCCD', N'https://cdn.tgdd.vn/Files/2021/04/18/1344478/cach-lam-can-cuoc-cong-dan-cccd-online_800x450.jpg', N'4', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (5, 1, N'Giấy phép lái xe', N'https://thongtingiaypheplaixe.com/wp-content/uploads/2019/11/IMAG0587-scaled.jpg', N'5', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (6, 1, N'Địa chỉ thường trú', N'https://storage.timviec365.vn/timviec365/pictures/images/dia-chi-cu-tru-la-gi-ho-so-dang-ky.jpg', N'6', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (7, 2, N'Cavet xe', N'https://vieclam123.vn/ckfinder/userfiles/images/images/ca-vet-xe-la-gi.jpg', N'1', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (8, 2, N'Bảo hiểm y tế', N'https://images2.thanhnien.vn/thumb_w/576/528068263637045248/2023/10/14/the-bhyt-giay-16972503246502066707966.jpg', N'2', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (9, 2, N'Giấy khám sức khỏe', N'https://vinaucare.com/wp-content/uploads/2018/02/M%E1%BA%ABu-gi%E1%BA%A5y-kh%C3%A1m-s%E1%BB%A9c-kh%E1%BA%BBo-theo-tt14-kh%C3%A1m-s%E1%BB%A9c-kh%E1%BB%8Fe-th%E1%BA%BB-xanh.jpg', N'3', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (10, 2, N'CCCD', N'https://cdn.tgdd.vn/Files/2021/04/18/1344478/cach-lam-can-cuoc-cong-dan-cccd-online_800x450.jpg', N'4', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (11, 2, N'Giấy phép lái xe', N'https://thongtingiaypheplaixe.com/wp-content/uploads/2019/11/IMAG0587-scaled.jpg', N'5', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (12, 2, N'Địa chỉ tạm trú', N'https://storage.timviec365.vn/timviec365/pictures/images/dia-chi-cu-tru-la-gi-ho-so-dang-ky.jpg', N'7', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (13, 2, N'Hồ sơ hình sự', N'https://f.hoatieu.vn/data/image/2018/01/23/mau-bia-ho-so-vu-an-hinh-su.jpg', N'8', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (14, 3, N'Cavet xe', N'https://i.ytimg.com/vi/XtmI2ho8Hck/maxresdefault.jpg', N'1', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (15, 3, N'Bảo hiểm y tế', N'https://thaison.vn/pic/general/images/su-dung-the-BHYT-kham-chua-benh-1.jpg', N'2', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (16, 3, N'Giấy khám sức khỏe', N'https://phapluatplus.baophapluat.vn/stores/news_dataimages/wwwphapluatplusvn/052021/28/09/mua-ban-giay-kham-suc-khoe-lien-tinh-32-.2129.jpg', N'3', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (17, 3, N'CCCD', N'https://cdn.tgdd.vn/Files/2021/07/13/1367936/cccd2_800x450.jpg', N'4', 0)
GO
INSERT [dbo].[UserInfo] ([Id], [UserId], [Type], [ImageUrl], [Value], [IsDeleted]) VALUES (18, 3, N'Giấy phép lái xe', N'https://cdn.vietnam.vn/wp-content/uploads/2024/08/Giay-phep-lai-xe-duoc-doi-cap-lai-nhu-the.jpg', N'5', 0)
GO
SET IDENTITY_INSERT [dbo].[UserInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[Wallet] ON 
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (1, 1, 799618304, 1, CAST(N'2024-10-25T23:46:49.703' AS DateTime), CAST(N'2024-12-02T01:16:03.857' AS DateTime), 0, NULL, NULL, NULL, NULL, N'string', N'string', N'string', N'string')
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (2, 5, 128885, 1, CAST(N'2024-10-25T23:46:49.703' AS DateTime), CAST(N'2024-11-28T02:19:32.003' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (3, 3, 129197584, 1, CAST(N'2024-10-25T23:46:49.703' AS DateTime), CAST(N'2024-12-07T14:20:13.350' AS DateTime), 0, NULL, NULL, NULL, NULL, N'1242 6332 7233 4879', N'Vietcombank', N'10/30', N'Nguyen Thanh tam')
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (4, 4, 943630848, 0, CAST(N'2024-10-25T23:46:49.703' AS DateTime), CAST(N'2024-12-07T14:20:13.387' AS DateTime), 0, NULL, NULL, NULL, NULL, N'12345543234', N'fd', NULL, NULL)
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (5, 13, 49900, NULL, CAST(N'2024-11-05T06:33:23.687' AS DateTime), CAST(N'2024-12-02T01:16:55.557' AS DateTime), 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (6, 14, 50100, 1, CAST(N'2024-11-05T18:20:42.527' AS DateTime), CAST(N'2024-12-02T01:16:55.593' AS DateTime), 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (7, 96, 0, 1, CAST(N'2024-11-11T16:19:56.977' AS DateTime), CAST(N'2024-11-11T16:19:56.997' AS DateTime), 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (8, 97, 0, 1, CAST(N'2024-11-11T16:20:57.007' AS DateTime), CAST(N'2024-11-11T16:20:57.007' AS DateTime), 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (21, 194, 0, 1, CAST(N'2024-11-24T14:58:45.900' AS DateTime), CAST(N'2024-11-24T14:58:45.900' AS DateTime), 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (25, 198, 39710952, 1, CAST(N'2024-11-25T22:25:28.460' AS DateTime), CAST(N'2024-11-30T21:00:15.783' AS DateTime), 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (29, 202, 0, 1, CAST(N'2024-11-28T11:33:51.497' AS DateTime), CAST(N'2024-11-28T11:33:51.497' AS DateTime), 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Wallet] ([Id], [UserId], [Balance], [Tier], [CreatedAt], [UpdatedAt], [IsLocked], [LockReason], [LockAmount], [Type], [FixedSalary], [BankNumber], [BankName], [ExpirdAt], [CardHolderName]) VALUES (31, 204, 0, 1, CAST(N'2024-12-02T04:10:23.507' AS DateTime), CAST(N'2024-12-02T04:10:23.507' AS DateTime), 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Wallet] OFF
GO
/****** Object:  Index [UQ__Truck__1788CC4D7110290E]    Script Date: 12/7/2024 2:44:39 PM ******/
ALTER TABLE [dbo].[Truck] ADD UNIQUE NONCLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ_Wallet_UserId]    Script Date: 12/7/2024 2:44:39 PM ******/
ALTER TABLE [dbo].[Wallet] ADD  CONSTRAINT [UQ_Wallet_UserId] UNIQUE NONCLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Assignment]  WITH CHECK ADD  CONSTRAINT [FK_Assignment_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[Assignment] CHECK CONSTRAINT [FK_Assignment_Booking]
GO
ALTER TABLE [dbo].[Assignment]  WITH CHECK ADD  CONSTRAINT [FK_Assignment_BookingDetails] FOREIGN KEY([BookingDetailsId])
REFERENCES [dbo].[BookingDetails] ([Id])
GO
ALTER TABLE [dbo].[Assignment] CHECK CONSTRAINT [FK_Assignment_BookingDetails]
GO
ALTER TABLE [dbo].[Assignment]  WITH CHECK ADD  CONSTRAINT [FK_Assignment_ScheduleBooking] FOREIGN KEY([ScheduleBookingId])
REFERENCES [dbo].[ScheduleBooking] ([Id])
GO
ALTER TABLE [dbo].[Assignment] CHECK CONSTRAINT [FK_Assignment_ScheduleBooking]
GO
ALTER TABLE [dbo].[Assignment]  WITH CHECK ADD  CONSTRAINT [FK_Assignment_Truck] FOREIGN KEY([TruckId])
REFERENCES [dbo].[Truck] ([Id])
GO
ALTER TABLE [dbo].[Assignment] CHECK CONSTRAINT [FK_Assignment_Truck]
GO
ALTER TABLE [dbo].[Assignment]  WITH CHECK ADD  CONSTRAINT [FK_Assignment_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Assignment] CHECK CONSTRAINT [FK_Assignment_User]
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
ALTER TABLE [dbo].[BookingDetails]  WITH CHECK ADD  CONSTRAINT [FK_BookingDetails_Service] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Service] ([Id])
GO
ALTER TABLE [dbo].[BookingDetails] CHECK CONSTRAINT [FK_BookingDetails_Service]
GO
ALTER TABLE [dbo].[BookingTracker]  WITH CHECK ADD  CONSTRAINT [FK_BookingTracker_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[BookingTracker] CHECK CONSTRAINT [FK_BookingTracker_Booking]
GO
ALTER TABLE [dbo].[FeeDetails]  WITH CHECK ADD  CONSTRAINT [FK_FeeDetails_Booking1] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[FeeDetails] CHECK CONSTRAINT [FK_FeeDetails_Booking1]
GO
ALTER TABLE [dbo].[FeeDetails]  WITH CHECK ADD  CONSTRAINT [FK_FeeDetails_FeeSetting] FOREIGN KEY([FeeSettingId])
REFERENCES [dbo].[FeeSetting] ([Id])
GO
ALTER TABLE [dbo].[FeeDetails] CHECK CONSTRAINT [FK_FeeDetails_FeeSetting]
GO
ALTER TABLE [dbo].[FeeSetting]  WITH CHECK ADD  CONSTRAINT [FK_FeeSetting_HouseType] FOREIGN KEY([HouseTypeId])
REFERENCES [dbo].[HouseType] ([Id])
GO
ALTER TABLE [dbo].[FeeSetting] CHECK CONSTRAINT [FK_FeeSetting_HouseType]
GO
ALTER TABLE [dbo].[FeeSetting]  WITH CHECK ADD  CONSTRAINT [FK_FeeSetting_Service] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Service] ([Id])
GO
ALTER TABLE [dbo].[FeeSetting] CHECK CONSTRAINT [FK_FeeSetting_Service]
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
ALTER TABLE [dbo].[PromotionCategory]  WITH CHECK ADD  CONSTRAINT [FK_PromotionCategory_Service] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Service] ([Id])
GO
ALTER TABLE [dbo].[PromotionCategory] CHECK CONSTRAINT [FK_PromotionCategory_Service]
GO
ALTER TABLE [dbo].[ScheduleWorking]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleWorking_Group] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Group] ([Id])
GO
ALTER TABLE [dbo].[ScheduleWorking] CHECK CONSTRAINT [FK_ScheduleWorking_Group]
GO
ALTER TABLE [dbo].[ScheduleWorking]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleWorking_Schedule] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[Schedule] ([Id])
GO
ALTER TABLE [dbo].[ScheduleWorking] CHECK CONSTRAINT [FK_ScheduleWorking_Schedule]
GO
ALTER TABLE [dbo].[Service]  WITH CHECK ADD  CONSTRAINT [FK_Service_ParentService] FOREIGN KEY([ParentServiceId])
REFERENCES [dbo].[Service] ([Id])
GO
ALTER TABLE [dbo].[Service] CHECK CONSTRAINT [FK_Service_ParentService]
GO
ALTER TABLE [dbo].[Service]  WITH CHECK ADD  CONSTRAINT [FK_Service_TruckCategory] FOREIGN KEY([TruckCategoryId])
REFERENCES [dbo].[TruckCategory] ([Id])
GO
ALTER TABLE [dbo].[Service] CHECK CONSTRAINT [FK_Service_TruckCategory]
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
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Truck] CHECK CONSTRAINT [FK_Truck_User]
GO
ALTER TABLE [dbo].[TruckImg]  WITH CHECK ADD  CONSTRAINT [FK_TruckImg_Truck] FOREIGN KEY([TruckId])
REFERENCES [dbo].[Truck] ([Id])
GO
ALTER TABLE [dbo].[TruckImg] CHECK CONSTRAINT [FK_TruckImg_Truck]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Group] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Group] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Group]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Role]
GO
ALTER TABLE [dbo].[UserInfo]  WITH CHECK ADD  CONSTRAINT [FK_UserInfo_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserInfo] CHECK CONSTRAINT [FK_UserInfo_User]
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD  CONSTRAINT [FK_Voucher_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[Voucher] CHECK CONSTRAINT [FK_Voucher_Booking]
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD  CONSTRAINT [FK_Voucher_PromotionCategory] FOREIGN KEY([PromotionCategoryId])
REFERENCES [dbo].[PromotionCategory] ([Id])
GO
ALTER TABLE [dbo].[Voucher] CHECK CONSTRAINT [FK_Voucher_PromotionCategory]
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD  CONSTRAINT [FK_Voucher_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Voucher] CHECK CONSTRAINT [FK_Voucher_User]
GO
ALTER TABLE [dbo].[Wallet]  WITH CHECK ADD  CONSTRAINT [FK_Wallet_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wallet] CHECK CONSTRAINT [FK_Wallet_User]
GO
