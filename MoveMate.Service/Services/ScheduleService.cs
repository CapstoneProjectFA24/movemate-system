using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.GoongMap;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class ScheduleService : IScheduleService
    {

        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<ScheduleService> _logger;
        

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScheduleService> logger)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
            _logger = logger;          
        }

        //public async Task<OperationResult<ScheduleDailyResponse>> CreateSchedule(ScheduleRequest request)
        //{
        //    var result = new OperationResult<ScheduleDailyResponse>();
        //    try
        //    {

        //    }catch(Exception ex) 
        //    {
        //        result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
        //        return result;
        //    }
        //}
    }
}
