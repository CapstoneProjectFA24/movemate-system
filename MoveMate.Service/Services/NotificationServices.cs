using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class NotificationServices : INotificationServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<NotificationServices> _logger;

        public NotificationServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<NotificationServices> logger,
           IMessageProducer producer, IFirebaseServices firebaseServices)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;    
        }

        public async Task CreateUserDeviceAsync(CreateNotificationRequest userDeviceRequest, IEnumerable<Claim> claims)
        {
            try
            {
                Claim sidClaim = claims.First(x => x.Type.ToLower() == "sid");
                string idAccount = sidClaim.Value;
                Notification existedUserDevice = await this._unitOfWork.NotificationRepository.GetUserDeviceAsync(userDeviceRequest.FCMToken);
                if (existedUserDevice is null)
                {
                    User existedAccount = await this._unitOfWork.UserRepository.GetUserAsync(int.Parse(idAccount));
                    Notification userDevice = new Notification()
                    {
                        User = existedAccount,
                        FCMToken = userDeviceRequest.FCMToken
                    };
                    await this._unitOfWork.NotificationRepository.CreateUserDeviceAsync(userDevice);
                    await this._unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task DeleteUserDeviceAsync(int userDeviceId)
        {
            try
            {
                Notification existedUserDevice = await this._unitOfWork.NotificationRepository.GetUserDeviceAsync(userDeviceId);
                if (existedUserDevice is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.UserDeviceIdNotExist);
                }
                this._unitOfWork.NotificationRepository.DeleteUserDevice(existedUserDevice);
                await this._unitOfWork.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("User device id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

    }
}
