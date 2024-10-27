using AutoMapper;
using Azure.Messaging;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class UserService : IUserServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }


        public async Task<OperationResult<List<UserResponse>>> GetAll(GetAllUserRequest request)
        {
            var result = new OperationResult<List<UserResponse>>();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.UserRepository.Get(
                    filter: filter,
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder(),
                    includeProperties: "Role"
                );
                var listResponse = _mapper.Map<List<UserResponse>>(entities);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.GetListUserFail , listResponse);
                    return result;
                }

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListUserSuccess , listResponse);

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


        public async Task<OperationResult<List<UserInfoResponse>>> GetUserInfoByUserIdAsync(GetAllUserInfoRequest request)
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
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.GetListUserInfoFail, listResponse);
                    return result;
                }

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListUserInfoSuccess, listResponse);

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
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.RegisterSuccess, userResponse);
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
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                }

                user.IsBanned = true;

                await _unitOfWork.SaveChangesAsync();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BanUserSuccess, true);
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
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUserInfo);
                    return result;
                }

                if (userInfo.IsDeleted == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UserInfoIsDeleted);
                    return result;
                }
                userInfo.IsDeleted = true;

                _unitOfWork.UserInfoRepository.Update(userInfo);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteUserInfo, true);
            }
            catch
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }
            return result;
        }


    }
}