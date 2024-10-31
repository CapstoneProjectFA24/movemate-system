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
            public const string ServerUrl = "Server URL is not available";
            public const string UserInfoIsDeleted = "User info has been deleted";
            public const string UserInfoExist = "User info with this type has existed";
            public const string AccountNotLogin = "This account has been banned";

            //User
            public const string UserIdInvalid = "Invalid user ID in token";
            public const string GetListUserFail = "List user is empty!";
            public const string GetListUserInfoFail = "List user info is empty!";
            public const string NotFoundUser = "User not found!";
            public const string NotFoundUserInfo = "User info not found";
            public const string UpdateUserFail = "Update user failed";
            public const string LoginByPhoneFail = "The phone number does not exist";
            public const string LoginByEmailFail = "The email or phone number does not exist";
            public const string PasswordFail = "Invalid email/phone number or password";
            public const string EmailExist = "Email is already registered";
            public const string PhoneExist = "Phone number is already registered";
            public const string RoleNotFound = "Role not found";
            public const string UserInfoUpdateFail = "Update user info failed";
            public const string UserNotDriver = "User is not a driver";
            public const string UserHaveTruck = "The user has already registered a truck";

            //Booking
            public const string NotFoundBooking = "Booking not found";
            public const string BookingCannotPay = "Booking is not from this user";
            public const string NotFoundBookingDetail = "Booking detail not found";
            public const string IsValidTimeGreaterNow = "Time is not null and whether the value is greater than or equal to the current time";
            public const string RegisterBookingFail = "Add booking failed";
            public const string CanNotUpdateStatus = "Cannot update to the next status from the current status";
            public const string BookingIdInputFail = "Booking ID is required and must be greater than 0";
            public const string BookingUpdateFail = "Update booking failed";
            public const string InvalidStatus = "Invalid status provided or cannot transition from the current status";
            public const string InvalidBookingDetails = "Invalid booking details list, must contain at least 1 element";
            public const string InvalidBookingDetailDifferent = "Invalid booking details list, truck Category Id in request is different from truck CategoryId in services";
            public const string BookingReviewed = "The booking status must be REVIEWED";
            public const string BookingWaiting = "The booking status must be WAITING";
            public const string BookingAssigned = "The booking status must be ASSIGNED";
            public const string BookingReviewOnline = "The booking is review online";
            public const string BookingReviewing = "The booking status must be REVIEWING";

            //Assignment
            public const string NotFoundAssignment = "Assignment not found";
            public const string AssignmentSuggeted = "The assignment status must be SUGGESTED";

            //House type
            public const string NotFoundHouseType = "House type not found";
            public const string AddHouseTypeFail = "Add house type setting failed";
            public const string HouseTypeUpdateFail = "Update house type failed";
            public const string HouseTypeAlreadyDeleted = "House type already deleted";

            //Service
            public const string NotFoundService = "Service not found";
            public const string TypeTruckRequire = "Type must be TRUCK when TruckCategoryId is provided";
            public const string TruckCategoryRequire = "TruckCategoryId is required when Type is TRUCK";
            public const string ParentServiceIdTier1 = "A valid ParentServiceId is required for Tier 1 services";
            public const string ParentServiceIdTier0 = "The specified ParentServiceId must refer to a Tier 0 service";
            public const string SynchronizeType = "The Type of the service must match the Type of its ParentService";
            public const string ServiceExisted = "Service has been existed";
            public const string InverseParentServiceType = "Each inverseParentService item must have the same Type as the main service";
            public const string InvalidServiceTier = "Service Tier is invalid for Truck type services or Porter type services";
            public const string ServiceAlreadyDeleted = "Service already deleted";
            public const string ServiceUpdateFail = "Update service failed";
            public const string NotFoundParentService = "Parent service not found";
            public const string CannotUpdateParentForTierZero = "Can not update parentServiceId for service tier 0)";
            public const string AssignedLeader = "Leader have been assigned";


            //FeeSetting
            public const string NotFoundFeeSetting = "Fee setting not found";
            public const string FeeSettingAlreadyDeleted = "Fee setting already deleted";

            //Truck category
            public const string NotFoundTruckCategory = "Truck category not found";
            public const string NotFoundTruckImg = "Truck image not found";
            public const string NotFoundTruck = "Truck not found";
            public const string TruckImgIsDeleted = "Truck image  has been deleted";
            public const string TruckCategoryAlreadyDeleted = "Truck category already deleted";
            public const string TruckCategoryUpdateFail = "Update truck category failed";
            public const string TruckAlreadyDeleted = "Truck already deleted";


            //Schedule
            public const string NotFoundSchedule = "Schedule not found";

            //Wallet
            public const string NotFoundWallet = "Wallet not found";
            public const string UpdateWalletBalance = "Failed to update wallet balance";


            //Payment
            public const string BookingStatus = "Booking status must be either DEPOSITING or COMPLETED";
            public const string AmountGreaterThanZero = "Amount must be greater than zero";
            public const string InvalidSignature = "Invalid payment signature";
            public const string CreatePaymentFail = "Payment was not successful";
            public const string ProcessPaymentFail = "Payment was not successful";
            public const string InvalidBookingId = "Invalid booking ID";


            //Transaction
            public const string TransactionExist = "Transaction has already been processed";

            //Tracker resource
            public const string NotFoundBookingTracker = "Booking tracker not found";
            public const string VerifyReviewOffline= "Must have image or video to verify from SUGGESTED to REVIEWED";
            public const string NotFoundTrackerSource = "Tracker source not found";
            public const string TrackerSourceIsDeleted = "Tracker source has been deleted";

            //Assignment
            public const string AssignmentUpdateFail = "Update assignment failed";

            //Promotion
            public const string NotFoundPromotion = "Promotion not found";
            public const string PromotionAlreadyDeleted = "Promotion category already deleted";
            public const string PromotionRunOut = "Vouchers are out of stock";

            //Voucher
            public const string NotFoundVoucher = "Voucher not found";
            public const string VoucherAlreadyDeleted = "Voucher already deleted";
            public const string VoucherAlreadyAssigned = "Voucher received";


        }


        public static class SuccessMessage
        {

            public const string LoginSuccess = "Login successful";
            public const string VerifyToken = "Token verification successful";
            public const string Success = "Success";
            public const string CheckUserInfo = "Customer information is available";
            public const string RegisterSuccess = "User registered successfully";
            public const string GetListUserSuccess = "Get list user done";
            public const string GetListUserInfoSuccess = "Get list user info done";
            public const string UserInformationRetrieved = "User information retrieved successfully";
            public const string CreateUser = "Create user successful";
            public const string BanUserSuccess = "User has been banned";
            public const string DeleteUserInfo = "User info has been deleted";
            public const string CreateUserInfo = "Create a new user info successful";
            public const string UserInfoUpdateSuccess = "Update user info succesful";
            public const string GetUserSuccess = "Get user done";
            

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
            public const string GetFeeSettingSuccess = "Get truck category successfully";
            public const string DeleteFeeSetiing = "Fee setting has been deleted";


            //House Type
            public const string GetListHouseTypeEmpty = "List house type is empty!";
            public const string GetListHouseTypeSuccess = "Get list house type done";
            public const string GetHouseTypeIdSuccess = "Get house type successfully";
            public const string AddHouseTypeSettingSuccess = "Add house type setting successed";
            public const string DeleteHouseType = "House type has been deleted";

            //Schedule
            public const string GetListScheduleEmpty = "List schedule is empty!";
            public const string GetListScheduleSuccess = "Get list schedule done";
            public const string GetScheduleSuccess = "Get schedule successfully";

            

            //Truck
            public const string GetListTruckEmpty = "List truck is empty!";
            public const string GetListTruckSuccess = "Get list truck done";
            public const string GetTruckSuccess = "Get truck successfully";
            public const string DeleteTruckImg = "Truck image has been deleted";
            public const string CreateTruckImg = "Create a new truck image successful";
            public const string DeleteTruckCategory = "Truck category has been deleted";
            public const string CreateTruckCategory = "Create a new truck category successful";
            public const string TruckCategoryUpdateSuccess = "Update truck category succesful";
            public const string GetListTruckCategoryEmpty = "List truck is empty!";
            public const string GetListTruckCategorySuccess = "Get list truck done";
            public const string GetTruckCategorySuccess = "Get truck category successfully";
            public const string DeleteTruck = "Truck has been deleted";
            public const string TruckUpdateSuccess = "Update truck succesful";


            //Wallet
            public const string GetWalletSuccess = "Wallet retrieved successfully";
            public const string UpdateWalletSuccess = "Wallet updated successfully";
            

            //Payment
            public const string CreatePaymentLinkSuccess = "Payment link created successfully";
            public const string PaymentHandle = "Payment handled successfully";
            public const string VNPPayment = "Payment with VnPay";
            public const string MomoPayment = "Payment with Momo";


            //Transaction
            public const string TransactionSuccess = "Transaction has already been processed";
            public const string AlreadyProcess = "Already processed";
            public const string GetListTransactionEmpty = "List transaction is empty!";
            public const string GetListTransactionSuccess = "Get list transaction done";

            //Service
            public const string CreateService = "Create a new service successful";
            public const string DeleteService = "Service has been deleted";
            public const string ServiceUpdateSuccess = "Update service succesful";
            public const string GetListServiceEmpty = "List service is empty!";
            public const string GetListServiceSuccess = "Get list service done";
            public const string GetServiceSuccess = "Get service successfully";

            //Tracker Source
            public const string DeleteTrackerSource = "Tracker source has been deleted";

            //HouseType
            public const string HouseTypeUpdateSuccess = "Update house type succesful";
            public const string CreateHouseType = "Create a new house type successful";

            //Assignment
            public const string UpdateAssignment = "Update assignment successful";
            //Promotion
            public const string GetListPromotionEmpty = "List promotion is empty!";
            public const string GetListPromotionSuccess = "Get list promotion done";
            public const string DeletePromotionCategory = "Promotion category has been deleted";
            public const string GetPromotionCategorySuccess = "Get promotion category successfully";
            public const string CreatePromotion = "Create a new promotion successful";
            public const string PromotionUpdateSuccess = "Update promotion category succesful";

            //Voucher
            public const string GetListVoucherEmpty = "List voucher is empty!";
            public const string GetListVoucherSuccess = "Get list voucher done";
            public const string CreateVoucher = "Create a new voucher successful";
            public const string DeleteVoucher = "Voucher has been deleted";
            public const string GetVoucherSuccess = "Get voucher successfully";
            public const string AssignVoucherToUserSuccess = "Get voucher successful";
        }



    }
}