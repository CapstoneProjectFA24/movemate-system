using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MoveMate.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class FirebaseServices : IFirebaseServices
    {
        private readonly FirebaseApp _firebaseApp;
        public FirebaseServices(string authJsonFile)
        {
            var appOptions = new AppOptions()
            {
                Credential = GoogleCredential.FromFile(authJsonFile),
            };
            _firebaseApp = FirebaseApp.Create(appOptions);
        }

        public async Task<UserRecord> CreateUser(string username,
            string password,
            string email,         
            string phoneNumber
            )
        {
            var defaultAuth = FirebaseAuth.GetAuth(_firebaseApp);

            var args = new UserRecordArgs()
            {
                Email = email,
                EmailVerified = false,
                PhoneNumber = phoneNumber,
                Password = password,
                Disabled = false,
            };

            return await defaultAuth.CreateUserAsync(args);

        }

        public async Task<UserRecord> RetrieveUser(string email)
        {
            return await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
        }
    }
}
