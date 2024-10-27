﻿using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IUserServices
    {
        public Task<OperationResult<List<UserResponse>>> GetAll(GetAllUserRequest request);
        public Task<UserResponse> GetAccountAsync(int id, IEnumerable<Claim> claims);
        public Task<OperationResult<List<UserInfoResponse>>> GetUserInfoByUserIdAsync(GetAllUserInfoRequest request);
        public Task UpdateUserAsync(string id, UpdateUserRequest updateUserRequest);

        public Task<OperationResult<UserResponse>> CreateUser(AdminCreateUserRequest request);

        public Task<OperationResult<bool>> BanUser(int id);
        public Task<OperationResult<bool>> DeleteUserInfo(int id);
    }
}