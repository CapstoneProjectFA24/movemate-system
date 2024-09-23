﻿using FirebaseAdmin.Auth;
using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoveMate.API.Middleware;
using MoveMate.API.Utils;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponse;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class AuthenticationsController : BaseController
    {
        private IAuthenticationService _authenticationService;
        private IFirebaseServices _firebaseService;

        private IOptions<Service.ViewModels.ModelRequests.JWTAuth> _jwtAuthOptions;
        private IValidator<AccountRequest> _accountRequestValidator;
        private IValidator<AccountTokenRequest> _accountTokenRequestValidator;
        private readonly ILogger<ExceptionMiddleware> _logger;
        // private IValidator<ResetPasswordRequest> _resetPasswordValidator;
        public AuthenticationsController(IAuthenticationService authenticationService, IOptions<JWTAuth> jwtAuthOptions,
            IValidator<AccountRequest> accountRequestValidator, IValidator<AccountTokenRequest> accountTokenRequestValidator,
            ILogger<ExceptionMiddleware> logger, IFirebaseServices firebaseServices)
        // IValidator<ResetPasswordRequest> resetPasswordValidator)
        {
            this._authenticationService = authenticationService;
            this._jwtAuthOptions = jwtAuthOptions;
            this._accountRequestValidator = accountRequestValidator;
            this._accountTokenRequestValidator = accountTokenRequestValidator;
            this._logger = logger;
            this._firebaseService = firebaseServices;
            // this._resetPasswordValidator = resetPasswordValidator;
        }

        #region Login API
        /// <summary>
        /// Login to access into the system by your account.
        /// </summary>
        /// <param name="account">
        /// Account object contains Email property and Password property. 
        /// Notice that the password must be hashed with MD5 algorithm before sending to Login API.
        /// </param>
        /// <returns>
        /// An Object with a json format that contains Account Id, Email, Role name, and a pair token (access token, refresh token).
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "email": "admin@gmail.com"
        ///             "password": "1"
        ///         }
        /// </remarks>
        /// <response code="200">Login Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        ///[HttpPost(APIEndPointConstant.Authentication.Login)]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostLoginAsync([FromBody] AccountRequest account)
        {
            try
            {
                var validationResult = await _accountRequestValidator.ValidateAsync(account);
                if (!validationResult.IsValid)
                {
                    var errors = ErrorUtil.GetErrorsString(validationResult);
                    var errorList = errors.Select(e => new Error { Code = MoveMate.Service.Commons.StatusCode.BadRequest, Message = e.ToString() });
                    return HandleErrorResponse((List<Error>)errorList);
                }

                var accountResponse = await _authenticationService.LoginAsync(account, _jwtAuthOptions.Value);
                return Ok(accountResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                var errors = new List<Error>
        {
            new Error
            {
                Code = MoveMate.Service.Commons.StatusCode.ServerError,
                Message = "An internal server error occurred."
            }
        };
                return HandleErrorResponse(errors);
            }
        }
        #endregion

        #region Re-GenerateTokens API
        /// <summary>
        /// Re-generate pair token from the old pair token that are provided by the MBKC system before.
        /// </summary>
        /// <param name="accountToken">
        /// AccountToken Object contains access token property and refresh token property.
        /// </param>
        /// <returns>
        /// The new pair token (Access token, Refresh token) to continue access into the MBKC system.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "accessToken": "abcxyz"
        ///             "refreshToken": "klmnopq"
        ///         }
        /// </remarks>
        /// <response code="200">Re-Generate Token Successfully.</response>
        /// <response code="404">Some Error about request data that are not found.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        ///[HttpPost(APIEndPointConstant.Authentication.ReGenerationTokens)]
        [HttpPost("Re")]
        [ProducesResponseType(typeof(AccountTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        ///[PermissionAuthorize(RoleConstants.Customer)]
        public async Task<IActionResult> PostReGenerateTokensAsync([FromBody] AccountTokenRequest accountToken)
        {
            var validationResult = await _accountTokenRequestValidator.ValidateAsync(accountToken);
            if (!validationResult.IsValid)
            {
                var errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var accountTokenResponse = await _authenticationService.ReGenerateTokensAsync(accountToken, _jwtAuthOptions.Value);
            return Ok(accountTokenResponse);
        }
        #endregion

        #region Register API
        /// <summary>
        /// Register a new account in the system.
        /// </summary>
        /// <param name="customerToRegister">The customer registration information (email).</param>
        /// <returns>The created account details.</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/authentications/register
        ///     {
        ///         "email": "user@example.com"
        ///     }
        /// </remarks>
        /// <response code="200">Registration successful.</response>
        /// <response code="400">Validation failed or email is already registered.</response>
        /// <response code="500">System error occurred.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync([FromBody] CustomerToRegister customerToRegister)
        {
            
                // Register user
                var response = await _authenticationService.Register(customerToRegister);
               
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            
        #endregion
    }
        [HttpPost("register/v2")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] CustomerToRegister customerToRegister)
        {

            // Register user
            var response = await _authenticationService.RegisterV2(customerToRegister);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);


        }

        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyToken([FromBody] TokenRequest tokenRequest)
        {
            try
            {
                
                var decodedToken = await _firebaseService.VerifyIdTokenAsync(tokenRequest.IdToken);

               
                if (decodedToken != null && !string.IsNullOrEmpty(decodedToken.Uid))
                {
                    var userId = decodedToken.Uid; 
                    var accountResponse = await _authenticationService.GenerateTokenWithUserIdAsync(userId, _jwtAuthOptions.Value);
                    return Ok(new
                    {
                        message = "Token verified and JWT generated successfully",
                        accessToken = accountResponse.Tokens.AccessToken, 
                        refreshToken = accountResponse.Tokens.RefreshToken 
                    });
                }

               
                return BadRequest(new { message = "Invalid token: UID not found" });
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { message = "Firebase token verification failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during token verification.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal server error occurred." });
            }
        }

        [HttpPost("verify-token/v2")]
        public async Task<IActionResult> VerifyTokenV2([FromBody] TokenRequest tokenRequest)
        {
            try
            {
                var decodedToken = await _firebaseService.VerifyIdTokenAsync(tokenRequest.IdToken);
                return Ok(new { isValid = decodedToken != null && !string.IsNullOrEmpty(decodedToken.Uid )});
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { message = "Firebase token verification failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during token verification.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal server error occurred." });
            }
        }


        [HttpPost("google-login")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var accountResponse = await _authenticationService.LoginWithEmailAsync(request.Email);
                if (accountResponse.IsError)
                {
                    return BadRequest(new { Errors = accountResponse.Errors });
                }
                return Ok(accountResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during Google login.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal server error occurred." });
            }
        }


    }
}
