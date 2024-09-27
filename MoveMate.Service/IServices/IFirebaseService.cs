using FirebaseAdmin.Auth;
using MoveMate.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IFirebaseServices
    {
        Task<UserRecord> RetrieveUser(string email);
        Task<UserRecord> CreateUser(
            string username,
            string password,
            string email,
            string phoneNumber
            );

        public Task<OperationResult<FirebaseToken>> VerifyIdTokenAsync(string idToken);

        public Task<bool> ValidateFcmToken(string token);
    }
}
