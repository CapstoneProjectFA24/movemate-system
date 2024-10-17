using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class StaffDailyServices : IStaffDailyServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<StaffDailyServices> _logger;

        public StaffDailyServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<StaffDailyServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public Task<OperationResult<List<StaffDailyResponse>>> GetAll(GetAllStaffDailyRequest request)
        {
            throw new NotImplementedException();
        }
    }
}