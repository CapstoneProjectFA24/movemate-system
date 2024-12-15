using AutoMapper;
using Azure.Messaging;
using Catel.Collections;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.ThirdPartyService.RabbitMQ.DTO;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Grpc.Core.Metadata;

namespace MoveMate.Service.Services
{
    public class UserService : IUserServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly IMessageProducer _producer;
        private readonly IFirebaseServices _firebaseServices;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger, IEmailService emailService, IFirebaseServices firebaseServices, IMessageProducer producer)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
            _emailService = emailService;
            _firebaseServices = firebaseServices;
            _producer = producer;
        }


        public async Task<OperationResult<List<UserResponse>>> GetAll(GetAllUserRequest request)
        {
            var result = new OperationResult<List<UserResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.UserRepository.GetWithCount(
                filter: request.GetExpressions(),
                pageIndex: request.page,
                pageSize: request.per_page,
                orderBy: request.GetOrder(),
                includeProperties: "Role,Truck,Wallet,UserInfos"
            );
                var listResponse = _mapper.Map<List<UserResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListUserEmpty, listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListUserSuccess, listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<UserResponse> GetAccountAsync(int idAccount, IEnumerable<Claim> claims)
        {
            try
            {
                Claim registeredEmailClaim = claims.First(x => x.Type == ClaimTypes.Email);
                string email = registeredEmailClaim.Value;

                User existedAccount = await this._unitOfWork.UserRepository.GetUserAsync(idAccount);
                if (existedAccount is null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }

                if (existedAccount.Email.Equals(email) == false)
                {
                    throw new BadRequestException(MessageConstant.AccountMessage.AccountIdNotBelongYourAccount);
                }

                UserResponse getAccountResponse = this._mapper.Map<UserResponse>(existedAccount);
                return getAccountResponse;
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Account id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Account id", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }


        public async Task<OperationResult<List<UserInfoResponse>>> GetUserInfoByUserIdAsync(
            GetAllUserInfoRequest request)
        {
            var result = new OperationResult<List<UserInfoResponse>>();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.UserInfoRepository.Get(
                    filter: filter,
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<UserInfoResponse>>(entities);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.GetListUserInfoFail,
                        listResponse);
                    return result;
                }

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListUserInfoSuccess,
                    listResponse);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }


        public async Task UpdateUserAsync(string id, UpdateUserRequest updateUserRequest)
        {
            int userId = int.Parse(id);
            var validationContext = new ValidationContext(updateUserRequest);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(updateUserRequest, validationContext, validationResults, true);

            if (!isValid)
            {
                throw new Exception(MessageConstant.FailMessage.ValidateField);
            }
            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetUserAsync(userId);
                if (existingUser == null)
                {
                    throw new NotFoundException(MessageConstant.FailMessage.NotFoundUser);
                }

                var updatedUser = _mapper.Map(updateUserRequest, existingUser);
                await _unitOfWork.UserRepository.UpdateAsync(updatedUser);
                var checkResult = _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(MessageConstant.FailMessage.UpdateUserFail);
            }
        }

        public async Task<OperationResult<UserResponse>> CreateUser(AdminCreateUserRequest request)
        {
            var result = new OperationResult<UserResponse>();
            try
            {
                // Check if the email already exists
                var userEmail = await _unitOfWork.UserRepository.GetUserAsyncByEmail(request.Email);
                if (userEmail != null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.EmailExist);
                    return result;
                }

                // Check if the phone already exists
                var userPhone = await _unitOfWork.UserRepository.GetUserByPhoneAsync(request.Phone);
                if (userPhone != null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.PhoneExist);
                    return result;
                }

                var roleId = await _unitOfWork.RoleRepository.GetByIdAsync(request.RoleId);
                if (roleId == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.RoleNotFound);
                    return result;
                }

                var user = _mapper.Map<User>(request);


                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                var userResponse = _mapper.Map<UserResponse>(user);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.RegisterSuccess,
                    userResponse);
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<bool>> BanUser(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    result.AddResponseErrorStatusCode(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser, false);
                    return result;
                }

                user.IsBanned = true;
                await _unitOfWork.UserRepository.SaveOrUpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BanUserSuccess, true);
            }
            catch (Exception ex)
            {
                result.AddResponseErrorStatusCode(StatusCode.ServerError, MessageConstant.FailMessage.ServerError, false);
            }

            return result;
        }

        public async Task<OperationResult<bool>> DeleteUser(int userId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    result.AddResponseErrorStatusCode(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser, false);
                    return result;
                }

                user.IsDeleted = true;
                await _unitOfWork.UserRepository.SaveOrUpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeletedUserSuccess, true);
            }
            catch (Exception ex)
            {
                result.AddResponseErrorStatusCode(StatusCode.ServerError, MessageConstant.FailMessage.ServerError, false);
            }

            return result;
        }

        public async Task<OperationResult<GetUserResponse>> UnBannedUser(int userId)
        {
            var result = new OperationResult<GetUserResponse>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }

                user.IsBanned = false;
                await _unitOfWork.UserRepository.SaveOrUpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                user = await _unitOfWork.UserRepository.GetByIdAsyncV1(userId, includeProperties: "Role,UserInfos,Wallet,Group");
                var response = _mapper.Map<GetUserResponse>(user);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BanUserSuccess, response);
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<bool>> DeleteUserInfo(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var userInfo = await _unitOfWork.UserInfoRepository.GetByIdAsync(id);
                if (userInfo == null)
                {
                    result.AddResponseErrorStatusCode(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUserInfo, false);
                    return result;
                }

                if (userInfo.IsDeleted == true)
                {
                    result.AddResponseErrorStatusCode(StatusCode.BadRequest, MessageConstant.FailMessage.UserInfoIsDeleted, false);
                    return result;
                }

                userInfo.IsDeleted = true;

                _unitOfWork.UserInfoRepository.Update(userInfo);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteUserInfo, true);
            }
            catch
            {
                result.AddResponseErrorStatusCode(StatusCode.ServerError, MessageConstant.FailMessage.ServerError, false);
            }

            return result;
        }

        public async Task<OperationResult<UserInfoResponse>> CreateUserInfo(CreateUserInfoRequest request)
        {
            var result = new OperationResult<UserInfoResponse>();
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
                return result;
            }
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync((int)request.UserId);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }

                var existingUserInfo = await _unitOfWork.UserInfoRepository
             .GetUserInfoByUserIdAndTypeAsync(user.Id, request.Type);


                if (existingUserInfo != null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UserInfoExist);
                    return result;
                }

                var userInfoReq = _mapper.Map<UserInfo>(request);

                await _unitOfWork.UserInfoRepository.AddAsync(userInfoReq);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<UserInfoResponse>(userInfoReq);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateUserInfo,
                    response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<UserInfoResponse>> UpdateUserInfoAsync(int id, UpdateUserInfoRequest request)
        {
            var result = new OperationResult<UserInfoResponse>();
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
                return result;
            }
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }

                var existingUserInfo = await _unitOfWork.UserInfoRepository.GetUserInfoByUserIdAndTypeAsync(id, request.Type);
                if (existingUserInfo == null) // Change to null check
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundUserInfo);
                    return result;
                }

                // Update properties from request to existingUserInfo
                ReflectionUtils.UpdateProperties(request, existingUserInfo);

                await _unitOfWork.UserInfoRepository.SaveOrUpdateAsync(existingUserInfo);
                var saveResult = await _unitOfWork.SaveChangesAsync(); // Ensure you await this

                // Check save result and return response
                if (saveResult > 0)
                {
                    existingUserInfo = await _unitOfWork.UserInfoRepository.GetByIdAsync(existingUserInfo.Id);
                    var response = _mapper.Map<UserInfoResponse>(existingUserInfo);

                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UserInfoUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UserInfoUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<GetUserResponse>> UpdateAccountAsync(int userId, UpdateAccountRequest request)
        {
            var result = new OperationResult<GetUserResponse>();

            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }

                // Update properties from request to existingUserInfo
                ReflectionUtils.UpdateProperties(request, user);

                await _unitOfWork.UserRepository.SaveOrUpdateAsync(user);
                var saveResult = await _unitOfWork.SaveChangesAsync(); // Ensure you await this

                // Check save result and return response
                if (saveResult > 0)
                {
                    user = await _unitOfWork.UserRepository.GetByIdAsync(userId, includeProperties: "Role,UserInfos,Wallet,Group");
                    var response = _mapper.Map<GetUserResponse>(user);

                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UserUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UserInfoUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<GetUserResponse>> GetUserById(int id)
        {
            var result = new OperationResult<GetUserResponse>();
            try
            {
                var entity =
                    await _unitOfWork.UserRepository.GetByIdAsync(id, includeProperties: "Role,UserInfos,Wallet,Group");
                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                }
                var productResponse = _mapper.Map<GetUserResponse>(entity);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetUserSuccess,
                        productResponse);


                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<BookingTrackerResponse>> UserReportException(int userId, ExceptionRequest request)
        {
            var result = new OperationResult<BookingTrackerResponse>();
            try
            {
                var checkBooking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(request.BookingId, userId);
                if (checkBooking == null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCannotPay);
                    return result;
                }
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(request.BookingId, includeProperties: "BookingTrackers.TrackerSources");

                if (booking.Status != BookingEnums.IN_PROGRESS.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.DamageReport);
                    return result;
                }

                if (request.IsInsurance == true)
                {
                    if (booking.IsInsurance == true)
                    {
                        var bookingDetail = await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(TypeServiceEnums.INSURANCE.ToString(), booking.Id);
                        if (bookingDetail == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingDetail);
                            return result;
                        }
                        var trackerReport = await _unitOfWork.BookingTrackerRepository.GetBookingTrackerByTypeAndBookingIdAsync(TrackerEnums.MONETARY.ToString(), booking.Id);

                        if (bookingDetail.Quantity <= trackerReport.Count())
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotEnoughInsurance);
                            return result;
                        }
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotInsurance);
                        return result;
                    }
                }

                var tracker = _mapper.Map<BookingTracker>(request);
                await _unitOfWork.BookingTrackerRepository.AddAsync(tracker);
                tracker.Type = TrackerEnums.MONETARY.ToString();
                tracker.IsCompensation = false;
                tracker.Status = StatusTrackerEnums.PENDING.ToString();
                tracker.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                List<TrackerSource> resourceList = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                tracker.TrackerSources = resourceList;

                booking.BookingTrackers.Add(tracker);

                await _unitOfWork.BookingRepository.SaveOrUpdateAsync(booking);
                await _unitOfWork.SaveChangesAsync();
                var assignmentPorter = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.PORTER.ToString(), booking.Id);
                var noti = new NotiListDto()
                {
                    BookingId = booking.Id,
                    StaffType = assignmentPorter.StaffType,
                    Type = NotificationEnums.CUSTOMER_REPORT.ToString(),
                };
                _producer.SendingMessage("movemate.notification_user", noti);
                //await _emailService.SendBookingSuccessfulEmailAsync(user.Email, response);
                var response = _mapper.Map<BookingTrackerResponse>(tracker);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AddTrackerReport, response);
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<GetUserResponse>> CreateStaff(CreateStaffRequest request)
        {
            var result = new OperationResult<GetUserResponse>();
            try
            {
                var requiredTypes = new List<string> {
                                    "CITIZEN_IDENTIFICATION_CARD",
                                    "HEALTH_CERTIFICATE",
                                    "DRIVER_LICENSE",
                                    "CRIMINAL_RECORD",
                                    "CURRICULUM_VITAE",
                                    "PORTRAIT"};

                if (request.RoleId == 4)
                {
                    requiredTypes.Add("TRUCK_NAME");
                }
                else
                {
                    var invalidTruckName = request.UserInfo.Any(u => u.Type == "TRUCK_NAME");
                    if (invalidTruckName)
                    {
                        result.AddError(StatusCode.BadRequest,
                            "UserInfo type 'TRUCK_NAME' is not allowed for this role.");
                        return result;
                    }
                }
                var missingTypes = requiredTypes.Except(request.UserInfo.Select(u => u.Type)).ToList();
                if (missingTypes.Any())
                {
                    result.AddError(StatusCode.BadRequest,
                        $"The following required UserInfo types are missing: {string.Join(", ", missingTypes)}");
                    return result;
                }


                // Check if the email already exists
                var userEmail = await _unitOfWork.UserRepository.GetUserAsyncByEmail(request.Email);
                if (userEmail != null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.EmailExist);
                    return result;
                }

                // Check if the phone already exists
                var userPhone = await _unitOfWork.UserRepository.GetUserByPhoneAsync(request.Phone);
                if (userPhone != null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.PhoneExist);
                    return result;
                }

                var roleId = await _unitOfWork.RoleRepository.GetByIdAsync((int)request.RoleId);
                if (roleId == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.RoleNotFound);
                    return result;
                }

                var user = _mapper.Map<User>(request);
                if (request.RoleId == 4)
                {
                    user.IsDriver = true;
                }

                // Map UserInfos
                List<UserInfo> resourceList = _mapper.Map<List<UserInfo>>(request.UserInfo);
                user.UserInfos = resourceList;

                // Create Wallet
                user.Wallet = new Wallet
                {
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsLocked = true,
                    Tier = 1
                };

                await _unitOfWork.UserRepository.AddAsync(user);
                _unitOfWork.Save();

                var staff = await _unitOfWork.UserRepository.GetByIdAsync(user.Id, includeProperties: "Role,Wallet,UserInfos,Group");
                var userResponse = _mapper.Map<GetUserResponse>(staff);

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.RegisterSuccess,
                    userResponse);
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }


        public async Task<OperationResult<GetUserResponse>> AcceptUser(int userId)
        {
            var result = new OperationResult<GetUserResponse>();
            try
            {
                var entity =
                    await _unitOfWork.UserRepository.GetByIdAsync(userId, includeProperties: "Role,UserInfos,Wallet,Group");
                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                }

                entity.IsAccepted = true;
                await _unitOfWork.UserRepository.SaveOrUpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                var userResponse = _mapper.Map<GetUserResponse>(entity);
                await _emailService.SendJobAcceptanceEmailAsync(entity.Email, userResponse);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetUserSuccess,
                        userResponse);
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }
    }
}