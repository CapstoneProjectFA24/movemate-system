using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.GoongMap;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class NotificationService : INotificationService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;


        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<NotificationService> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<bool>> ManagerReadMail(int userId, int notiId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user.RoleId != 6)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotManager);
                    return result;
                }

                var noti = await _unitOfWork.NotificationRepository.GetByIdAsync(notiId);
                if (noti == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundNotification);
                    return result;
                }

                noti.IsRead = true;
                await _unitOfWork.NotificationRepository.SaveOrUpdateAsync(noti);
                await _unitOfWork.SaveChangesAsync();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.ReadNoti,
                    true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }
            return result;
        }

        public async Task<OperationResult<List<NotificationResponse>>> GetAll(GetAllNotificationRequest request)
        {
            var result = new OperationResult<List<NotificationResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.NotificationRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<NotificationResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListNotificationEmpty,
                        listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListNotificationSuccess,
                    listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }



        }

        public async Task<OperationResult<List<BookingTrackerResponse>>> GetAllBookingTracker(GetAllBookingTrackerReport request)
        {
            var result = new OperationResult<List<BookingTrackerResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.BookingTrackerRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder(),
                    includeProperties: "TrackerSources"
                );
                var listResponse = _mapper.Map<List<BookingTrackerResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListReportSuccess,
                        listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListReportSuccess,
                    listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }
    }
}
