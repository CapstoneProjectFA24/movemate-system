using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.ViewModels.ModelResponses;
using System.ComponentModel.DataAnnotations;
using FirebaseAdmin.Messaging;
using MoveMate.Repository.Repositories.UnitOfWork;

namespace MoveMate.Service.ThirdPartyService.Firebase
{
    public class FirebaseServices : IFirebaseServices
    {
        private static FirebaseApp? _firebaseApp;
        private readonly FirestoreDb _dbFirestore;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _producer;
        private readonly UnitOfWork _unitOfWork;


        public FirebaseServices(string authJsonFile, IMapper mapper, IMessageProducer producer, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _producer = producer;

            // Check if the default FirebaseApp is already created
            if (_firebaseApp == null)
            {
                var appOptions = new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(authJsonFile)
                };

                _firebaseApp = FirebaseApp.Create(appOptions);

            }

            string path = AppDomain.CurrentDomain.BaseDirectory + @"firebase_app_settings.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            _dbFirestore = FirestoreDb.Create("movemate-firebase");
            _unitOfWork = (UnitOfWork)unitOfWork;
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

        public async Task<string?> SaveBooking(Booking saveObj, long id, string collectionName)
        {
            if (saveObj.Status != BookingEnums.PENDING.ToString() && saveObj.Status != BookingEnums.ASSIGNED.ToString())
            {
                Console.WriteLine(saveObj.Id);
            }

            try
            {
                var existBooking = _unitOfWork.BookingRepository.GetByIdAsync(saveObj.Id, includeProperties: "Assignments");

                var save = _mapper.Map<BookingResponse>(existBooking);
                if (saveObj.Status == BookingEnums.COMING.ToString())
                {
                    Console.WriteLine("push to movemate.booking_assign_driver");

                    _producer.SendingMessage("movemate.booking_assign_driver", saveObj.Id);

                }

                //_producer.SendingMessage("movemate.notification_update_booking", saveObj.Id);

                DocumentReference docRef = _dbFirestore.Collection(collectionName).Document(id.ToString());
                await docRef.SetAsync(save);
                var saved = (await docRef.GetSnapshotAsync()).UpdateTime.ToString();

                Console.WriteLine($"SaveBooking message to firebase: {id}, time: {saved}");

                return saved;
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
        
        // SubCollection
        public async Task SaveSubcollection<T>(T saveObj, long parentId, string parentCollectionName, string subcollectionName, long subId)
        {
            try
            {
                DocumentReference parentDocRef = _dbFirestore.Collection(parentCollectionName).Document(parentId.ToString());

                DocumentReference subDocRef = parentDocRef.Collection(subcollectionName).Document(subId.ToString());

                await subDocRef.SetAsync(saveObj);
                Console.WriteLine($"Subcollection document saved with ID: {subId}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving subcollection document: {e.Message}");
                throw;
            }
        }
        
        public async Task<T> GetSubcollectionByKey<T>(long parentId, string parentCollectionName, string subcollectionName, string subKey)
        {
            try
            {
                DocumentReference parentDocRef = _dbFirestore.Collection(parentCollectionName).Document(parentId.ToString());

                DocumentReference subDocRef = parentDocRef.Collection(subcollectionName).Document(subKey);

                DocumentSnapshot snapshot = await subDocRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    return snapshot.ConvertTo<T>();
                }
                else
                {
                    throw new NotFoundException($"Document with key '{subKey}' not found in subcollection '{subcollectionName}'.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error getting subcollection document: {e.Message}");
                throw;
            }
        }
        public async Task<List<Booking>> GetCanceledBookingsOlderThanAsync(DateTime cutoffDate)
        {
            var canceledBookings = new List<Booking>();

            var collectionRef = _dbFirestore.Collection("bookings"); // Replace with your collection name

            // First query for CANCELED bookings
            var canceledQuery = collectionRef
                .WhereEqualTo("status", BookingEnums.CANCEL)
                .WhereLessThan("bookingAt", cutoffDate);

            var canceledSnapshots = await canceledQuery.GetSnapshotAsync();
            foreach (var doc in canceledSnapshots.Documents)
            {
                var booking = doc.ConvertTo<Booking>();
                canceledBookings.Add(booking);
            }

            // Second query for COMPLETED bookings
            var completedQuery = collectionRef
                .WhereEqualTo("status", BookingEnums.COMPLETED)
                .WhereLessThan("bookingAt", cutoffDate);

            var completedSnapshots = await completedQuery.GetSnapshotAsync();
            foreach (var doc in completedSnapshots.Documents)
            {
                var booking = doc.ConvertTo<Booking>();
                canceledBookings.Add(booking);
            }

            return canceledBookings;
        }


        public async Task SendNotificationAsync(string title, string body, string fcmToken, Dictionary<string, string>? data = null)
        {
            try
            {
                var message = new Message()
                {
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Token = fcmToken,
                    Data = data ?? new Dictionary<string, string>()
                };

                // Gửi thông báo
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"Thông báo đã được gửi thành công với ID: {response}");
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Lỗi khi gửi thông báo: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi không xác định khi gửi thông báo: {ex.Message}");
                throw;
            }
        }
    }
}