﻿using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.API.Middleware;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]

    public class UserController : BaseController
    {
        private readonly IUserServices _userService;

        private readonly IFirebaseMiddleware _firebaseMiddleware;

        public UserController(IUserServices userService, IFirebaseMiddleware firebaseMiddleware)
        {
            _firebaseMiddleware = firebaseMiddleware;
            _userService = userService;
        }

        /// <summary>
        /// 
        /// Get all users
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/get-all")]
        [Authorize(Roles = "1")]
        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllUserRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _userService.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }



        /// <summary>
        /// Get User Information by UserID 
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/info")]
        [Authorize]
        public async Task<IActionResult> GetAddressByUserIdAsync()
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value).ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var result = await _userService.GetUserInfoByUserIdAsync(userId);
            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return Ok(result.Payload);
        }


        [HttpPost(Name = "CreateUser")]
        public async Task<OperationResult<UserRecord>> CreateUser(CreateUserRequest user)
        {
            return await _firebaseMiddleware.CreateUser(
               user.Name,
               user.Password,
               user.Email,
               user.Phone
               );
        }

    }
}
