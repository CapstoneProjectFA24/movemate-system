﻿using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.Commons;

namespace MoveMate.Service.ThirdPartyService.Firebase
{
    public class FirebaseServices : IFirebaseServices
    {
        private static FirebaseApp _firebaseApp;

        public FirebaseServices(string authJsonFile)
        {
            // Check if the default FirebaseApp is already created
            if (_firebaseApp == null)
            {
                var appOptions = new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(authJsonFile)
                };

                _firebaseApp = FirebaseApp.Create(appOptions);
            }
        }


        // Verify the ID token sent from the client
        public async Task<OperationResult<FirebaseToken>> VerifyIdTokenAsync(string idToken)
        {
            var result = new OperationResult<FirebaseToken>();

            try
            {
                FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                FirebaseToken decodedToken = await auth.VerifyIdTokenAsync(idToken);
                result.AddResponseStatusCode(StatusCode.Ok, "Token verified successfully", decodedToken);
            }
            catch (FirebaseAuthException ex)
            {
                result.AddError(StatusCode.BadRequest, "Something went wrong");
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, "An internal server error occurred");
            }

            return result;
        }


        public async Task<UserRecord> CreateUser(string username, string password, string email, string phoneNumber)
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
            return await FirebaseAuth.GetAuth(_firebaseApp).GetUserByEmailAsync(email);
        }

        public async Task<bool> ValidateFcmToken(string token)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                return decodedToken != null;
            }
            catch (FirebaseAuthException ex)
            {
                return false;
            }
        }
    }

}