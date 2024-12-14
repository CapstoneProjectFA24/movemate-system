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
            public const string AccountNotFound = "Account not found in the system";
            public const string RequestIdFail = "The input Id must match the request Id";
            public const string NotPermission = "User have no permission";
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
            public const string NotManager = "You do not have permission to perform this operation";
            public const string UserCannotAdd = "This user cannot be added to the group";

            //Booking
            public const string NotFoundBooking = "Booking not found";
            public const string BookingCannotPay = "Booking is not from this user";
            public const string NotFoundBookingDetail = "Booking detail not found or status not WAITING";
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
            public const string BookingConfirmed = "The booking status must be CONFIRMED";
            public const string BookingAssigned = "The booking status must be ASSIGNED";
            public const string BookingReviewOnline = "The booking is review online";
            public const string BookingReviewing = "The booking status must be REVIEWING";
            public const string ReviewerBadRequest = "Bad Request";
            public const string BookingCompleted = "The booking status must be COMPLETED";
            public const string BookingRefund = "The booking status must be REFUNDING";
            public const string BookingCancel = "Booking have been canceled";
            public const string ChangeBookingAtFail = "BookingAt cannot be changed";
            public const string RequiredId = "The input id does not match the request id";
            public const string BookingHasBeenUpdated = "Booking can only be updated once";
            public const string OnlyInscrease = "Service can only increase quantity";
            public const string CancelExpirePayment = "Expired - Automatically canceled by system";
            public const string CancelExpireBooking = "Is expired, Cancel by System";
            public const string UpdateTimeNotAllowed = "It's not time yet";
            public const string SomethingWrong = "An error has occurred, please wait patiently for the manager to handle it";
            public const string CannotUpdateBookingCloseToTime = "Cannot update BookingAt as it is less than 1 hour from now";

            public const string BookingNotEstimated = "Booking has not been updated Estimated Delivery Time yet";
            public const string EstimatedNotEnough = "EstimatedDeliveryTime of Booking was not enough!";
            public const string NoAvailableDrivers = "Currently there are no suitable drivers";
            public const string NotFoundDriver = "Not found driver";
            public const string CannotAssigned = "Drivers or porters cannot be added to the booking";
            public const string RefundFail = "Refund failure reasons cannot be added when you agree to a refund";
            public const string RefundTrueNoReasonFail = "Please provide a reason for not accepting a refund";
            public const string NotInsurance = "This booking has not purchased insurance";
            public const string PriceOver20m = "Cannot set a desired price greater than 20 million";
            public const string NotEnoughInsurance = "Insufficient amount of insurance";
            public const string NotSuitableBookingTracker = "Booking tracker is not suitable for exchange";
            public const string DamageReport = "Damage cannot be reported at this time";
            //Assignment
            public const string NotFoundAssignment = "Assignment not found";
            public const string AssignmentSuggeted = "The assignment status must be SUGGESTED";
            public const string AssignmentWaiting = "The assignment status must be WAITING";
            public const string AssignWrongReview = "Booking is not from this reviewer";
            public const string CanNotFix = "Booking detail is completely fine";
            public const string AssignmentManual = "Manual assignment Faild";
            public const string AssignmentDuplicate = "The staff member is already assigned to this booking.";
            public const string ReviewIsRequired = "What makes you unhappy about this staff?";
            public const string MonetoryFail = "Cannot be added fail reason when you agree to a solve";
            public const string MonetoryFailedReason = "Must give reason for refusal if refusing to resolve";
            public const string AssignFailed = "Automatic employee assignment failed";
            public const string AssignmentIsChanged = "This staff has been replaced";
            //House type
            public const string NotFoundHouseType = "House type not found";
            public const string AddHouseTypeFail = "Add house type setting failed";
            public const string HouseTypeUpdateFail = "Update house type failed";
            public const string HouseTypeAlreadyDeleted = "House type already deleted";

            //Noti
            public const string NotFoundNotification = "Notification not found";

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
            public const string InvalidServiceDepen = "Service Porter Depen is invalid for Porter type services or Porter type services, must have Porter supper serivce";
            public const string CanNotDeletedSuperService = "This service cannot be removed because other services depend on it";
            public const string ServiceAlreadyDeleted = "Service already deleted";
            public const string ServiceUpdateFail = "Update service failed";
            public const string NotFoundParentService = "Parent service not found";
            public const string CannotUpdateParentForTierZero = "Can not update parentServiceId for service tier 0";
            public const string AssignedLeader = "Leader have been assigned";
            public const string InvalidRequest = "Invalid request";
            public const string SuperService = "Other services depend on it";

            //FeeSetting
            public const string NotFoundFeeSetting = "Fee setting not found";
            public const string FeeSettingAlreadyDeleted = "Fee setting already deleted";
            public const string FeeTypeTruckFail = "Fee setting with Type TRUCK does not depend on house type";
            public const string FeeUnitKMFail = "Fee setting with Type TRUCK must have unit KM";
            public const string FeeUnitNotKMFail = "Unit can't be KM";
            public const string FeeUnitPercentFail = "Unit musst be PERCENT";
            public const string FeeUnitFloorFail = "Floor percent can't be null";
            public const string NotServiceFeeFail = "This type of fee needs to be associated with 1 service";
            public const string ServiceFeeFail = "This type of fee don't needs to be associated with 1 service";
            public const string ServiceTypeTruck = "Service type's must be TRUCK";
            public const string ServiceTier1 = "Cannot create fee settings for parent service";
            public const string ServiceTruckCategory = "Service does not have a truck category";


            //Truck category
            public const string NotFoundTruckCategory = "Truck category not found";
            public const string NotFoundTruckImg = "Truck image not found";
            public const string NotFoundTruck = "Truck not found";
            public const string TruckImgIsDeleted = "Truck image  has been deleted";
            public const string TruckCategoryAlreadyDeleted = "Truck category already deleted";
            public const string TruckCategoryUpdateFail = "Update truck category failed";
            public const string TruckAlreadyDeleted = "Truck already deleted";
            public const string TruckImgRequire = "Truck images cannot be empty";
            public const string NotFoundPorter = "Not found booking detail with type PORTER";


            //Schedule
            public const string NotFoundSchedule = "Schedule not found";
            public const string NotFoundScheduleWorking = "Schedule working not found";
            public const string ScheduleWorkingAlreadyDeleted = "Schedule working already deleted";
            public const string ScheduleWorkingUpdateFail = "Update truck category failed";
            public const string NotFoundScheduleBooking = "Schedule booking not found";
            public const string ScheduleBookingAlreadyDeleted = "Truck already deleted";
            public const string ScheduleBookingUpdateFail = "Update schedule booking failed";
            public const string ScheduleIsDeleted = "Schedule has been deleted";
            //Wallet
            public const string NotFoundWallet = "Wallet not found";
            public const string NotFoundWithDraw = "Withdrawal not found";
            public const string UpdateWalletBalance = "Failed to update wallet balance";
            public const string NotEnoughMoney = "Wallet balance is not enough";
            public const string WithdrawNotFromUser = "Withdrawal is not from this user";
            public const string WithdrawSuccess = "Withdrawal has been completed";
            public const string WithdrawCancel = "Withdrawal has been canceled";
            public const string TransactionCancel = "Transaction has been canceled";
            //Group
            public const string NotFoundGroup = "Not found group";
            public const string CannotAddReviewer = "Group cannot have more than 3 REVIEWER";
            public const string CannotAddStaff = "Group has reached the maximum number of these staffs";
            //Payment
            public const string BookingStatus = "Booking status must be either DEPOSITING or INPROGRESS";
            public const string AmountGreaterThanZero = "Amount must be greater than zero";
            public const string InvalidSignature = "Invalid payment signature";
            public const string CreatePaymentFail = "Payment was not successful";
            public const string ProcessPaymentFail = "Payment was not successful";
            public const string InvalidBookingId = "Invalid booking ID";
            public const string IsAtLeast24HoursApart = "Promotion at least 24 hours";
            public const string PaymentFail = "Payment was failed";
            public const string PayByCash = "You have chosen the cash payment method";
            public const string BalanceNotEnough = "The balance in the wallet is not enough";
            public const string UnspPayment = "Unsupported payment method selected";
            public const string WalletLocked = "Wallet has been locked, please update wallet information to unlock";

            //Transaction
            public const string TransactionExist = "Transaction has already been processed";

            //Tracker resource
            public const string NotFoundBookingTracker = "Booking tracker not found";
            public const string VerifyReviewOffline= "Must have image or video to verify from SUGGESTED to REVIEWED";
            public const string NotFoundTrackerSource = "Tracker source not found";
            public const string TrackerSourceIsDeleted = "Tracker source has been deleted";
            public const string TrackerSourceFail = "Can not create tracker source";

            //Assignment
            public const string AssignmentUpdateFail = "Update assignment failed";
            public const string AssignmentUpdateFailByDiffTruckCate = "Update assignment failed by driver is other truckCageID";
            public const string AssignmentUpdateFailOtherStaffType = "Update assignment failed by other StaffType";


            //Promotion
            public const string NotFoundPromotion = "Promotion not found";
            public const string PromotionAlreadyDeleted = "Promotion category already deleted";
            public const string PromotionRunOut = "Vouchers are out of stock";
            public const string LessAssigned ="Quantity cannot be less than the number of assigned vouchers";
            public const string InvalidDates = "The start date must be less than the end date";
            public const string ServiceTiers1 = "Service must be tier 1";

            //Voucher
            public const string NotFoundVoucher = "Voucher not found";
            public const string VoucherAlreadyDeleted = "Voucher already deleted";
            public const string VoucherAlreadyAssigned = "Voucher received";
            public const string VoucherHasBeenAssigned = "The user has already received the voucher";
            public const string VoucherLessThanQuantity = "The number of vouchers provided does not match the specified quantity";
            public const string VoucherUnique = "Vouchers must have unique Promotion IDs";
            public const string VoucherNotUser = "Invalid or unauthorized vouchers in request";
            public const string VoucherNotMatch = "Invalid voucher: promotion does not match booking services";

            //Validate
            public const string ValidateField = "Field is required";

            //ScheduleWorking
            public const string TimeOnlyFormat = "Invalid format";



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
            public const string DeletedUserSuccess = "User has been deleted";
            public const string DeleteUserInfo = "User info has been deleted";
            public const string CreateUserInfo = "Create a new user info successful";
            public const string UserInfoUpdateSuccess = "Update user info succesful";
            public const string UserUpdateSuccess = "Update user succesful";
            public const string GetUserSuccess = "Get user done";
            public const string GetListUserEmpty = "List user is empty!";


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
            public const string BookingDetailUpdateSuccess = "Update booking detail succesful";
            public const string FoundAvailableDrivers = "Get Driver successful";
            public const string FoundAvailablePorters = "Get Porter successful";
            public const string ResolveException = "Successful compensation";


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
            public const string GetListScheduleWorkingEmpty = "List schedule working is empty!";
            public const string GetListScheduleWorkingSuccess = "Get list schedule working done";
            public const string GetScheduleWorkingSuccess = "Get schedule working successfully";
            public const string DeleteScheduleWorking = "Schedule working has been deleted";
            public const string CreateScheduleWorking = "Create a new schedule working successful";
            public const string ScheduleWorkingUpdateSuccess = "Update schedule working succesful";
            public const string GetListScheduleDailyEmpty = "List schedule daily is empty!";
            public const string GetListScheduleDailySuccess = "Get list schedule daily done";
            public const string CreateSchedule = "Create a new schedule successful";
            public const string GetListScheduleBookingEmpty = "List schedule booking is empty!";
            public const string GetListScheduleBookingSuccess = "Get list schedule booking done";
            public const string GetScheduleBookingSuccess = "Get truck successfully";
            public const string DeleteScheduleBooking = "Schedule booking has been deleted";
            public const string CreateScheduleBookingCategory = "Create a new schedule booking successful";
            public const string ScheduleBookingUpdateSuccess = "Update schedule booking succesful";

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
            public const string BookingHasNoPorter = "Booking do not need porter";
            public const string BookingHasNoDriver = "Booking do not need driver";

            //Group
            public const string DeleteGroup = "Group has been deleted";
            public const string GetListGroupEmpty = "List group is empty!";
            public const string GetListGroupSuccess = "Get list group done";
            public const string GetGroupSuccess = "Get group successfully";
            public const string CreateGroup = "Create a new group successful";
            public const string AddUserToGroup = "Add user into group successful";
            public const string AddScheduleToGroup = "Add group into schedule successful";

            //Wallet
            public const string GetWalletSuccess = "Wallet retrieved successfully";
            public const string UpdateWalletSuccess = "Wallet updated successfully";
            public const string EnoughMoney = "Sufficient wallet balance";
            public const string WithDrawMoney = "Withdrawal request successful";
            public const string CancelWithDrawMoney = "Cancel withdrawal request successful";
            public const string WithDrawMoneySuccess = "Successful withdrawal";
            public const string GetListWithDrawalEmpty = "List withdrawal is empty!";
            public const string GetListWithDrawalSuccess = "Get list withdrawal done";

            //Payment
            public const string CreatePaymentLinkSuccess = "Payment link created successfully";
            public const string PaymentHandle = "Payment handled successfully";
            public const string VNPPayment = "Payment with VnPay";
            public const string MomoPayment = "Payment with Momo";
            public const string PaymentSuccess = "Payment booking successful";


            //Transaction
            public const string TransactionSuccess = "Transaction has already been processed";
            public const string AlreadyProcess = "Already processed";
            public const string GetListTransactionEmpty = "List transaction is empty!";
            public const string GetListTransactionSuccess = "Get list transaction done";
            public const string TranferSuccess = "You have successfully transferred money";

            //Service
            public const string CreateService = "Create a new service successful";
            public const string DeleteService = "Service has been deleted";
            public const string ServiceUpdateSuccess = "Update service succesful";
            public const string GetListServiceEmpty = "List service is empty!";
            public const string GetListServiceSuccess = "Get list service done";
            public const string GetServiceSuccess = "Get service successfully";

            //Tracker Source
            public const string DeleteTrackerSource = "Tracker source has been deleted";
            public const string AddTrackerSuccess = "Add tracker successful";
            public const string AddTrackerReport = "We have noted your report";
            public const string UpdateBookingTracker = "Update booking tracker successful";

            //HouseType
            public const string HouseTypeUpdateSuccess = "Update house type succesful";
            public const string CreateHouseType = "Create a new house type successful";

            //Assignment
            public const string UpdateAssignment = "Update assignment successful";
            public const string AssignmentManual = "Manual assignment successful";
            public const string ReviewSuccess = "Review staff successful";

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
            public const string VoucherUpdateSuccess = "Update voucher succesful";

            //Notification
            public const string CreatedUserDeviceSuccessfully = "Created User Device Successfully.";
            public const string DeletedUserDeviceSuccessfully = "Delete User Device Successfully.";
            public const string ReadNoti = "Notification have been read";
            public const string GetListNotificationEmpty = "List notification is empty!";
            public const string GetListNotificationSuccess = "Get list notification done";
            public const string GetListReportEmpty = "List report is empty!";
            public const string GetListReportSuccess = "Get list report done";

        }

        public static class ShardErrorMessage
        {
            public const string ShardRangeCannotBeNullOrEmpty = "Shard range cannot be null or empty.";
            public const string InvalidShardRangeFormat = "Invalid shard range format. Expected format: yyyy-yyyy, yyyyMM-yyyyMM, yyyyMMdd-yyyyMMdd, or a single shard.";
            public const string InvalidShardFormat = "One or both shards are in an invalid format.";
            public const string StartAndEndShardsMustBeSameType = "Start and end shards must be of the same type (yyyy, yyyyMM, or yyyyMMdd).";
            public const string InvalidYearFormat = "Invalid year format.";
            public const string InvalidMonthFormat = "Invalid month format.";
            public const string InvalidDayFormat = "Invalid day format.";
            public const string UnsupportedShardFormat = "Unsupported shard format. Supported formats: yyyy, yyyyMM, yyyyMMdd.";
            public const string ShardAndTypeCannotBeProvidedTogether = "Both Shard and Type cannot be provided together. Please specify only one.";
        }


    }
}