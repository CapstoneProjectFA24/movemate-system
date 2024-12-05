using AutoMapper;
using MoveMate.Domain.Models;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelRequests.Booking;
using MoveMate.Service.ViewModels.ModelResponse;
using MoveMate.Service.ViewModels.ModelResponses;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;

namespace MoveMate.Service.Commons.AutoMapper
{
    public class AutoMapperService : Profile
    {
        public AutoMapperService()
        {
            // Mapping for UserResponse
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.Wallet.Id))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<UpdateUserRequest, User>();
            CreateMap<CustomerToRegister, User>()
                .ForMember(dest => dest.Wallet, opt => opt.Ignore());
            // Mapping for AccountToken
            CreateMap<AccountTokenRequest, AccountToken>();
            CreateMap<AccountToken, AccountTokenRequest>();

            // Mapping for AccountRequest -> User
            CreateMap<AccountRequest, User>();
            CreateMap<PhoneLoginRequest, User>();
            CreateMap<CreateUserRequest, User>();


            // Mapping for User -> AccountResponse
            CreateMap<User, AccountResponse>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.Id))
                ; // Adjust as needed
            //Address
            CreateMap<UserInfo, UserInfoResponse>();
            CreateMap<CreateUserInfoRequest, UserInfo>();
            CreateMap<UpdateUserInfoRequest, UserInfo>();
            CreateMap<User, GetUserResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));


            //Register
            CreateMap<User, RegisterResponse>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                ;

            //Booking
            CreateMap<Assignment, AssignmentResponse>();
            CreateMap<Booking, BookingResponse>()
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments))
                .ForMember(dest => dest.FeeDetails, opt => opt.MapFrom(src => src.FeeDetails))
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails))
                .ForMember(dest => dest.BookingTrackers, opt => opt.MapFrom(src => src.BookingTrackers))
                .ForMember(dest => dest.Vouchers, opt => opt.MapFrom(src => src.Vouchers));

            CreateMap<Booking, BookingRegisterResponse>();
            //.ForMember(dest => dest.ServiceDetails, opt => opt.MapFrom(src => src.ServiceDetails))
            //.ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails))
            //.ForMember(dest => dest.HouseTypes, opt => opt.MapFrom(src => src.HouseTypes))
            //.ForMember(dest => dest.BookingTrackers, opt => opt.MapFrom(src => src.BookingTrackers));
            CreateMap<ChangeBookingAtRequest, Booking>();
            CreateMap<BookingServiceDetailsUpdateRequest, Booking>()
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails));
            CreateMap<DriverUpdateBookingRequest, Booking>()
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails));
            CreateMap<PorterUpdateDriverRequest, Booking>()
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails));
            CreateMap<BookingDetail, BookingDetailsResponse>()
                 .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Service != null ? src.Service.ImageUrl : null));
            CreateMap<BookingDetail, BookingDetailWaitingResponse>()
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments));

            CreateMap<FailReportRequest, BookingDetail>();

            CreateMap<BookingDetailRequest, BookingDetail>();

            CreateMap<BookingTracker, BookingTrackerResponse>();
            CreateMap<HouseType, HouseTypeResponse>();
            // REQUEST
            CreateMap<BookingRegisterRequest, Booking>()
                .ForMember(dest => dest.BookingDetails,
                    opt => opt.Ignore()) // Ignore ServiceDetails; handle separately if needed
                .ForMember(dest => dest.TotalFee, opt => opt.Ignore()) // Ignore TotalFee; calculate separately
                .ForMember(dest => dest.FeeDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Vouchers, opt => opt.Ignore());
            CreateMap<BookingBasicInfoUpdateRequest, Booking>();
            CreateMap<ReviewAtRequest, Booking>();
            CreateMap<StatusRequest, Booking>()
                .ForMember(dest => dest.Vouchers, opt => opt.MapFrom(src => src.Vouchers));

            CreateMap<BookingDetail, BookingDetailReport>()
                .ForMember(dest => dest.BookingAt, opt => opt.MapFrom(src => src.Booking.BookingAt))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Booking.TruckNumber))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Booking.User));

            //.ForMember(dest => dest.HouseTypeId, opt => opt.Ignore());

            //Schedule
            //CreateMap<ScheduleBooking, ScheduleResponse>()
            //    .ForMember(dest => dest.ScheduleDetails, opt => opt.MapFrom(src => src.ScheduleBookingDetails));

            //CreateMap<ScheduleBookingDetail, ScheduleDetailResponse>();

            CreateMap<ExceptionRequest, BookingTracker>();

            CreateMap<Schedule, ScheduleDailyResponse>()
            .ForMember(dest => dest.ScheduleWorkingId,
                       opt => opt.MapFrom(src => src.ScheduleWorkings.FirstOrDefault().Id))
            .ForMember(dest => dest.GroupId,
                       opt => opt.MapFrom(src => src.ScheduleWorkings.FirstOrDefault().GroupId));

            CreateMap<ScheduleRequest, Schedule>()
           .ForMember(dest => dest.Date, opt => opt.MapFrom(src =>
               string.IsNullOrWhiteSpace(src.Date)
                   ? (DateOnly?)null
                   : DateOnly.ParseExact(src.Date, "MM/dd/yyyy")));

            CreateMap<CreateScheduleWorkingRequest, ScheduleWorking>()
     .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
         !string.IsNullOrWhiteSpace(src.StartDate) ? TimeOnly.Parse(src.StartDate) : (TimeOnly?)null))
     .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
         !string.IsNullOrWhiteSpace(src.EndDate) ? TimeOnly.Parse(src.EndDate) : (TimeOnly?)null));
            CreateMap<ScheduleWorking, ScheduleWorkingResponse>()
               ;
            CreateMap<Group, GroupResponse>()
                .ForMember(dest => dest.ScheduleWorkings, opt => opt.MapFrom(src => src.ScheduleWorkings))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users));
            CreateMap<CreateGroupRequest, Group>();
            CreateMap<UpdateGroupRequest, Group>();

            CreateMap<ScheduleBooking, ScheduleBookingResponse>()
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments));

            CreateMap<CreateScheduleBookingRequest, ScheduleBooking>();
            //Truck
            CreateMap<TruckCategory, TruckCateResponse>();
            CreateMap<TruckCategory, TruckCateDetailResponse>();
            CreateMap<CreateTruckImgRequest, TruckImg>();
            CreateMap<TruckImg, TruckImageResponse>();
            CreateMap<TruckCategoryRequest, TruckCategory>();
            CreateMap<CreateTruckRequest, Truck>()
                .ForMember(dest => dest.TruckImgs, opt => opt.MapFrom(src => src.TruckImgs));
            CreateMap<UpdateTruckRequest, Truck>();
            CreateMap<Truck, TruckResponse>()
                .ForMember(dest => dest.TruckImgs, opt => opt.MapFrom(src => src.TruckImgs));
            CreateMap<TruckImg, TruckImgResponse>();
            CreateMap<TruckImgRequest, TruckImg>();


            //Transaction 
            CreateMap<Transaction, TransactionResponse>();
            //Payment
            CreateMap<Payment, PaymentResponse>()
                .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

            //House Type
            CreateMap<HouseType, HouseTypesResponse>();
            //.ForMember(dest => dest.HouseTypeSettings, opt => opt.MapFrom(src => src.HouseTypeSettings));
            // CreateMap<HouseTypeSetting, HouseTypeSettingResponse>();
            //CreateMap<CreateHouseTypeSetting, HouseTypeSetting>()
            //   .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<CreateHouseTypeRequest, HouseType>();
            CreateMap<UpdateHouseTypeRequest, HouseType>();




            //Service
            CreateMap<MoveMate.Domain.Models.Service, ServiceResponse>();
            CreateMap<MoveMate.Domain.Models.Service, ServicesResponse>()
                .ForMember(dest => dest.InverseParentService, opt => opt.MapFrom(src => src.InverseParentService));
            CreateMap<UpdateServiceRequest, ServiceResponse>();

            CreateMap<BookingDetail, BookingDetailRequest>();
            CreateMap<CreateServiceRequest, MoveMate.Domain.Models.Service>()
                .ForMember(dest => dest.InverseParentService, opt => opt.MapFrom(src => src.InverseParentService));
            CreateMap<ServiceRequest, MoveMate.Domain.Models.Service>();
            //CreateMap<List<ServiceDetail>, List<ServiceDetailResponse>>();

            //Wallet
            CreateMap<Wallet, WalletResponse>();


            // Mapping for TruckCategory to TruckCategoryResponse
            CreateMap<TruckCategory, TruckCategoryResponse>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));


            // Free
            CreateMap<FeeDetail, FeeDetailResponse>();
            CreateMap<FeeDetailRequest, FeeDetail>();
            CreateMap<CreateFeeSettingRequest, FeeSetting>();
            //CreateMap<List<FeeDetail>, List<FeeDetailResponse>>();

            // Resource
            CreateMap<ResourceRequest, TrackerSource>();
            CreateMap<TrackerSource, TrackerSourceResponse>();
            CreateMap<TrackerSourceRequest, BookingTracker>()
                .ForMember(dest => dest.TrackerSources, opt => opt.MapFrom(src => src.ResourceList));

            //Tracker
            CreateMap<BookingTracker, BookingTrackerResponse>()
                .ForMember(dest => dest.IsInsurance, opt => opt.MapFrom(src => src.IsInsurance))
                .ForMember(dest => dest.IsCompensation, opt => opt.MapFrom(src => src.IsCompensation))
                .ForMember(dest => dest.TrackerSources, opt => opt.MapFrom(src => src.TrackerSources));
            CreateMap<BookingTracker, ExceptionResponse>()
                .ForMember(dest => dest.Deposit, opt => opt.MapFrom(src => src.Booking.Deposit)) 
                .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src => src.Booking.Status))
                .ForMember(dest => dest.PickupAddress, opt => opt.MapFrom(src => src.Booking.PickupAddress))
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.Booking.DeliveryAddress))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Booking.Total))
                .ForMember(dest => dest.TotalReal, opt => opt.MapFrom(src => src.Booking.TotalReal))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Booking.Note))
                .ForMember(dest => dest.BookingAt, opt => opt.MapFrom(src => src.Booking.BookingAt))
                .ForMember(dest => dest.IsReviewOnline, opt => opt.MapFrom(src => src.Booking.IsReviewOnline))
                .ForMember(dest => dest.BookingIsInsurance, opt => opt.MapFrom(src => src.Booking.IsInsurance))
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Booking.Assignments))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Booking.User));


            CreateMap<User, UserExceptionResponse>()
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.Wallet.BankName))
                .ForMember(dest => dest.BankNumber, opt => opt.MapFrom(src => src.Wallet.BankNumber))
                .ForMember(dest => dest.CardHolderName, opt => opt.MapFrom(src => src.Wallet.CardHolderName));

            //Fee Setting
            CreateMap<FeeSetting, FeeSettingResponse>();
            CreateMap<FeeSetting, GetFeeSettingResponse>();

            //Promotion
            CreateMap<CreatePromotionRequest, PromotionCategory>();
            CreateMap<UpdatePromotionRequest, PromotionCategory>();
            CreateMap<PromotionCategory, PromotionResponse>()
                .ForMember(dest => dest.Vouchers, opt => opt.MapFrom(src => src.Vouchers));

            //Voucher
            CreateMap<VoucherRequest, Voucher>();
            CreateMap<Voucher, VoucherResponse>();
            CreateMap<CreateVoucherRequest, Voucher>();
            CreateMap<AddVoucherRequest, Voucher>();

            //Notification
            CreateMap<MoveMate.Domain.Models.Notification, NotificationResponse>();
        }
    }
}