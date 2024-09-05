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

namespace MoveMate.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<AuthenticationService> _logger;
        public AuthenticationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuthenticationService> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
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
            new Claim(JwtRegisteredClaimNames.Sid, accountResponse.UserId.ToString()),
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



        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

    }
}
