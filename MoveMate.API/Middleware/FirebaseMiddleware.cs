using FirebaseAdmin.Auth;
using MoveMate.Service.Commons;
using MoveMate.Service.ThirdPartyService.Firebase;

namespace MoveMate.API.Middleware
{
    public class FirebaseMiddleware : IFirebaseMiddleware
    {
        private readonly IFirebaseServices _firebaseServices;

        public FirebaseMiddleware(IFirebaseServices firebaseServices)
        {
            _firebaseServices = firebaseServices;
        }

        public async Task<OperationResult<UserRecord>> CreateUser(
            string username,
            string password,
            string email,
            string phoneNumber
        )
        {
            var operationResult = new OperationResult<UserRecord>();
            try
            {
                // Call the service to create a new user
                var userRecord = await _firebaseServices.CreateUser(
                    username, password, email, phoneNumber);

                // Handle success response
                operationResult.AddResponseStatusCode(
                    Service.Commons.StatusCode.Ok, "User created successfully", userRecord);
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase-specific exception
                operationResult.AddError(Service.Commons.StatusCode.FirebaseAuthError, ex.Message);
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                operationResult.AddUnknownError(ex.Message);
            }

            return operationResult;
        }

        public async Task<OperationResult<UserRecord>> RetrieveUser(string email)
        {
            var operationResult = new OperationResult<UserRecord>();
            try
            {
                // Call the service to retrieve user by email
                var userRecord = await _firebaseServices.RetrieveUser(email);

                // Handle success response
                operationResult.AddResponseStatusCode(
                    Service.Commons.StatusCode.Ok, "User retrieved successfully", userRecord);
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase-specific exception
                operationResult.AddError(Service.Commons.StatusCode.FirebaseAuthError, ex.Message);
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                operationResult.AddUnknownError(ex.Message);
            }

            return operationResult;
        }
    }
}