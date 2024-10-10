using FirebaseAdmin.Auth;
using FluentValidation;
using Google.Rpc;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoveMate.API.Middleware;
using MoveMate.API.Utils;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponse;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private IAuthenticationService _authenticationService;
        private IFirebaseServices _firebaseService;

        private IOptions<Service.ViewModels.ModelRequests.JWTAuth> _jwtAuthOptions;
        private IValidator<AccountRequest> _accountRequestValidator;
        private IValidator<PhoneLoginRequest> _phoneLoginRequestValidator;
        private IValidator<AccountTokenRequest> _accountTokenRequestValidator;

        private readonly ILogger<ExceptionMiddleware> _logger;

        // private IValidator<ResetPasswordRequest> _resetPasswordValidator;
        public AuthenticationController(IAuthenticationService authenticationService, IOptions<JWTAuth> jwtAuthOptions,
                IValidator<AccountRequest> accountRequestValidator,
                IValidator<AccountTokenRequest> accountTokenRequestValidator,
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




        /// <summary>
        /// FEATURE : Login to access the system using either email or phone number.
        /// </summary>
        /// <param name="loginRequest">
        /// LoginRequest object contains EmailOrPhone property and Password property. 
        /// Notice that the password must be hashed with MD5 algorithm before sending to Login API.
        /// </param>
        /// <returns>
        /// An Object with a JSON format that contains Account Id, Email/Phone, Role name, and a pair token (access token, refresh token).
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     POST 
        ///     {
        ///         "emailOrPhone": "admin@gmail.com", // Or "0123456789"
        ///         "password": "1"
        ///     }
        /// </remarks>
        /// <response code="200">Login Successfully.</response>
        /// <response code="400">Some error about request data and logic data.</response>
        /// <response code="404">Some error about request data not found.</response>
        /// <response code="500">Some error about the system.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] AccountRequest loginRequest)
        {
            var result = await _authenticationService.Login(loginRequest, _jwtAuthOptions.Value);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return Ok(result);
        }


        #region Re-GenerateTokens API

        /// <summary>
        /// FEATURE : Re-generate pair token from the old pair token.
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
        [HttpPost("re-generate-token")]
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

            var accountTokenResponse =
                await _authenticationService.ReGenerateTokensAsync(accountToken, _jwtAuthOptions.Value);
            return Ok(accountTokenResponse);
        }

        #endregion

        #region Register API

        /// <summary>
        /// FEATURE : Register a new account in the system.
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
        [HttpPost("registeration")]
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


        /// <summary>
        /// TEST : Register a new account in the system.
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
        public async Task<IActionResult> Register([FromBody] CustomerToRegister customerToRegister)
        {

            var response = await _authenticationService.RegisterV2(customerToRegister);

            if (response.IsError)
            {
                return HandleErrorResponse(response.Errors);
            }

            // Format response similar to your desired output structure
            return Ok(response);

        }


        


            /// <summary>
            /// FEATURE : Check Customer Exists
            /// </summary>
            /// <param name="checkCustomer">Check information of customer</param>
            /// <returns>Validate information customer are available</returns>
            /// <remarks>
            /// Sample request:
            ///     POST 
            ///     {
            ///         "email": "user@example.com",
            ///         "phone": "string",
            ///         "name": "string",
            ///         "password": "string"
            ///     }
            /// </remarks>
            /// <response code="200">Customer information is available.</response>
            /// <response code="400">Email already exists.</response>
            /// <response code="400">Phone already exists.</response>
            /// <response code="500">An unexpected error occurred.</response>
            [HttpPost("check-exists")]
        public async Task<IActionResult> CheckCustomerExists([FromBody] CustomerToRegister customer)
        {
            var result = await _authenticationService.CheckCustomerExistsAsync(customer);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return Ok(result);
        }





        /// <summary>
        /// FEATURE : Verify token from phone authen firebase
        /// </summary>
        /// <param name="checkToken">Check token from firebase</param>
        /// <returns>Validate token are available</returns>
        /// <remarks>
        /// Sample request:
        ///     POST 
        ///     {
        ///         "idToken": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Token verification successful</response>
        /// <response code="400">Something went wrong</response>
        /// <response code="500">An unexpected error occurred</response>
        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyTokenV2([FromBody] TokenRequest tokenRequest)
        {
            var result = new OperationResult<object>();

            // Call the VerifyIdTokenAsync method
            var verifyResult = await _firebaseService.VerifyIdTokenAsync(tokenRequest.IdToken);

            if (verifyResult.IsError)
            {
                // If there are errors, handle the error response
                return HandleErrorResponse(verifyResult.Errors);
            }

            // If token verification is successful, create a success response
            result.AddResponseStatusCode(Service.Commons.StatusCode.Ok, "Token verification successful", new
            {
                isValid = true,
                uid = verifyResult.Payload.Uid
            });

            return Ok(result);
        }


        /// <summary>
        /// FEATURE : Login Google 
        /// </summary>
        /// <param name="loginGoogle">User login by Google Gmail</param>
        /// <returns>User access to the system by Google </returns>
        /// <remarks>
        /// Sample request:
        ///     POST 
        ///     {
        ///         "idToken": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Token verified and JWT generated successfully</response>
        /// <respomse code="404">User not found</respomse>
        /// <response code="500">An internal server error occurred</response>
        [HttpPost("google-login")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var result = new OperationResult<AccountResponse>();
         
                var loginResult = await _authenticationService.LoginWithEmailAsync(request.Email);
                if (loginResult.IsError)
                {
                    return HandleErrorResponse(loginResult.Errors);
                }
                return Ok(loginResult);
        }


        /// <summary>
        /// TEST : Validate firebase cloud message token
        /// </summary>
        /// <param name="validateFCMTken">Check token form firebase cloud message</param>
        /// <returns>Save information devide ID</returns>
        /// <remarks>
        /// Sample request:
        ///     POST 
        ///     {
        ///         "idToken": "string"
        ///     }
        /// </remarks>        
        /// <respomse code="400">Token cannnot be empty</respomse>
        [HttpPost("validate-fcm-token")]
        public async Task<IActionResult> ValidateFcmToken([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token cannnot be empty"); 
            }

            // Perform token validation but do not return any response
            await _firebaseService.ValidateFcmToken(token);

          
            return NoContent();
        }
    }
}