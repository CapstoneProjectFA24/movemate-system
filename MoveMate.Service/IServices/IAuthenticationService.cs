using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponse;
using MoveMate.Service.ViewModels.ModelResponses;
using System.Security.Claims;

namespace MoveMate.Service.IServices
{
    public interface IAuthenticationService
    {
        public Task<OperationResult<AccountResponse>> Register(CustomerToRegister customerToRegister, JWTAuth jwtAuth);
        public Task<OperationResult<object>> CheckCustomerExistsAsync(CustomerToRegister customer);
        public Task<OperationResult<AccountResponse>> RegisterV2(CustomerToRegister customerToRegister);
        public Task<OperationResult<AccountResponse>> LoginAsync(AccountRequest accountRequest, JWTAuth jwtAuth);
        public Task<OperationResult<AccountResponse>> LoginByPhoneAsync(PhoneLoginRequest request, JWTAuth jwtAuth);
        public Task<AccountResponse> GenerateTokenWithUserIdAsync(string userId, JWTAuth jwtAuthOptions);

        public Task<AccountTokenResponse> ReGenerateTokensAsync(AccountTokenRequest accountTokenRequest,
            JWTAuth jwtAuth);

        public Task<OperationResult<AccountResponse>> LoginWithEmailAsync(string email);
        public Task<OperationResult<AccountResponse>> Login(AccountRequest request, JWTAuth jwtAuth);
        public Task CreateUserDeviceAsync(CreateUserDeviceRequest userDeviceRequest, IEnumerable<Claim> claims);
        public Task DeleteUserDeviceAsync(int userDeviceId);
    }
}