using AutoMapper;

using Microsoft.IdentityModel.Tokens;


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Logging;
using MoveMate.Service.IServices;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ViewModels.ModelResponse;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.Exceptions;
using MoveMate.Service.Commons;
using MoveMate.Service.Utils;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MoveMate.Domain.Models;
using MoveMate.Service.ViewModels.ModelResponses;
using Microsoft.Extensions.Options;

namespace MoveMate.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly JWTAuth _jwtAuthOptions;
        private readonly ILogger<AuthenticationService> _logger;
        public AuthenticationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuthenticationService> logger, IOptions<JWTAuth> jwtAuthOptions)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
            _jwtAuthOptions = jwtAuthOptions.Value;
        }

        public async Task<AccountResponse> LoginAsync(AccountRequest accountRequest, JWTAuth jwtAuth)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetUserAsync(accountRequest.Email);
                if (user == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistEmail);
                }
          
                if (!user.Password.Equals(accountRequest.Password))
                {
                    throw new BadRequestException(MessageConstant.LoginMessage.InvalidEmailOrPassword);
                }

                var accountResponse = _mapper.Map<AccountResponse>(user);
                accountResponse = await GenerateTokenAsync(accountResponse, jwtAuth);
                return accountResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                throw;
            }
        }

        public async Task<AccountTokenResponse> ReGenerateTokensAsync(AccountTokenRequest accountTokenRequest, JWTAuth jwtAuth)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(jwtAuth.Key);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            var tokenVerification = jwtTokenHandler.ValidateToken(accountTokenRequest.AccessToken, tokenValidationParameters, out var validatedToken);
            if (validatedToken is JwtSecurityToken jwtSecurityToken && !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new BadRequestException(MessageConstant.ReGenerationMessage.InvalidAccessToken);
            }

            var utcExpiredDate = long.Parse(tokenVerification.Claims.First(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiredDate = DateUtil.ConvertUnixTimeToDateTime(utcExpiredDate);
            if (expiredDate > DateTime.UtcNow)
            {
                throw new BadRequestException(MessageConstant.ReGenerationMessage.NotExpiredAccessToken);
            }

            return new AccountTokenResponse
            {
                AccessToken = jwtTokenHandler.WriteToken(jwtTokenHandler.CreateToken(new SecurityTokenDescriptor
                {
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512),
                    Subject = new ClaimsIdentity(tokenVerification.Claims)
                })),
                RefreshToken = GenerateRefreshToken()
            };
        }



        public async Task<AccountResponse> GenerateTokenAsync(AccountResponse accountResponse, JWTAuth jwtAuth)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuth.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, accountResponse.Email),
            new Claim(JwtRegisteredClaimNames.Email, accountResponse.Email),
            new Claim(JwtRegisteredClaimNames.Sid, accountResponse.Id.ToString()),
            new Claim(ClaimTypes.Role, accountResponse.RoleId.ToString()), // Use ClaimTypes.Role for roles
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            accountResponse.Tokens = new AccountTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return accountResponse;
        }


        public async Task<OperationResult<RegisterResponse>> Register(CustomerToRegister customerToRegister)
        {
            var result = new OperationResult<RegisterResponse>();

            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetUserAsync(customerToRegister.Email);
                if (existingUser != null)
                {
                    result.AddResponseStatusCode(StatusCode.BadRequest, "Email is already registered.", null);
                    return result;
                }
                var existingUserByPhone = await _unitOfWork.UserRepository.GetUserByPhoneAsync(customerToRegister.Phone);
                if (existingUserByPhone != null)
                {
                    result.AddResponseStatusCode(StatusCode.BadRequest, "Phone number is already registered.", null);
                    return result;
                }


                var newUser = new User
                {
                    Email = customerToRegister.Email,
                    Password = customerToRegister.Password,
                    Name = customerToRegister.Name,
                    Phone = customerToRegister.Phone,
                    RoleId = 3
                };

                await _unitOfWork.UserRepository.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                var userResponse = _mapper.Map<RegisterResponse>(newUser);

                result.AddResponseStatusCode(StatusCode.Ok, "User registered successfully.", userResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration.");
                result.AddResponseStatusCode(StatusCode.ServerError, "An internal error occurred during registration.", null);
                return result;
            }
        }

        public async Task<OperationResult<AccountResponse>> RegisterV2(CustomerToRegister customerToRegister)
        {
            var result = new OperationResult<AccountResponse>();

            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetUserAsync(customerToRegister.Email);
                if (existingUser != null)
                {
                    result.AddResponseStatusCode(StatusCode.BadRequest, "Email is already registered.", null);
                    return result;
                }

                var newUser = new User
                {
                    Email = customerToRegister.Email,
                    Phone = customerToRegister.Phone,
                    Name = customerToRegister.Name,
                    Password = customerToRegister.Password,
                    RoleId = 3 // or set to the appropriate role
                };

                await _unitOfWork.UserRepository.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                var userResponse = _mapper.Map<AccountResponse>(newUser);
                result.AddResponseStatusCode(StatusCode.Ok, "User registered successfully.", userResponse);

                // Generate token for the newly registered user
                var tokenResponse = await GenerateTokenAsync(userResponse, _jwtAuthOptions);
                userResponse.Tokens = tokenResponse.Tokens; // Use the correct property

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration.");
                result.AddResponseStatusCode(StatusCode.ServerError, "An internal error occurred during registration.", null);
                return result;
            }
        }
        private string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            StringBuilder password = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[1];
                while (password.Length < length)
                {
                    rng.GetBytes(randomBytes);
                    char randomChar = (char)randomBytes[0];
                    if (validChars.Contains(randomChar))
                    {
                        password.Append(randomChar);
                    }
                }
            }
            return password.ToString();
        }


        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public async Task<AccountResponse> GenerateTokenWithUserIdAsync(string userId, JWTAuth jwtAuthOptions)
        {  
            var jwtTokenHandler = new JwtSecurityTokenHandler(); 
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, userId), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
        }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            var accountResponse = new AccountResponse
            {
                Tokens = new AccountTokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };

            return accountResponse;
        }

        public async Task<AccountResponse> LoginWithEmailAsync(string email)
        {
            // Check if the email exists in the system
            var user = await _unitOfWork.UserRepository.FindByEmailAsync(email);
            if (user == null)
            {
                return new AccountResponse
                {
                    IsError = true,
                    Errors = new List<Error>
                    {
                        new Error
                        {
                            Code = StatusCode.NotFound,
                            Message = "User not found"
                        }
                    }
                };
            }

            // Generate JWT token for the user
            var token = await GenerateJwtTokenAsync(user, _jwtAuthOptions.Key);
            return new AccountResponse
            {
                Tokens = new AccountTokenResponse
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken
                }
            };
        }


        private async Task<AccountTokenResponse> GenerateJwtTokenAsync(User user, string jwtAuthKey)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString()), // Use ClaimTypes.Role for roles
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            return new AccountTokenResponse
            {
                AccessToken = jwtTokenHandler.WriteToken(token),
                RefreshToken = GenerateRefreshToken()
            };
        }

    }
}
