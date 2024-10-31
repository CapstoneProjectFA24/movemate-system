using AutoMapper;
using MoveMate.Domain.Models;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelRequests.Booking;
using MoveMate.Service.ViewModels.ModelResponse;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.Service.Commons.AutoMapper
{
    public class AutoMapperService : Profile
    {
        public AutoMapperService()
        {
            // Mapping for UserResponse
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<UpdateUserRequest, User>();

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
            CreateMap<User, GetUserResponse>();


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
                .ForMember(dest => dest.BookingTrackers, opt => opt.MapFrom(src => src.BookingTrackers));

            CreateMap<Booking, BookingRegisterResponse>();
            //.ForMember(dest => dest.ServiceDetails, opt => opt.MapFrom(src => src.ServiceDetails))
            //.ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails))
            //.ForMember(dest => dest.HouseTypes, opt => opt.MapFrom(src => src.HouseTypes))
            //.ForMember(dest => dest.BookingTrackers, opt => opt.MapFrom(src => src.BookingTrackers));

            CreateMap<BookingServiceDetailsUpdateRequest, Booking>()
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails));
            CreateMap<BookingDetail, BookingDetailsResponse>();
            CreateMap<BookingDetailRequest, BookingDetail>();
            
            CreateMap<BookingTracker, BookingTrackerResponse>();
            CreateMap<HouseType, HouseTypeResponse>();
            // REQUEST
            CreateMap<BookingRegisterRequest, Booking>()
                .ForMember(dest => dest.BookingDetails,
                    opt => opt.Ignore()) // Ignore ServiceDetails; handle separately if needed
                .ForMember(dest => dest.TotalFee, opt => opt.Ignore()) // Ignore TotalFee; calculate separately
                .ForMember(dest => dest.FeeDetails, opt => opt.Ignore());
            CreateMap<BookingBasicInfoUpdateRequest, Booking>();
            CreateMap<ReviewAtRequest, Booking>();
            CreateMap<StatusRequest, Booking>();

            //.ForMember(dest => dest.HouseTypeId, opt => opt.Ignore());

            //Schedule
            CreateMap<ScheduleBooking, ScheduleResponse>()
                .ForMember(dest => dest.ScheduleDetails, opt => opt.MapFrom(src => src.ScheduleBookingDetails));

            CreateMap<ScheduleBookingDetail, ScheduleDetailResponse>();

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
            CreateMap<Transaction,  TransactionResponse>();

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
            //CreateMap<List<FeeDetail>, List<FeeDetailResponse>>();

            // Resource
            CreateMap<ResourceRequest, TrackerSource>();
            CreateMap<TrackerSource, TrackerSourceResponse>();

            //Tracker
            CreateMap<BookingTracker, BookingTrackerResponse>()
                .ForMember(dest => dest.TrackerSources, opt => opt.MapFrom(src => src.TrackerSources));

            //Fee Setting
            CreateMap<FeeSetting, FeeSettingResponse>();
            CreateMap<FeeSetting, GetFeeSettingResponse>();

            //Promotion
            CreateMap<CreatePromotionRequest, PromotionCategory>()
                .ForMember(dest => dest.Vouchers, opt => opt.MapFrom(src => src.Vouchers));
            CreateMap<UpdatePromotionRequest, PromotionCategory>();
            CreateMap<PromotionCategory, PromotionResponse>()
                .ForMember(dest => dest.Vouchers, opt => opt.MapFrom(src => src.Vouchers));

            //Voucher
            CreateMap<VoucherRequest, Voucher>();
            CreateMap<Voucher, VoucherResponse>();
            CreateMap<CreateVoucherRequest, Voucher>();
        }
    }
}