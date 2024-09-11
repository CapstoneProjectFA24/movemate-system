using FirebaseAdmin.Auth;
using MoveMate.Service.Commons;

namespace MoveMate.API.Middleware
{
    public interface IFirebaseMiddleware
    {
        Task<OperationResult<UserRecord>> RetrieveUser(string email);
        Task<OperationResult<UserRecord>> CreateUser(
            string username,
            string password,
            string email,         
            string phoneNumber);
    }
}
