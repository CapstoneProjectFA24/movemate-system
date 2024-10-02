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
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<UserResponse>>(entities);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "List User is Empty!", listResponse);
                    return result;
                }

                result.AddResponseStatusCode(StatusCode.Ok, "Get List User Done.", listResponse);

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


        public async Task<OperationResult<UserInfoResponse>> GetUserInfoByUserIdAsync(string userId)
        {
            var result = new OperationResult<UserInfoResponse>();

            try
            {
                // Retrieve the user and their address information using the userId
                var userInfo = await _unitOfWork.UserInfoRepository.GetUserInfoByUserIdAsync(int.Parse(userId));
                
                // Check if the user's address is available
                if (userInfo != null)
                {
                    // Map the UserInfo to a UserAddressResponse
                    var addressResponse = _mapper.Map<UserInfoResponse>(userInfo);
                    result.Payload = addressResponse;
                    result.Message = "User address retrieved successfully.";
                }
                else
                {
                    result.AddError(StatusCode.NotFound, $"Address for user '{userId}' not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user address.");
                result.AddError(StatusCode.ServerError, "An unexpected error occurred.");
            }

            return result;
        }


        public async Task UpdateUserAsync(string id, UpdateUserRequest updateUserRequest)
        {
            int userId = int.Parse(id);

            try
            {         
                var existingUser = await _unitOfWork.UserRepository.GetUserAsync(userId);
                if (existingUser == null)
                {
                    throw new Exception("User not found.");
                }
                var updatedUser = _mapper.Map(updateUserRequest, existingUser);
                await _unitOfWork.UserRepository.UpdateAsync(updatedUser);
                var checkResult = _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user: " + ex.Message);
            }
        }


    }
}
