using AutoMapper;
using MoveMate.Domain.Models;
using MoveMate.Service.ViewModels.ModelRequests;
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
                .ForMember(dest => dest.WalletIds, opt => opt.MapFrom(src => src.Wallets.Select(w => w.Id)));

            // Mapping for AccountToken
            CreateMap<AccountTokenRequest, AccountToken>();
            CreateMap<AccountToken, AccountTokenRequest>();

            // Mapping for AccountRequest -> User
            CreateMap<AccountRequest, User>();

            // Mapping for User -> AccountResponse
            CreateMap<User, AccountResponse>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.Id))
                .ForMember(dest => dest.Tokens, opt => opt.MapFrom(src => src.Tokens.FirstOrDefault())); // Adjust as needed
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
            CreateMap<Booking, BookingResponse>();
                


            //Schedule
            CreateMap<Schedule, ScheduleResponse>()
               .ForMember(dest => dest.ScheduleDetails, opt => opt.MapFrom(src => src.ScheduleDetails));

            CreateMap<ScheduleDetail, ScheduleDetailResponse>();
                
           

            //Truck
            CreateMap<TruckCategory, TruckCateResponse>();
            CreateMap<TruckCategory, TruckCateDetailResponse>();

        }
    }

}
