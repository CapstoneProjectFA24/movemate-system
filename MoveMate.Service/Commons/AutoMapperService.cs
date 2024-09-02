using AutoMapper;
using MoveMate.Domain.Models;
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
        public AutoMapperService() {

            CreateMap<User, UserResponse>()
     .ForMember(dest => dest.WalletIds, opt => opt.MapFrom(src => src.Wallets.Select(w => w.Id)));




        }
    }
}
