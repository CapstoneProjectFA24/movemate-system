﻿using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;

namespace MoveMate.Service.ThirdPartyService.Firebase
{
    public class FirebaseServices : IFirebaseServices
    {
        private static FirebaseApp _firebaseApp;
        private readonly FirestoreDb _dbFirestore;

        public FirebaseServices(string authJsonFile)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"firebase_app_settings.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            _dbFirestore = FirestoreDb.Create("movemate-bb487");

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

        public async Task<string> Save<T>(T saveObj, long id, string collectionName)
        {
            try
            {
                DocumentReference docRef = _dbFirestore.Collection(collectionName).Document(id.ToString());
                await docRef.SetAsync(saveObj);
                return (await docRef.GetSnapshotAsync()).UpdateTime.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving document: {e.Message}");
                throw;
            }
        }

        public async Task<T> GetByKey<T>(string key, string collectionName)
        {
            try
            {
                DocumentReference docRef = _dbFirestore.Collection(collectionName).Document(key);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    return snapshot.ConvertTo<T>();
                }
                else
                {
                    throw new NotFoundException(
                        $"Document with key '{key}' not found in collection '{collectionName}'.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error getting document: {e.Message}");
                throw;
            }
        }

        public async Task<bool> Delete(string key, string collectionName)
        {
            try
            {
                await _dbFirestore.Collection(collectionName).Document(key).DeleteAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting document: {e.Message}");
                throw;
            }
        }
    }
}