using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponse;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.Service.IServices
{
    public interface IAuthenticationService
    {
        public Task<OperationResult<RegisterResponse>> Register(CustomerToRegister customerToRegister);
        public Task<AccountResponse> LoginAsync(AccountRequest accountRequest, JWTAuth jwtAuth);
        public Task<AccountResponse> GenerateTokenWithUserIdAsync(string userId, JWTAuth jwtAuthOptions);
        public Task<AccountTokenResponse> ReGenerateTokensAsync(AccountTokenRequest accountTokenRequest, JWTAuth jwtAuth);
        public Task<AccountResponse> LoginWithEmailAsync(string email);
    }
}
