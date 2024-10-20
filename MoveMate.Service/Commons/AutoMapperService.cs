using AutoMapper;
using MoveMate.Domain.Models;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelRequests.Booking;
using MoveMate.Service.ViewModels.ModelResponse;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Commons
{
    public class AutoMapperService : Profile
    {
        public AutoMapperService()
        {
            // Mapping for UserResponse
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.Id));
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
                .ForMember(dest => dest.Tokens,
                    opt => opt.MapFrom(src => src.Tokens.FirstOrDefault())); // Adjust as needed
            //Address
            CreateMap<UserInfo, UserInfoResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl));

            //Register
            CreateMap<User, RegisterResponse>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                ;

            //Booking
            CreateMap<BookingDetail, BookingDetailsResponse>();
            CreateMap<Booking, BookingResponse>()
                .ForMember(dest => dest.ServiceDetails, opt => opt.MapFrom(src => src.ServiceDetails))
                .ForMember(dest => dest.FeeDetails, opt => opt.MapFrom(src => src.FeeDetails))
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails))
                .ForMember(dest => dest.BookingTrackers, opt => opt.MapFrom(src => src.BookingTrackers));

            CreateMap<Booking, BookingRegisterResponse>();
            //.ForMember(dest => dest.ServiceDetails, opt => opt.MapFrom(src => src.ServiceDetails))
            //.ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails))
            //.ForMember(dest => dest.HouseTypes, opt => opt.MapFrom(src => src.HouseTypes))
            //.ForMember(dest => dest.BookingTrackers, opt => opt.MapFrom(src => src.BookingTrackers));

            CreateMap<BookingServiceDetailsUpdateRequest, Booking>()
                .ForMember(dest => dest.ServiceDetails, opt => opt.MapFrom(src => src.ServiceDetails));
            CreateMap<ServiceDetail, ServiceDetailsResponse>();
            CreateMap<ServiceDetailRequest, ServiceDetail>();
            CreateMap<BookingDetail, BookingDetailsResponse>();
            CreateMap<BookingTracker, BookingTrackerResponse>();
            CreateMap<HouseType, HouseTypeResponse>();
            // REQUEST
            CreateMap<BookingRegisterRequest, Booking>()
                .ForMember(dest => dest.ServiceDetails,
                    opt => opt.Ignore()) // Ignore ServiceDetails; handle separately if needed
                .ForMember(dest => dest.TotalFee, opt => opt.Ignore()) // Ignore TotalFee; calculate separately
                .ForMember(dest => dest.FeeDetails, opt => opt.Ignore());
            CreateMap<BookingBasicInfoUpdateRequest, Booking>();
            CreateMap<ReviewAtRequest, Booking>();
            CreateMap<StatusRequest, Booking>();

            //.ForMember(dest => dest.HouseTypeId, opt => opt.Ignore());

            //Schedule
            CreateMap<Schedule, ScheduleResponse>()
                .ForMember(dest => dest.ScheduleDetails, opt => opt.MapFrom(src => src.ScheduleDetails));

            CreateMap<ScheduleDetail, ScheduleDetailResponse>();

            //Truck
            CreateMap<TruckCategory, TruckCateResponse>();
            CreateMap<TruckCategory, TruckCateDetailResponse>();


            //House Type
            CreateMap<HouseType, HouseTypesResponse>()
                .ForMember(dest => dest.HouseTypeSettings, opt => opt.MapFrom(src => src.HouseTypeSettings));
            CreateMap<HouseTypeSetting, HouseTypeSettingResponse>();
            CreateMap<CreateHouseTypeSetting, HouseTypeSetting>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());


            //Service
            CreateMap<MoveMate.Domain.Models.Service, ServiceResponse>();
            CreateMap<MoveMate.Domain.Models.Service, ServicesResponse>()
                .ForMember(dest => dest.InverseParentService, opt => opt.MapFrom(src => src.InverseParentService));
            CreateMap<ServiceDetail, ServiceDetailsResponse>();

            //CreateMap<List<ServiceDetail>, List<ServiceDetailResponse>>();

            //Wallet
            CreateMap<Wallet, WalletResponse>();


            // Mapping for TruckCategory to TruckCategoryResponse
            CreateMap<TruckCategory, TruckCategoryResponse>();

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
        }
    }
}