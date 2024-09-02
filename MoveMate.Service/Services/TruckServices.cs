using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class TruckServices : ITruckServices
    {

        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<TruckServices> _logger;
        public TruckServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TruckServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }
    }
}