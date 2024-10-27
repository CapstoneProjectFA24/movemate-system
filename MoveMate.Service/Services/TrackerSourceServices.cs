using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Grpc.Core.Metadata;

namespace MoveMate.Service.Services
{
    public class TrackerSourceServices : ITrackerSourceServices
    {

        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<TrackerSourceServices> _logger;

        public TrackerSourceServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TrackerSourceServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }
        public async Task<OperationResult<bool>> DeleteTrackerSource(int trackerSourceId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var trackerSource = await _unitOfWork.TrackerSourceRepository.GetByIdAsync(trackerSourceId);
                if (trackerSource == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTrackerSource);
                    return result;
                }
                if (trackerSource.IsDeleted == true) 
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TrackerSourceIsDeleted);
                    return result;
                }
                trackerSource.IsDeleted = true;

                _unitOfWork.TrackerSourceRepository.Update(trackerSource);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteTrackerSource, true);
            }
            catch 
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }
            return result;
        }


    }
}
