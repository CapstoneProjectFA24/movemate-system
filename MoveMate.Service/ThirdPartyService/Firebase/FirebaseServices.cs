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
using Microsoft.Extensions.Configuration;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;

namespace MoveMate.Service.ThirdPartyService.Firebase
{
    public class FirebaseServices : IFirebaseServices
    {
        private static FirebaseApp? _firebaseApp;
        private readonly FirestoreDb _dbFirestore;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _producer;
        private readonly IConfiguration _config;
        private readonly IRedisService _redisService;


        public FirebaseServices(IConfiguration config, IMapper mapper, IMessageProducer producer,
            IRedisService redisService)
        {
            _mapper = mapper;
            _producer = producer;
            _redisService = redisService;
            _config = config;

            // Check if the default FirebaseApp is already created
            if (_firebaseApp == null)
            {
                string authJsonFile = _config["FirebaseSettings:ConfigFile"];
                var appOptions = new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(authJsonFile)
                };

                _firebaseApp = FirebaseApp.Create(appOptions);
            }
           

            string path = AppDomain.CurrentDomain.BaseDirectory + @"firebase_app_settings.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            _dbFirestore = FirestoreDb.Create("movemate-firebase");
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

        public async Task<string?> SaveBooking(Booking saveObj, long id, string collectionName,
            bool isRecursiveCall = false)
        {
            try
            {

                if (saveObj.IsReviewOnline ==false && saveObj.Status == BookingEnums.REVIEWING.ToString() && saveObj.Assignments.Any(a => a.StaffType == RoleEnums.REVIEWER.ToString() && a.Status == AssignmentStatusEnums.ASSIGNED.ToString()))
                {
                    if (!isRecursiveCall)
                    {
                        _producer.SendingMessage("movemate.push_to_firebase_local", saveObj.Id);
                        Console.WriteLine("Pushed to old_bookings successfully");
                    }
                }

                var save = _mapper.Map<BookingResponse>(saveObj);
                if (saveObj.Status == BookingEnums.COMING.ToString())
                {
                    Console.WriteLine("push to movemate.booking_assign_driver");

                    var keyDriverAssigned = DateUtil.GetKeyDriverBooking(saveObj.BookingAt, saveObj.Id);
                    var isDriverAssigned = await _redisService.KeyExistsAsync(keyDriverAssigned);

                    var keyPorterAssigned = DateUtil.GetKeyPorterBooking(saveObj.BookingAt, saveObj.Id);
                    var isPorterAssigned = await _redisService.KeyExistsAsync(keyPorterAssigned);

                    if (!saveObj.Assignments.Any(a => a.StaffType == RoleEnums.DRIVER.ToString()) &&
                        isDriverAssigned == false)
                    {
                        if (!isRecursiveCall)
                        {
                            //await SaveBooking(saveObj, id, "old_bookings", true);
                            _producer.SendingMessage("movemate.push_to_firebase", saveObj.Id);

                            Console.WriteLine("Pushed to old_bookings successfully");
                            _producer.SendingMessage("movemate.booking_assign_driver", saveObj.Id);
                        }
                    }
                    
                    if (saveObj.Assignments.Any(a => a.StaffType == RoleEnums.DRIVER.ToString()) &&
                        !saveObj.Assignments.Any(a => a.StaffType == RoleEnums.PORTER.ToString())
                        && isPorterAssigned == false && saveObj.IsPorter == true)
                    {
                        _producer.SendingMessage("movemate.booking_assign_porter", saveObj.Id);
                    }
                }
                
                var redisKey = saveObj.Id + '-' + saveObj.Status;

                var checkExistQueue = await _redisService.KeyExistsAsync(redisKey);
                if (checkExistQueue == false)
                {
                    _redisService.SetData(redisKey, saveObj.Id);
                    _producer.SendingMessage("movemate.notification_update_booking", saveObj.Id);
                }
                
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
        public async Task SaveSubcollection<T>(T saveObj, long parentId, string parentCollectionName,
            string subcollectionName, long subId)
        {
            try
            {
                DocumentReference parentDocRef =
                    _dbFirestore.Collection(parentCollectionName).Document(parentId.ToString());

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

        public async Task<T> GetSubcollectionByKey<T>(long parentId, string parentCollectionName,
            string subcollectionName, string subKey)
        {
            try
            {
                DocumentReference parentDocRef =
                    _dbFirestore.Collection(parentCollectionName).Document(parentId.ToString());

                DocumentReference subDocRef = parentDocRef.Collection(subcollectionName).Document(subKey);

                DocumentSnapshot snapshot = await subDocRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    return snapshot.ConvertTo<T>();
                }
                else
                {
                    throw new NotFoundException(
                        $"Document with key '{subKey}' not found in subcollection '{subcollectionName}'.");
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

        public async Task<string?> SaveMailManager(MoveMate.Domain.Models.Notification saveObj, string id,
            string collectionName)
        {
            try
            {
                var save = _mapper.Map<NotificationResponse>(saveObj); // Map to response model if needed

                // Optional: Add specific logic here if required for 'mail-manager'

                DocumentReference docRef = _dbFirestore.Collection(collectionName).Document(id.ToString());
                await docRef.SetAsync(save);

                var saved = (await docRef.GetSnapshotAsync()).UpdateTime.ToString();
                Console.WriteLine($"SaveMailManager message to firebase: {id}, time: {saved}");

                return saved;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving document to mail-manager: {e.Message}");
                throw;
            }
        }

        public async Task SendNotificationAsync(string title, string body, string fcmToken,
            Dictionary<string, string>? data = null)
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

        public async Task<List<BookingResponse>> GetAllBookings(string collectionName)
        {
            try
            {
                // Lấy toàn bộ document trong collection
                CollectionReference collectionRef = _dbFirestore.Collection(collectionName);
                QuerySnapshot snapshot = await collectionRef.GetSnapshotAsync();

                // Map dữ liệu thành danh sách BookingResponse
                var bookings = snapshot.Documents
                    .Select(doc => doc.ConvertTo<BookingResponse>())
                    .ToList();

                Console.WriteLine($"Fetched {bookings.Count} bookings from {collectionName}.");
                return bookings;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching all bookings: {e.Message}");
                throw;
            }
        }

        public async Task<BookingResponse?> GetBookingById(long id, string collectionName)
        {
            try
            {
                // Tham chiếu tới document với ID cụ thể
                DocumentReference docRef = _dbFirestore.Collection(collectionName).Document(id.ToString());
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    // Map dữ liệu thành BookingResponse
                    var booking = snapshot.ConvertTo<BookingResponse>();
                    Console.WriteLine($"Fetched booking with ID {id} from {collectionName}.");
                    return booking;
                }
                else
                {
                    Console.WriteLine($"No booking found with ID {id} in {collectionName}.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching booking by ID: {e.Message}");
                throw;
            }
        }
    }
}