using MoveMate.Domain.Models;

namespace MoveMate.Service.Commons
{
    public static class MessageConstant
    {
        public static class CommonMessage
        {
            public const string NotExistEmail = "Email does not exist in the system.";
            public const string AlreadyExistEmail = "Email already exists in the system.";
            public const string AlreadyExistCitizenNumber = "Citizen number already exists in the system.";
            public const string InvalidKitchenCenterId = "Kitchen center id is not suitable id in the system.";
            public const string InvalidBrandId = "Brand id is not suitable id in the system.";
            public const string InvalidStoreId = "Store id is not suitable id in the system.";
            public const string InvalidCategoryId = "Category id is not suitable id in the system.";
            public const string InvalidBankingAccountId = "Banking account id is not suitable id in the system.";
            public const string InvalidCashierId = "Cashier id is not suitable id in the system.";
            public const string InvalidProductId = "Product id is not suitable id in the system.";
            public const string NotExistKitchenCenterId = "Kitchen center id does not exist in the system.";
            public const string NotExistKitchenCenter = "Kitchen center does not exist in the system.";
            public const string NotExistBrandId = "Brand id does not exist in the system.";
            public const string NotExistStoreId = "Store id does not exist in the system.";
            public const string NotExistCategoryId = "Category id does not exist in the system.";
            public const string NotExistBankingAccountId = "Banking account id does not exist in the system.";
            public const string NotExistProductId = "Product id does not exist in the system.";
            public const string NotExistOrderPartnerId = "Order parnter id does not exist in the system.";
            public const string NotExistCashierId = "Cashier id does not exist in the system.";
            public const string InvalidItemsPerPage = "Items per page number is required more than 0.";
            public const string InvalidCurrentPage = "Current page number is required more than 0.";
            public const string NotExistPartnerId = "Partner id does not exist in the system.";
            public const string NotExistAccountId = "Account id does not exist in the system.";
            public const string InvalidPartnerId = "Partner id is not suitable id in the system.";
            public const string CategoryIdNotBelongToBrand = "Category id does not belong to your brand.";
            public const string CategoryIdNotBelongToStore = "Category id does not belong to your store.";
            public const string AlreadyExistPartnerProduct = "Mapping product already exists in the system.";
            public const string NotExistPartnerProduct = "Mapping product does not exist in the system.";
            public const string UserDeviceIdNotExist = "User device id does not exist in the system.";
        }

        public static class LoginMessage
        {
            public const string DisabledAccount = "Account has been disabled.";
            public const string InvalidEmailOrPassword = "Email or Password is invalid.";
        }

        public static class AccountMessage
        {
            public const string AccountIdNotBelongYourAccount = "Account id does not belong to your account.";
            public const string AccountNoLongerActive = "Your account is no longer active.";
        }


        public static class ReGenerationMessage
        {
            public const string InvalidAccessToken = "Access token is invalid.";
            public const string NotExpiredAccessToken = "Access token has not yet expired.";

            public const string NotExistAuthenticationToken =
                "You do not have the authentication tokens in the system.";

            public const string NotExistRefreshToken = "Refresh token does not exist in the system.";
            public const string NotMatchAccessToken = "Your access token does not match the registered access token.";
            public const string ExpiredRefreshToken = "Refresh token expired.";
        }


        public static class FailMessage
        {

            public const string ServerError = "An unexpected error occurred";
            public const string TokenEmpty = "Token cannnot be empty";
            public const string ReturnUrl = "Return URL is required";
            public const string PaymentMethod = "Payment method is required and must be a valid value";
            public const string Callback = "Invalid callback data";
            public const string UnsupportPayment = "Unsupported payment method selected";

            //User
            public const string UserIdInvalid = "Invalid user ID in token";


            public const string GetListUserFail = "List user is empty!";
            public const string NotFoundUser = "User not found!";
            public const string NotFoundUserInfo = "User info not found";
            public const string UpdateUserFail = "Update user failed";
            public const string LoginByPhoneFail = "The phone number does not exist";
            public const string LoginByEmailFail = "The email or phone number does not exist";
            public const string PasswordFail = "Invalid email/phone number or password";
            public const string EmailExist = "Email is already registered";
            public const string PhoneExist = "Phone number is already registered";

            //Booking
            public const string NotFoundBooking = "Booking not found";
            public const string NotFoundBookingDetail = "Booking detail not found";
            public const string IsValidBookingAt = "BookingAt is not null and whether the value is greater than or equal to the current time";
            public const string RegisterBookingFail = "Add booking failed";
            public const string CanNotUpdateStatus = "Cannot update to the next status from the current status";
            public const string BookingIdInputFail = "Booking ID is required and must be greater than 0";
            public const string BookingUpdateFail = "Update booking failed";
            public const string InvalidStatus = "Invalid status provided or cannot transition from the current status";


            //House type
            public const string NotFoundHouseType = "House type not found";
            public const string AddHouseTypeFail = "Add house type setting failed";

            //Service
            public const string NotFoundService = "Service not found";

            //FeeSetting
            public const string NotFoundFeeSetting = "Fee setting not found";

            //Truck category
            public const string NotFoundTruckCategory = "Truck category not found";

            //Schedule
            public const string NotFoundSchedule = "Schedule not found";

            //Wallet
            public const string NotFoundWallet = "Wallet not found";
        }


        public static class SuccessMessage
        {

            public const string LoginSuccess = "Login successful";
            public const string VerifyToken = "Token verification successful";

            public const string CheckUserInfo = "Customer information is available";
            public const string RegisterSuccess = "User registered successfully";
            public const string GetListUserSuccess = "Get list user done";
            public const string UserInformationRetrieved = "User information retrieved successfully";

            //Booking
            public const string GetListBookingEmpty = "List booking is empty!";
            public const string GetListBookingSuccess = "Get list booking done";
            public const string GetBookingIdSuccess = "Get booking successfully";
            public const string RegisterBookingSuccess = "Add booking successed";
            public const string ValuationBooking = "Valuation!";
            public const string CancelBooking = "Cancel booking successed";
            public const string UpdateStatusSuccess = "Status updated successfully";
            public const string UserConfirm = "Confirm round trip successfully";
            public const string BookingUpdateSuccess = "Update booking succesful";

            //FeeSetting
            public const string GetListFeeSettingEmpty = "List fee setting is empty!";
            public const string GetListFeeSettingSuccess = "Get list fee setting done";


            //House Type
            public const string GetListHouseTypeEmpty = "List house type is empty!";
            public const string GetListHouseTypeSuccess = "Get list house type done";
            public const string GetHouseTypeIdSuccess = "Get house type successfully";
            public const string AddHouseTypeSettingSuccess = "Add house type setting successed";

            //Schedule
            public const string GetListScheduleEmpty = "List schedule is empty!";
            public const string GetListScheduleSuccess = "Get list schedule done";
            public const string GetScheduleSuccess = "Get schedule successfully";

            //Schedule
            public const string GetListServiceEmpty = "List service is empty!";
            public const string GetListServiceSuccess = "Get list service done";
            public const string GetServiceSuccess = "Get service successfully";

            //Truck
            public const string GetListTruckEmpty = "List truck is empty!";
            public const string GetListTruckSuccess = "Get list truck done";
            public const string GetTruckSuccess = "Get truck successfully";

            //Wallet
            public const string GetWalletSuccess = "Wallet retrieved successfully";

        }



    }
}