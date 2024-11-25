using FirebaseAdmin.Auth;
using MoveMate.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.Models;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.Service.ThirdPartyService.Firebase
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

        public Task<string> Save<T>(T saveObj, long id, string collectionName);
        public Task<string?> SaveBooking(Booking saveObj, long id, string collectionName, bool isRecursiveCall = false);
        public Task<string?> SaveMailManager(MoveMate.Domain.Models.Notification saveObj, string id, string collectionName);
        Task SendNotificationAsync(string title, string body, string fcmToken, Dictionary<string, string>? data = null);
        public Task<T> GetByKey<T>(string key, string collectionName);
        public Task<bool> Delete(string key, string collectionName);

        public Task SaveSubcollection<T>(T saveObj, long parentId, string parentCollectionName,
            string subcollectionName,
            long subId);

        public Task<T> GetSubcollectionByKey<T>(long parentId, string parentCollectionName, string subcollectionName,
            string subKey);

        public Task<List<Booking>> GetCanceledBookingsOlderThanAsync(DateTime cutoffDate);

        public Task<List<BookingResponse>> GetAllBookings(string collectionName);
        public Task<BookingResponse?> GetBookingById(long id, string collectionName);
    }
}