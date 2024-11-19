using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging.Abstractions;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Service.Exceptions;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoveMate.Service.ViewModels.ModelRequests.Booking;
using Catel.Collections;
using Newtonsoft.Json;
using static Grpc.Core.Metadata;
using Microsoft.IdentityModel.Tokens;
using Parlot.Fluent;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.ThirdPartyService.GoongMap;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;
using Sprache;

namespace MoveMate.Service.Services
{
    public class BookingServices : IBookingServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<BookingServices> _logger;
        private readonly IMessageProducer _producer;
        private readonly IFirebaseServices _firebaseServices;
        private readonly IGoogleMapsService _googleMapsService;
        private readonly IEmailService _emailService;

        public BookingServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookingServices> logger,
            IMessageProducer producer, IFirebaseServices firebaseServices, IGoogleMapsService googleMapsService, IEmailService emailService)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
            _producer = producer;
            _firebaseServices = firebaseServices;
            _googleMapsService = googleMapsService;
            _emailService = emailService;
        }

        // FEATURE    
        // GET ALL

        #region FEATURE: GetAll booking in the system.

        public async Task<OperationResult<List<BookingResponse>>> GetAll(GetAllBookingRequest request)
        {
            var result = new OperationResult<List<BookingResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.BookingRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder(),
                    includeProperties: "BookingDetails.Service,FeeDetails,BookingTrackers.TrackerSources,Assignments"
                );
                var listResponse = _mapper.Map<List<BookingResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListBookingEmpty,
                        listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListBookingSuccess,
                    listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        #endregion

        // GET BY ID

        #region FEATURE: GetById a booking in the system.

        public async Task<OperationResult<BookingResponse>> GetById(int id)
        {
            var result = new OperationResult<BookingResponse>();
            try
            {
                var booking =
                    await _unitOfWork.BookingRepository.GetByIdAsync(id,
                        includeProperties: "BookingTrackers.TrackerSources,BookingDetails,FeeDetails,Assignments");

                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }
                else
                {
                    var productResponse = _mapper.Map<BookingResponse>(booking);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetBookingIdSuccess,
                        productResponse);
                }

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        #endregion

        // REGISTER

        #region FEATURE: Register a new booking in the system.

        public async Task<OperationResult<BookingResponse>> RegisterBooking(BookingRegisterRequest request,
            string userId)
        {
            var result = new OperationResult<BookingResponse>();
            string status = BookingEnums.PENDING.ToString();

            if (!request.IsBookingAtValid())
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.IsValidTimeGreaterNow);
                return result;
            }

            if (!request.IsBookingDetailsValid())
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidBookingDetails);
                return result;
            }

            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
                return result;
            }

            try
            {
                var existingHouseType =
                    await _unitOfWork.HouseTypeRepository.GetByIdAsyncV1(request.HouseTypeId);

                // check houseType
                if (existingHouseType == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundHouseType);
                    return result;
                }

                // setting var
                var bookingDetails = new List<BookingDetail>();
                var feeDetails = new List<FeeDetail>();
                double total = 0;

                // mapping dto to entity
                var entity = _mapper.Map<Booking>(request);

                // logic services and fee set amount
                var (totalServices, listBookingDetails, driverNumber, porterNumber, feeServiceDetails) =
                    await CalculateServiceFees(request.BookingDetails,
                        request.HouseTypeId,
                        request.TruckCategoryId, request.FloorsNumber, request.EstimatedDistance);

                total += totalServices;
                //feeDetails.AddRange(feeServiceDetails);
                bookingDetails.AddRange(listBookingDetails);

                // list lên fee common
                var dateBooking = entity.BookingAt ?? DateTime.Now;
                var (totalFee, feeCommonDetails) = await CalculateAndAddFees(dateBooking);

                total += totalFee;
                feeDetails.AddRange(feeCommonDetails);

                if (request.IsRoundTrip == true)
                {
                    (double updatedTotal, List<FeeDetail> updatedFeeDetails) = await ApplyPercentFeesAsync(total);
                    total += updatedTotal;
                    feeDetails.AddRange(updatedFeeDetails);
                }

                var isHoliday = await _unitOfWork.HolidaySettingRepository.IsHolidayAsync(dateBooking);
                if (isHoliday)
                {

                    (double updatedTotal, List<FeeDetail> updatedFeeDetails) =
                        await ApplyPercentHolidayFeesAsync(total);
                    total += updatedTotal;
                    feeDetails.AddRange(updatedFeeDetails);

                }
                
                // resource logic 

                var tracker = new BookingTracker();
                tracker.Type = TrackerEnums.PENDING.ToString();
                tracker.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                List<TrackerSource> resourceList = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                tracker.TrackerSources = resourceList;


                // save
                entity.Status = status;
                entity.PorterNumber = porterNumber;
                entity.DriverNumber = driverNumber;
                entity.TruckNumber = request.TruckCategoryId;
                entity.BookingTrackers.Add(tracker);
                entity.BookingDetails = bookingDetails;
                entity.FeeDetails = feeDetails;

                var deposit = total * 30 / 100;

                if (entity.IsReviewOnline == false)
                {
                    var feeReviewerOffline = await _unitOfWork.FeeSettingRepository.GetReviewerFeeSettingsAsync();
                    deposit = feeReviewerOffline!.Amount!.Value;
                }

                entity.Deposit = deposit;
                entity.TotalFee = totalFee;
                entity.TotalReal = total;
                entity.Total = total;
                entity.UserId = int.Parse(userId);

                DateTime now = DateTime.Now;

                if ((request.BookingAt.Value - now).TotalHours <= 3 && (request.BookingAt.Value - now).TotalHours >= 0)
                {
                    entity.TypeBooking = TypeBookingEnums.NOW.ToString();
                }
                else
                {
                    entity.TypeBooking = TypeBookingEnums.DELAY.ToString();
                }

                await _unitOfWork.BookingRepository.AddAsync(entity);

                var checkResult = await _unitOfWork.SaveChangesAsync(); // Make sure this saves the booking

                if (checkResult > 0)
                {
                    BackgroundJob.Schedule(() => CheckAndCancelBooking(entity.Id), entity.BookingAt ?? DateTime.Now);
                    var response = _mapper.Map<BookingResponse>(entity);
                    foreach (var bookingDetail in response.BookingDetails)
                    {
                        var service = await _unitOfWork.ServiceRepository.GetByIdAsync((int)bookingDetail.ServiceId);
                        if (service != null)
                        {
                            bookingDetail.ImageUrl = service.ImageUrl;
                        }
                    }

                    _producer.SendingMessage("movemate.booking_assign_review", entity.Id);
                    await _firebaseServices.SaveBooking(entity, entity.Id, "bookings");
                    int userid = int.Parse(userId);
                    var user = await _unitOfWork.UserRepository.GetByIdAsync(userid);
                    // Sending a booking confirmation email
                    // Inside RegisterBooking method, after the booking is successfully created:
                    //await _emailService.SendBookingSuccessfulEmailAsync(user.Email, response);

                    result.AddResponseStatusCode(StatusCode.Created,
                        MessageConstant.SuccessMessage.RegisterBookingSuccess, response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.RegisterBookingFail);
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        // VAlUA

        #region FEATURE: VAlUATION a booking.

        public async Task<OperationResult<BookingValuationResponse>> ValuationBooking(BookingValuationRequest request,
            string userId)
        {
            var result = new OperationResult<BookingValuationResponse>();

            var existingHouseType =
                await _unitOfWork.HouseTypeRepository.GetByIdAsyncV1(request.HouseTypeId);

            // check houseType
            if (existingHouseType == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundHouseType);
                return result;
            }


            double total = 0;
            var bookingDetails = new List<BookingDetail>();
            var feeDetails = new List<FeeDetail>();

            try
            {
                var (totalServices, listBookingDetails, driverNumber, porterNumber, feeServiceDetails) =
                    await CalculateServiceFees(request.BookingDetails,
                        request.HouseTypeId,
                        request.TruckCategoryId, request.FloorsNumber, request.EstimatedDistance);
                total += totalServices;
                feeDetails.AddRange(feeServiceDetails);
                bookingDetails.AddRange(listBookingDetails);

                var dateBooking = request.BookingAt ?? DateTime.Now;

                var (totalFee, feeCommonDetails) = await CalculateAndAddFees(dateBooking);

                total += totalFee;
                feeDetails.AddRange(feeCommonDetails);

                if (request.IsRoundTrip == true)
                {
                    (double updatedTotal, List<FeeDetail> updatedFeeDetails) = await ApplyPercentFeesAsync(total);
                    total += updatedTotal;
                    feeDetails.AddRange(updatedFeeDetails);
                }

                var isHoliday = await _unitOfWork.HolidaySettingRepository.IsHolidayAsync(dateBooking);
                if (isHoliday)
                {

                    (double updatedTotal, List<FeeDetail> updatedFeeDetails) =
                        await ApplyPercentHolidayFeesAsync(total);
                    total += updatedTotal;
                    feeDetails.AddRange(updatedFeeDetails);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            var response = new BookingValuationResponse();

            response.Total = total;

            var deposit = total * 30 / 100;

            if (request.IsReviewOnline == false)
            {
                var feeReviewerOffline = await _unitOfWork.FeeSettingRepository.GetReviewerFeeSettingsAsync();
                deposit = feeReviewerOffline!.Amount!.Value;
            }

            response.Deposit = deposit;

            response.BookingDetails = _mapper.Map<List<BookingDetailsResponse>>(bookingDetails);
            response.FeeDetails = _mapper.Map<List<FeeDetailResponse>>(feeDetails);
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.ValuationBooking, response);

            return result;
        }

        #endregion

        // CANCEL

        #region FEATURE: Cancel a booking in the system.

        public async Task<OperationResult<BookingResponse>> CancelBooking(int id, int userId,
            BookingCancelRequest request)
        {
            var result = new OperationResult<BookingResponse>();

            if (request.Id != id)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.RequiredId);
                return result;
            }

            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
                return result;
            }

            try
            {
                var entity = await _unitOfWork.BookingRepository.GetByIdAsync(request.Id);
                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                if (entity.Status == BookingEnums.CANCEL.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCancel);
                    return result;
                }

                entity.Status = BookingEnums.CANCEL.ToString();
                entity.IsCancel = true;
                entity.CancelReason = request.CancelReason;

                var vouchers = await _unitOfWork.VoucherRepository.GetUserVouchersByBookingIdAsync(userId, request.Id);
                if (vouchers != null && vouchers.Any())
                {
                    foreach (var voucher in vouchers)
                    {
                        voucher.BookingId = null;
                        //voucher.IsActived = true; 
                        await _unitOfWork.VoucherRepository.UpdateAsync(voucher);
                    }
                }

                _unitOfWork.BookingRepository.Update(entity);
                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    entity = await _unitOfWork.BookingRepository.GetByIdAsyncV1((int)entity.Id,
                        includeProperties:
                        "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments,Vouchers");
                    var response = _mapper.Map<BookingResponse>(entity);
                    await _firebaseServices.SaveBooking(entity, entity.Id, "bookings");
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.CancelBooking,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingUpdateFail);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CancelBooking is Error:  {request.Id}");
                throw;
            }
        }

        #endregion

        //TEST


        // CRONJOB

        #region CRONJOB: CheckAndCancelBooking a booking in the system.

        public async Task CheckAndCancelBooking(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);

            if (booking != null && booking.Status == BookingEnums.PENDING.ToString())
            {
                booking.Status = BookingEnums.CANCEL.ToString();
                booking.IsCancel = true;
                booking.CancelReason = MessageConstant.FailMessage.CancelExpireBooking;
                _unitOfWork.BookingRepository.Update(booking);
                _unitOfWork.Save();
                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
            }

            if (booking != null && booking.Status == BookingEnums.DEPOSITING.ToString())
            {
                booking.Status = BookingEnums.CANCEL.ToString();
                booking.IsCancel = true;
                booking.CancelReason = MessageConstant.FailMessage.CancelExpirePayment;
                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
            }
        }

        #endregion

        // PRIVATE

        #region PRIVATE: ApplyPercentFeesAsync for a new booking in the system.

        private async Task<(double updatedTotal, List<FeeDetail> feeDetails)> ApplyPercentFeesAsync(double total)
        {
            var feePercentSettings = await _unitOfWork.FeeSettingRepository.GetPercentSystemFeeSettingsAsync();

            var feeDetails = new List<FeeDetail>();

            var totalAmount = 0d;

            foreach (var feeSetting in feePercentSettings)
            {
                var value = feeSetting.Amount ?? 100;
                var amount = total * value / 100;

                var feeDetail = new FeeDetail
                {
                    FeeSettingId = feeSetting.Id,
                    Name = feeSetting.Name,
                    Description = feeSetting.Description,
                    Amount = amount,
                };

                feeDetails.Add(feeDetail);
                totalAmount += amount;
            }

            return (totalAmount, feeDetails);
        }

        private async Task<(double updatedTotal, List<FeeDetail> feeDetails)> ApplyPercentHolidayFeesAsync(double total)
        {
            var feePercentSettings = await _unitOfWork.FeeSettingRepository.GetPercentHolidayFeeSettingsAsync();

            var feeDetails = new List<FeeDetail>();

            var totalAmount = 0d;

            foreach (var feeSetting in feePercentSettings)
            {
                var value = feeSetting.Amount ?? 100;
                var amount = total * value / 100;

                var feeDetail = new FeeDetail
                {
                    FeeSettingId = feeSetting.Id,
                    Name = feeSetting.Name,
                    Description = feeSetting.Description,
                    Amount = amount,
                };

                feeDetails.Add(feeDetail);
                totalAmount += amount;
            }

            return (totalAmount, feeDetails);
        }

        #endregion

        #region PRIVATE: AddFeeDetails for a new booking in the system.

        private (double totalFee, List<FeeDetail> feeDetails) AddFeeDetails(IEnumerable<FeeSetting> feeSettings)
        {
            double totalFee = 0;
            var feeDetails = new List<FeeDetail>();

            foreach (var feeSetting in feeSettings)
            {
                var feeDetail = new FeeDetail
                {
                    FeeSettingId = feeSetting.Id,
                    Name = feeSetting.Name,
                    Description = feeSetting.Description,
                    Amount = feeSetting.Amount,
                };

                feeDetails.Add(feeDetail);
                totalFee += feeSetting.Amount ?? 0;
            }

            return (totalFee, feeDetails);
        }

        #endregion

        #region PRIVATE: CalculateAndAddFees for a new booking in the system.

        private async Task<(double totalFee, List<FeeDetail> feeNullDetails)> CalculateAndAddFees(DateTime dateBooking)
        {
            double totalFee = 0;
            var feeNullDetails = new List<FeeDetail>();

            // Get common fees
            var feeSettings = await _unitOfWork.FeeSettingRepository.GetCommonFeeSettingsAsync();
            var (commonFee, commonFeeDetails) = AddFeeDetails(feeSettings);
            totalFee += commonFee;
            feeNullDetails.AddRange(commonFeeDetails);

            // Check if the date is a weekend
            if (DateUtil.IsWeekend(dateBooking))
            {
                var feeWeekendSettings = await _unitOfWork.FeeSettingRepository.GetWeekendFeeSettingsAsync();
                var (weekendFee, weekendFeeDetails) = AddFeeDetails(feeWeekendSettings);
                totalFee += weekendFee;
                feeNullDetails.AddRange(weekendFeeDetails);
            }

            // Check if it's outside business hours
            if (DateUtil.IsOutsideBusinessHours(dateBooking))
            {
                var feeOBHSettings = await _unitOfWork.FeeSettingRepository.GetOBHFeeSettingsAsync();
                var (obhFee, obhFeeDetails) = AddFeeDetails(feeOBHSettings);
                totalFee += obhFee;
                feeNullDetails.AddRange(obhFeeDetails);
            }

            return (totalFee, feeNullDetails);
        }

        #endregion

        #region PRIVATE: CalculateDistanceFee for a new booking in the system.

        private (double totalFee, List<FeeDetail> feeDetails) CalculateDistanceFee(int truckCategoryId,
            double estimatedDistance,
            List<FeeSetting>? feeSettings, int quantity)
        {
            if (feeSettings == null || !feeSettings.Any())
            {
                // Return 0 total fee and an empty list of fee details
                return (0, new List<FeeDetail>());
            }

            // Lọc các FeeSetting thuộc TruckCategoryId và có Unit là "KM"
            var relevantFees = feeSettings
                .Where(f => f.Unit == "KM" && f.IsActived == true)
                .OrderBy(f => f.RangeMin)
                .ToList();

            double totalFee = 0;
            double remainingDistance = estimatedDistance;
            var feeDetails = new List<FeeDetail>();

            foreach (var fee in relevantFees)
            {
                double rangeMin = fee.RangeMin ?? 0;
                double rangeMax = fee.RangeMax ?? double.MaxValue; // Nếu RangeMax là null, hiểu là không giới hạn

                // Nếu RangeMin = 0, thì đây là mức giá mặc định
                if (rangeMin == 0)
                {
                    totalFee += fee.Amount ?? 0;
                    remainingDistance -= (rangeMax - rangeMin);

                    // Thêm FeeDetail vào danh sách
                    var feeDetail = new FeeDetail
                    {
                        FeeSettingId = fee.Id,
                        Name = fee.Name,
                        Description = fee.Description,
                        Amount = (fee.Amount ?? 0) * quantity,
                        Quantity = quantity
                    };
                    feeDetails.Add(feeDetail);
                }
                else
                {
                    // Tính khoảng cách trong khoảng này
                    if (estimatedDistance > rangeMin)
                    {
                        double distanceInRange = Math.Min(remainingDistance, rangeMax - rangeMin);
                        totalFee += distanceInRange * (fee.Amount ?? 0);

                        // Thêm FeeDetail vào danh sách
                        var feeDetail = new FeeDetail
                        {
                            FeeSettingId = fee.Id,
                            Name = fee.Name,
                            Description = fee.Description,
                            Amount = distanceInRange * (fee.Amount ?? 0) * quantity,
                            Quantity = quantity
                        };
                        feeDetails.Add(feeDetail);

                        remainingDistance -= distanceInRange;
                    }

                    // Nếu khoảng cách đã được tính hết, thoát khỏi vòng lặp
                    if (remainingDistance <= 0)
                    {
                        break;
                    }
                }
            }

            // Nếu còn khoảng cách chưa được tính và không có phí cho phần còn lại
            if (remainingDistance > 0)
            {
                var lastFeeSetting = relevantFees.LastOrDefault(f => f.RangeMax == null);
                if (lastFeeSetting != null)
                {
                    totalFee += remainingDistance * (lastFeeSetting.Amount ?? 0);

                    // Thêm FeeDetail vào danh sách
                    var feeDetail = new FeeDetail
                    {
                        FeeSettingId = lastFeeSetting.Id,
                        Name = lastFeeSetting.Name,
                        Description = lastFeeSetting.Description,
                        Amount = remainingDistance * (lastFeeSetting.Amount ?? 0) * quantity,
                        Quantity = quantity
                    };
                    feeDetails.Add(feeDetail);
                }
            }

            totalFee = totalFee * quantity;
            return (totalFee, feeDetails);
        }

        #endregion

        #region PRIVATE: SeparateFeeSettingsByUnit for a new booking in the system.

        private (List<FeeSetting>? nullUnitFees, List<FeeSetting>? kmUnitFees, List<FeeSetting>? floorUnitFees)
            SeparateFeeSettingsByUnit(List<FeeSetting> feeSettings, int HouseTypeId, int CateTruckId)
        {
            var nullUnitFees = new List<FeeSetting>();
            var kmUnitFees = new List<FeeSetting>();
            var floorUnitFees = new List<FeeSetting>();

            //   feeSettings.AddRange(_unitOfWork.FeeSettingRepository.GetTruckFeeSettings(CateTruckId));

            foreach (var fee in feeSettings)
            {
                switch (fee.Unit)
                {
                    case null:
                        nullUnitFees.Add(fee);
                        break;
                    case "KM":
                        kmUnitFees.Add(fee);
                        break;
                    case "FLOOR":
                        if (fee.HouseTypeId == HouseTypeId)
                        {
                            floorUnitFees.Add(fee);
                        }

                        break;
                }
            }

            return (nullUnitFees, kmUnitFees, floorUnitFees);
        }

        #endregion

        #region PRIVATE: CalculateBaseFee for a new booking in the system.

        private (double totalNullFee, List<FeeDetail> feeNullDetails) CalculateBaseFee(List<FeeSetting>? nullUnitFees,
            int quantity)
        {
            if (nullUnitFees == null || !nullUnitFees.Any())
            {
                // Return 0 total fee and an empty list of fee details
                return (0, new List<FeeDetail>());
            }

            // Khởi tạo danh sách FeeDetail để trả về
            var feeDetails = new List<FeeDetail>();
            double totalFee = 0;

            // Lặp qua danh sách nullUnitFees
            foreach (var feeSetting in nullUnitFees)
            {
                // Tính toán phí dựa trên FeeSetting
                double feeAmount = (feeSetting.Amount ?? 0) * quantity;
                totalFee += feeAmount; // Cộng dồn vào totalFee

                // Tạo FeeDetail từ FeeSetting
                var feeDetail = new FeeDetail
                {
                    FeeSettingId = feeSetting.Id,
                    Name = feeSetting.Name,
                    Description = feeSetting.Description,
                    Amount = feeAmount,
                    Quantity = quantity
                };

                // Thêm FeeDetail vào danh sách feeDetails
                feeDetails.Add(feeDetail);
            }

            // Trả về tổng phí và danh sách FeeDetail
            return (totalFee, feeDetails);
        }

        #endregion

        #region PRIVATE: CalculateFloorFeeV2 for a new booking in the system.

        private (double totalFee, List<FeeDetail> feeDetails) CalculateFloorFeeV2(int truckCategoryId,
            int numberOfFloors,
            List<FeeSetting>? feeSettings, int quantity)
        {
            if (feeSettings == null || !feeSettings.Any())
            {
                // Return 0 total fee and an empty list of fee details
                return (0, new List<FeeDetail>());
            }

            // Lọc các FeeSetting thuộc TruckCategoryId và có Unit là "FLOOR"
            var relevantFees = feeSettings
                .Where(f => f.Unit == "FLOOR" && f.IsActived == true)
                .OrderBy(f => f.RangeMin)
                .ToList();

            double totalFee = 0;
            int remainingFloors = numberOfFloors;
            var feeDetails = new List<FeeDetail>();
            double defaultAmount = 0;
            foreach (var fee in relevantFees)
            {
                double rangeMin = fee.RangeMin ?? 0;
                double rangeMax = fee.RangeMax ?? double.MaxValue; // Nếu RangeMax là null, hiểu là không giới hạn
                double baseAmount = fee.Amount ?? 0;
                double currentAmount = defaultAmount;

                // Nếu RangeMin = 0 hoặc 1, tính phí cố định từ RangeMin đến RangeMax
                if (rangeMin == 0 || rangeMin == 1)
                {
                    if (numberOfFloors > rangeMin)
                    {
                        int floorsInRange = (int)Math.Min(remainingFloors, rangeMax - rangeMin);
                        totalFee += baseAmount * quantity; // Giá cố định cho khoảng từ RangeMin đến RangeMax

                        // Thêm FeeDetail vào danh sách
                        feeDetails.Add(new FeeDetail
                        {
                            FeeSettingId = fee.Id,
                            Name = fee.Name,
                            Description = fee.Description,
                            Amount = baseAmount * quantity,
                            Quantity = quantity
                        });

                        remainingFloors -= floorsInRange;

                        defaultAmount = baseAmount;
                    }
                }
                else
                {
                    // Tính phí cho số tầng trong khoảng này
                    if (numberOfFloors > rangeMin)
                    {
                        double floorPercentage = (fee.FloorPercentage ?? 100) / 100;
                        int floorsInRange = (int)Math.Min(remainingFloors, rangeMax - rangeMin);
                        totalFee += floorsInRange * currentAmount * quantity * floorPercentage;

                        // Thêm FeeDetail vào danh sách
                        feeDetails.Add(new FeeDetail
                        {
                            FeeSettingId = fee.Id,
                            Name = fee.Name,
                            Description = fee.Description,
                            Amount = floorsInRange * currentAmount * quantity * floorPercentage,
                            Quantity = quantity
                        });

                        remainingFloors -= floorsInRange;
                    }
                }

                // Nếu đã tính hết số tầng, thoát khỏi vòng lặp
                if (remainingFloors <= 0)
                {
                    break;
                }
            }

            return (totalFee, feeDetails);
        }

        #endregion

        #region PRIVATE: CalculateServiceFees for a new booking in the system.

        private async
            Task<(double totalServices, List<BookingDetail> bookingDetails, int driverNumber, int porterNumber,
                List<FeeDetail> feeDetails)> CalculateServiceFees(
                List<BookingDetailRequest> bookingDetailRequests,
                int houseTypeId,
                int truckCategoryId,
                string floorsNumber,
                string estimatedDistance)
        {
            double totalServices = 0;
            var bookingDetails = new List<BookingDetail>();
            int driverNumber = 0;
            int porterNumber = 0;
            var feeDetails = new List<FeeDetail>();

            var isServiceDepen = false;
            var isServiceSupper = false;
            
            foreach (var bookingDetailRequest in bookingDetailRequests)
            {
                if (bookingDetailRequest.Quantity == 0)
                {
                    //_unitOfWork.BookingDetailRepository.GetAsyncByServiceIdAndBookingId(bookingDetailRequest.ServiceId, )
                    continue;
                }

                // Check Service
                var service =
                    await _unitOfWork.ServiceRepository.GetByIdAsyncV1(bookingDetailRequest.ServiceId, "FeeSettings");


                if (service == null)
                {
                    throw new NotFoundException(
                        MessageConstant.FailMessage
                            .NotFoundService); // Consider throwing an exception for better error handling
                }

                // Set var
                var quantity = bookingDetailRequest.Quantity;
                var price = service.Amount * quantity - service.Amount * quantity * ((service.DiscountRate ?? 0) / 100);

                if (service.FeeSettings.Count() == 0 && service.Type == RoleEnums.PORTER.ToString())
                {
                    isServiceDepen = true;
                }
                
                if (service.FeeSettings.Count() > 0)
                {
                    // Logic fee 

                    var amount = 0d;
                    var (nullUnitFees, kmUnitFees, floorUnitFees) =
                        SeparateFeeSettingsByUnit(service.FeeSettings.ToList(), houseTypeId, truckCategoryId);

                    switch (service.Type)
                    {
                        case null:

                            break;
                        case "TRUCK":
                            // FEE DISTANCE
                            if (service.TruckCategoryId != truckCategoryId)
                            {
                                throw new BadRequestException(MessageConstant.FailMessage
                                    .InvalidBookingDetailDifferent);
                            }

                            if (service.Tier == 0)
                            {
                                throw new BadRequestException(MessageConstant.FailMessage.InvalidServiceTier);
                            }

                            var (totalTruckFee, feeTruckDetails) = CalculateDistanceFee(truckCategoryId,
                                double.Parse(estimatedDistance.ToString()), kmUnitFees, quantity ?? 1);
                            amount += totalTruckFee;
                            driverNumber = quantity ?? 0;
                            //feeDetails.AddRange(feeTruckDetails);
                            break;
                        case "PORTER":
                            // FEE FLOOR
                            if (service.Tier == 0)
                            {
                                throw new BadRequestException(MessageConstant.FailMessage.InvalidServiceTier);
                            }

                            var (floorTotalFee, floorUnitFeeDetails) = CalculateFloorFeeV2(truckCategoryId,
                                int.Parse(floorsNumber ?? "1"), floorUnitFees, quantity ?? 1);
                            amount += floorTotalFee;
                            porterNumber = quantity ?? 0;
                            //feeDetails.AddRange(floorUnitFeeDetails);
                            isServiceSupper = true;
                            break;
                    }

                    // FEE BASE
                    var (nullTotalFee, nullUnitFeeDetails) = CalculateBaseFee(nullUnitFees, quantity ?? 1);
                    amount += nullTotalFee;
                    feeDetails.AddRange(nullUnitFeeDetails);

                    amount = (double)(amount -
                                      amount * (service.DiscountRate ?? 0) / 100)!;

                    totalServices += amount;

                    var bookingDetail = new BookingDetail
                    {
                        ServiceId = service.Id,
                        Quantity = quantity,
                        Price = amount,
                        Name = service.Name,
                        Status = BookingDetailStatusEnums.AVAILABLE.ToString(),
                        Description = service.Description,
                        Type = service.Type
                    };

                    bookingDetails.Add(bookingDetail);
                }
                else
                {
                    var bookingDetail = new BookingDetail
                    {
                        ServiceId = service.Id,
                        Quantity = quantity,
                        Price = price,
                        Name = service.Name,
                        Status = BookingDetailStatusEnums.AVAILABLE.ToString(),
                        Description = service.Description,
                        Type = service.Type
                    };
                    totalServices += price.Value;
                    bookingDetails.Add(bookingDetail);
                }
            }

            /*if (isServiceDepen == true && isServiceSupper == false)
            {
                throw new BadRequestException(MessageConstant.FailMessage.InvalidServiceDepen);

            }*/

            return (totalServices, bookingDetails, driverNumber, porterNumber, feeDetails);
        }

        #endregion

        //
        //Driver update status booking  detail
        public async Task<OperationResult<AssignmentResponse>> DriverUpdateStatusBooking(int userId, int bookingId, TrackerSourceRequest request)
        {
            var result = new OperationResult<AssignmentResponse>();

            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, includeProperties: "Assignments");
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                var assignment =
                    await _unitOfWork.AssignmentsRepository.GetByUserIdAndStaffTypeAndIsResponsible(userId,
                        RoleEnums.DRIVER.ToString(), bookingId);
                if (assignment == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                DateTime currentTime = DateTime.Now;
                if (booking.BookingAt.HasValue)
                {
                    DateTime earliestUpdateTime = booking.BookingAt.Value.AddMinutes(-30);
                    if (currentTime < earliestUpdateTime)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateTimeNotAllowed);
                        return result;
                    }
                }



                string nextStatus = assignment.Status;

                switch (assignment.Status)
                {
                    case var status when status == AssignmentStatusEnums.ASSIGNED.ToString():
                        nextStatus = AssignmentStatusEnums.INCOMING.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.INCOMING.ToString():
                        var bookingTracker =
                            await _unitOfWork.BookingTrackerRepository.GetBookingTrackerByBookingIdAsync(booking.Id);
                        if (bookingTracker == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingTracker);
                            return result;
                        }

                        if (request.ResourceList.Count() <= 0)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VerifyReviewOffline);
                            return result;
                        }

                        var tracker = new BookingTracker();
                        tracker.BookingId = booking.Id;
                        tracker.Type = TrackerEnums.DRIVER_ARRIVED.ToString();
                        tracker.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                        List<TrackerSource> resourceList = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                        tracker.TrackerSources = resourceList;
                        await _unitOfWork.BookingTrackerRepository.AddAsync(tracker);
                        nextStatus = AssignmentStatusEnums.ARRIVED.ToString();
                        booking.Status = BookingEnums.IN_PROGRESS.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.ARRIVED.ToString() &&
                                         booking.Status == BookingEnums.IN_PROGRESS.ToString():
                        nextStatus = AssignmentStatusEnums.IN_PROGRESS.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.IN_PROGRESS.ToString() &&
                                         booking.Status == BookingEnums.IN_PROGRESS.ToString():
                        var bookingTrackers =
                            await _unitOfWork.BookingTrackerRepository.GetBookingTrackerByBookingIdAsync(booking.Id);
                        if (bookingTrackers == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingTracker);
                            return result;
                        }

                        if (request.ResourceList.Count() <= 0)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VerifyReviewOffline);
                            return result;
                        }
                        if (booking.BookingDetails.Any(bd => bd.Status != BookingDetailStatusEnums.AVAILABLE.ToString()))
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.SomethingWrong);
                            return result;
                        }

                        var trackers = new BookingTracker();
                        trackers.BookingId = booking.Id;
                        trackers.Type = TrackerEnums.DRIVER_COMPLETED.ToString();
                        trackers.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                        List<TrackerSource> resourceLists = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                        trackers.TrackerSources = resourceLists;
                        await _unitOfWork.BookingTrackerRepository.AddAsync(trackers);
                        nextStatus = AssignmentStatusEnums.COMPLETED.ToString();
                        break;

                    default:
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CanNotUpdateStatus);
                        return result;
                }

                assignment.Status = nextStatus;
                await _unitOfWork.AssignmentsRepository.SaveOrUpdateAsync(assignment);
                await _unitOfWork.BookingRepository.SaveOrUpdateAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<AssignmentResponse>(assignment);
                booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, includeProperties: "Assignments");
                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateStatusSuccess,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }


        public async Task<OperationResult<AssignmentResponse>> DriverUpdateRoundTripBooking(int bookingId)
        {
            var result = new OperationResult<AssignmentResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingDetail);
                    return result;
                }

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)bookingDetail.BookingId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                string nextStatus = bookingDetail.Status;

                switch (bookingDetail.Status)
                {
                    case var status when status == AssignmentStatusEnums.WAITING.ToString():
                        nextStatus = AssignmentStatusEnums.ASSIGNED.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.ASSIGNED.ToString():
                        nextStatus = AssignmentStatusEnums.INCOMING.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.INCOMING.ToString():
                        nextStatus = AssignmentStatusEnums.ARRIVED.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.ARRIVED.ToString():
                        nextStatus = AssignmentStatusEnums.IN_PROGRESS.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.IN_PROGRESS.ToString():
                        nextStatus = AssignmentStatusEnums.COMPLETED.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.COMPLETED.ToString():
                        if (booking.IsUserConfirm == true && booking.IsRoundTrip == false)
                        {
                            nextStatus = AssignmentStatusEnums.CONFIRM.ToString();
                            booking.IsRoundTrip = true;
                            bookingDetail.IsRoundTripCompleted = true;
                        }

                        break;
                    case var status when status == AssignmentStatusEnums.CONFIRM.ToString():
                        nextStatus = AssignmentStatusEnums.ROUND_TRIP.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.ROUND_TRIP.ToString():
                        nextStatus = AssignmentStatusEnums.ARRIVED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CanNotUpdateStatus);
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<AssignmentResponse>(bookingDetail);
                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateStatusSuccess,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<AssignmentResponse>> ReportFail(int bookingId, string failedReason)
        {
            var result = new OperationResult<AssignmentResponse>();

            try
            {
                var assignment = await _unitOfWork.AssignmentsRepository.GetByIdAsync(bookingId);
                if (assignment == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                string nextStatus = assignment.Status;

                switch (assignment.Status)
                {
                    case var status when status == AssignmentStatusEnums.ASSIGNED.ToString():
                        nextStatus = AssignmentStatusEnums.FAILED.ToString();
                        assignment.FailedReason = failedReason;
                        break;
                    case var status when status == AssignmentStatusEnums.INCOMING.ToString():
                        nextStatus = AssignmentStatusEnums.FAILED.ToString();
                        assignment.FailedReason = failedReason;
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CanNotUpdateStatus);
                        return result;
                }

                assignment.Status = nextStatus;
                _unitOfWork.AssignmentsRepository.Update(assignment);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<AssignmentResponse>(assignment);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateStatusSuccess,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }


        public async Task<OperationResult<AssignmentResponse>> PorterUpdateStatusBooking(int userId, int bookingId, TrackerSourceRequest request)
        {
            var result = new OperationResult<AssignmentResponse>();

            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, includeProperties: "Assignments");
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                var assignment =
                    await _unitOfWork.AssignmentsRepository.GetByUserIdAndStaffTypeAndIsResponsible(userId,
                        RoleEnums.PORTER.ToString(), bookingId);
                if (assignment == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                DateTime currentTime = DateTime.Now;
                if (booking.BookingAt.HasValue)
                {
                    DateTime earliestUpdateTime = booking.BookingAt.Value.AddMinutes(-30);
                    if (currentTime < earliestUpdateTime)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateTimeNotAllowed);
                        return result;
                    }
                }

                string nextStatus = assignment.Status;

                switch (assignment.Status)
                {
                    case var status when status == AssignmentStatusEnums.ASSIGNED.ToString():
                        nextStatus = AssignmentStatusEnums.INCOMING.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.INCOMING.ToString():
                        var bookingTracker =
                            await _unitOfWork.BookingTrackerRepository.GetBookingTrackerByBookingIdAsync(booking.Id);
                        if (bookingTracker == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingTracker);
                            return result;
                        }

                        if (request.ResourceList.Count() <= 0)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VerifyReviewOffline);
                            return result;
                        }

                        var tracker = new BookingTracker();
                        tracker.BookingId = booking.Id;
                        tracker.Type = TrackerEnums.PORTER_ARRIVED.ToString();
                        tracker.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                        List<TrackerSource> resourceList = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                        tracker.TrackerSources = resourceList;

                        await _unitOfWork.BookingTrackerRepository.AddAsync(tracker);
                        booking.Status = BookingEnums.IN_PROGRESS.ToString();
                        nextStatus = AssignmentStatusEnums.ARRIVED.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.ARRIVED.ToString() &&
                                         booking.Status == BookingEnums.IN_PROGRESS.ToString():
                        nextStatus = AssignmentStatusEnums.IN_PROGRESS.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.IN_PROGRESS.ToString() &&
                                         booking.Status == BookingEnums.IN_PROGRESS.ToString():
                        var bookingTrackers =
                            await _unitOfWork.BookingTrackerRepository.GetBookingTrackerByBookingIdAsync(booking.Id);
                        if (bookingTrackers == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingTracker);
                            return result;
                        }

                        if (request.ResourceList.Count() <= 0)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VerifyReviewOffline);
                            return result;
                        }

                        var trackers = new BookingTracker();
                        trackers.BookingId = booking.Id;
                        trackers.Type = TrackerEnums.PORTER_ONGOING.ToString();
                        trackers.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                        List<TrackerSource> resourceLists = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                        trackers.TrackerSources = resourceLists;
                        await _unitOfWork.BookingTrackerRepository.AddAsync(trackers);
                        nextStatus = AssignmentStatusEnums.ONGOING.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.ONGOING.ToString() &&
                                         booking.Status == BookingEnums.IN_PROGRESS.ToString():
                        var bookingTrackerDelis =
                            await _unitOfWork.BookingTrackerRepository.GetBookingTrackerByBookingIdAsync(booking.Id);
                        if (bookingTrackerDelis == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingTracker);
                            return result;
                        }

                        if (request.ResourceList.Count() <= 0)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VerifyReviewOffline);
                            return result;
                        }

                        var trackerDelis = new BookingTracker();
                        trackerDelis.BookingId = booking.Id;
                        trackerDelis.Type = TrackerEnums.PORTER_DELIVERED.ToString();
                        trackerDelis.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                        List<TrackerSource> resourceListDelis = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                        trackerDelis.TrackerSources = resourceListDelis;
                        await _unitOfWork.BookingTrackerRepository.AddAsync(trackerDelis);
                        nextStatus = AssignmentStatusEnums.DELIVERED.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.DELIVERED.ToString() &&
                                         booking.Status == BookingEnums.IN_PROGRESS.ToString():
                        var bookingTrackerUnloads =
                            await _unitOfWork.BookingTrackerRepository.GetBookingTrackerByBookingIdAsync(booking.Id);
                        if (bookingTrackerUnloads == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingTracker);
                            return result;
                        }

                        if (request.ResourceList.Count() <= 0)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VerifyReviewOffline);
                            return result;
                        }

                        var trackerUnloads = new BookingTracker();
                        trackerUnloads.BookingId = booking.Id;
                        trackerUnloads.Type = TrackerEnums.PORTER_UNLOADED.ToString();
                        trackerUnloads.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                        List<TrackerSource> resourceListUnloads = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                        trackerUnloads.TrackerSources = resourceListUnloads;
                        await _unitOfWork.BookingTrackerRepository.AddAsync(trackerUnloads);
                        nextStatus = AssignmentStatusEnums.UNLOAD.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.UNLOAD.ToString() &&
                                         booking.Status == BookingEnums.IN_PROGRESS.ToString():
                        var bookingTrackerComs =
                            await _unitOfWork.BookingTrackerRepository.GetBookingTrackerByBookingIdAsync(booking.Id);
                        if (bookingTrackerComs == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingTracker);
                            return result;
                        }

                        if (request.ResourceList.Count() <= 0)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VerifyReviewOffline);
                            return result;
                        }
                        if (booking.BookingDetails.Any(bd => bd.Status != BookingDetailStatusEnums.AVAILABLE.ToString()))
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.SomethingWrong);
                            return result;
                        }

                        var trackerComs = new BookingTracker();
                        trackerComs.BookingId = booking.Id;
                        trackerComs.Type = TrackerEnums.PORTER_COMPLETED.ToString();
                        trackerComs.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                        List<TrackerSource> resourceListComs = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                        trackerComs.TrackerSources = resourceListComs;
                        await _unitOfWork.BookingTrackerRepository.AddAsync(trackerComs);
                        nextStatus = AssignmentStatusEnums.COMPLETED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CanNotUpdateStatus);
                        return result;
                }

                assignment.Status = nextStatus;
                _unitOfWork.AssignmentsRepository.Update(assignment);
                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<AssignmentResponse>(assignment);
                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateStatusSuccess,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<AssignmentResponse>> PorterRoundTripBooking(int bookingId,
            ResourceRequest request)
        {
            var result = new OperationResult<AssignmentResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingDetail);
                    return result;
                }

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)bookingDetail.BookingId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                string nextStatus = bookingDetail.Status;

                switch (bookingDetail.Status)
                {
                    case var status when status == AssignmentStatusEnums.WAITING.ToString():
                        nextStatus = AssignmentStatusEnums.ASSIGNED.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.ASSIGNED.ToString():
                        nextStatus = AssignmentStatusEnums.INCOMING.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.INCOMING.ToString():
                        nextStatus = AssignmentStatusEnums.ARRIVED.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.ARRIVED.ToString():
                        nextStatus = AssignmentStatusEnums.IN_PROGRESS.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.IN_PROGRESS.ToString():
                        nextStatus = AssignmentStatusEnums.ONGOING.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.ONGOING.ToString():
                        nextStatus = AssignmentStatusEnums.DELIVERED.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.DELIVERED.ToString():
                        nextStatus = AssignmentStatusEnums.UNLOAD.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.UNLOAD.ToString():
                        nextStatus = AssignmentStatusEnums.COMPLETED.ToString();
                        break;
                    case var status when status == AssignmentStatusEnums.COMPLETED.ToString():
                        if (booking.IsUserConfirm == true && booking.IsRoundTrip == false)
                        {
                            nextStatus = AssignmentStatusEnums.CONFIRM.ToString();
                            bookingDetail.IsRoundTripCompleted = true;
                            booking.IsRoundTrip = true;
                        }

                        break;
                    case var status when status == AssignmentStatusEnums.CONFIRM.ToString():
                        nextStatus = AssignmentStatusEnums.ROUND_TRIP.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.ROUND_TRIP.ToString():
                        nextStatus = AssignmentStatusEnums.ARRIVED.ToString();
                        break;

                    default:
                        result.AddError(StatusCode.BadRequest,
                            MessageConstant.FailMessage.CanNotUpdateStatus);
                        return result;
                }

                bookingDetail.Status = nextStatus;

                var bookingTracker =
                    await _unitOfWork.BookingTrackerRepository.GetBookingTrackerByBookingIdAsync(booking.Id);
                if (bookingTracker == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingDetail);
                    return result;
                }

                var trackerSource = new TrackerSource
                {
                    BookingTrackerId = bookingTracker.Id,
                    ResourceUrl = request.ResourceUrl,
                    ResourceCode = request.ResourceCode,
                    Type = request.Type,
                    IsDeleted = false
                };
                await _unitOfWork.TrackerSourceRepository.AddAsync(trackerSource);
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                _unitOfWork.BookingRepository.Update(booking);

                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<AssignmentResponse>(bookingDetail);
                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateStatusSuccess,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }


        public async Task<OperationResult<AssignmentResponse>> ReviewerUpdateStatusBooking(int userId, int bookingId,
            TrackerByReviewOfflineRequest request)
        {
            var result = new OperationResult<AssignmentResponse>();

            try
            {
                var assigment = await
                    _unitOfWork.AssignmentsRepository.GetByUserIdAndStaffType(userId, RoleEnums.REVIEWER.ToString(),
                        bookingId);
                if (assigment == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }


                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                if (booking.Status == BookingEnums.CANCEL.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCancel);
                    return result;
                }


                string nextStatus = assigment.Status;

                switch (assigment.Status)
                {
                    case var status when status == AssignmentStatusEnums.ASSIGNED.ToString() &&
                                         booking.Status == BookingEnums.REVIEWING.ToString() &&
                                         booking.IsReviewOnline == true:

                        if (request.EstimatedDeliveryTime == null)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingNotEstimated);
                            return result;
                        }
                        else
                        {
                            booking.EstimatedDeliveryTime = request.EstimatedDeliveryTime;
                        }

                        nextStatus = AssignmentStatusEnums.REVIEWED.ToString();
                        booking.Status = BookingEnums.REVIEWED.ToString();
                        booking.IsStaffReviewed = true;
                        break;
                    case var status when status == AssignmentStatusEnums.ARRIVED.ToString() &&
                                         booking.Status == BookingEnums.REVIEWING.ToString() &&
                                         booking.IsReviewOnline == false:

                        if (booking.IsReviewOnline == false)
                        {
                            var bookingTracker =
                                await _unitOfWork.BookingTrackerRepository
                                    .GetBookingTrackerByBookingIdAsync(booking.Id);
                            if (bookingTracker == null)
                            {
                                result.AddError(StatusCode.NotFound,
                                    MessageConstant.FailMessage.NotFoundBookingTracker);
                                return result;
                            }

                            if (request.ResourceList.Count() <= 0)
                            {
                                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VerifyReviewOffline);
                                return result;
                            }

                            var tracker = new BookingTracker();
                            tracker.Type = TrackerEnums.REVIEW_OFFLINE.ToString();
                            tracker.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                            List<TrackerSource> resourceList = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                            tracker.TrackerSources = resourceList;
                            await _unitOfWork.BookingTrackerRepository.AddAsync(tracker);
                        }

                        if (request.EstimatedDeliveryTime == null)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingNotEstimated);
                            return result;
                        }
                        else
                        {
                            booking.EstimatedDeliveryTime = request.EstimatedDeliveryTime;
                        }

                        nextStatus = AssignmentStatusEnums.REVIEWED.ToString();
                        booking.Status = BookingEnums.REVIEWED.ToString();
                        booking.IsStaffReviewed = true;
                        break;

                    case var status when status == AssignmentStatusEnums.ASSIGNED.ToString():
                        if (booking.IsReviewOnline == true && booking.Status == BookingEnums.ASSIGNED.ToString())
                        {
                            booking.Status = BookingEnums.REVIEWING.ToString();
                        }
                        else if (booking.IsReviewOnline == false && booking.Status == BookingEnums.REVIEWING.ToString())
                        {
                            nextStatus = AssignmentStatusEnums.INCOMING.ToString();
                        }
                        else
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingReviewing);
                            return result;
                        }

                        break;

                    case var status when status == AssignmentStatusEnums.INCOMING.ToString():
                        nextStatus = AssignmentStatusEnums.ARRIVED.ToString();
                        break;
                    //case var status when status == AssignmentStatusEnums.ARRIVED.ToString():
                    //    nextStatus = AssignmentStatusEnums.SUGGESTED.ToString();

                    //    break;

                    case var status when status == AssignmentStatusEnums.SUGGESTED.ToString() &&
                                         booking.Status == BookingEnums.REVIEWING.ToString():

                        if (booking.IsReviewOnline == false)
                        {
                            var bookingTracker =
                                await _unitOfWork.BookingTrackerRepository
                                    .GetBookingTrackerByBookingIdAsync(booking.Id);
                            if (bookingTracker == null)
                            {
                                result.AddError(StatusCode.NotFound,
                                    MessageConstant.FailMessage.NotFoundBookingTracker);
                                return result;
                            }

                            if (request.ResourceList.Count() <= 0)
                            {
                                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VerifyReviewOffline);
                                return result;
                            }

                            var tracker = new BookingTracker();
                            tracker.Type = TrackerEnums.REVIEW_OFFLINE.ToString();
                            tracker.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                            List<TrackerSource> resourceList = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                            tracker.TrackerSources = resourceList;
                            await _unitOfWork.BookingTrackerRepository.AddAsync(tracker);
                        }

                        if (booking.EstimatedDeliveryTime == null)
                        {
                            if (request.EstimatedDeliveryTime == null)
                            {
                                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingNotEstimated);
                                return result;
                            }
                            else
                            {
                                booking.EstimatedDeliveryTime = request.EstimatedDeliveryTime;
                            }
                        }
                        else if (booking.EstimatedDeliveryTime != null && request.EstimatedDeliveryTime.HasValue)
                        {
                            booking.EstimatedDeliveryTime = request.EstimatedDeliveryTime;
                        }

                        nextStatus = AssignmentStatusEnums.REVIEWED.ToString();
                        booking.Status = BookingEnums.REVIEWED.ToString();
                        booking.IsStaffReviewed = true;
                        break;

                    default:
                        result.AddError(StatusCode.BadRequest,
                            MessageConstant.FailMessage.CanNotUpdateStatus);
                        return result;
                }

                assigment.Status = nextStatus;
                booking.UpdatedAt = DateTime.Now;
                _unitOfWork.AssignmentsRepository.Update(assigment);
                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<AssignmentResponse>(assigment);
                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateStatusSuccess,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<AssignmentResponse>> ReviewerCancelBooking(int bookingId)
        {
            var result = new OperationResult<AssignmentResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingDetail);
                    return result;
                }

                //var booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)bookingDetail.BookingId);
                //if (booking == null)
                //{
                //    result.AddError(StatusCode.NotFound, "Booking not found.");
                //    return result;
                //}
                string nextStatus = bookingDetail.Status;

                switch (bookingDetail.Status)
                {
                    case var status when status == AssignmentStatusEnums.SUGGESTED.ToString():
                        nextStatus = AssignmentStatusEnums.CANCELLED.ToString();
                        break;

                    case var status when status == AssignmentStatusEnums.CANCELLED.ToString():
                        nextStatus = AssignmentStatusEnums.REFUNDED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest,
                            MessageConstant.FailMessage.CanNotUpdateStatus);
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                //_unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<AssignmentResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateStatusSuccess,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<AssignmentResponse>> ReviewerCompletedBooking(int bookingId)
        {
            var result = new OperationResult<AssignmentResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingDetail);
                    return result;
                }

                //var booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)bookingDetail.BookingId);
                //if (booking == null)
                //{
                //    result.AddError(StatusCode.NotFound, "Booking not found.");
                //    return result;
                //}
                string nextStatus = bookingDetail.Status;

                switch (bookingDetail.Status)
                {
                    case var status when status == AssignmentStatusEnums.ASSIGNED.ToString():
                        nextStatus = AssignmentStatusEnums.REVIEWED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest,
                            MessageConstant.FailMessage.CanNotUpdateStatus);
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                //_unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<AssignmentResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateStatusSuccess,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<BookingResponse>> UserConfirmRoundTrip(int bookingId)
        {
            var result = new OperationResult<BookingResponse>();

            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                booking.IsUserConfirm = true;
                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<BookingResponse>(booking);
                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UserConfirm, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirm round trip");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<BookingResponse>> UpdateBookingByBookingIdAsync(int id,
            BookingServiceDetailsUpdateRequest request)
        {
            var result = new OperationResult<BookingResponse>();
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
                return result;
            }

            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id, "Assignments");
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                if (booking.Assignments.Count == 0)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                if (booking.Status == BookingEnums.CANCEL.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCancel);
                    return result;
                }

                if (booking.Status != BookingEnums.REVIEWING.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingReviewing);
                    return result;
                }

                var reviewer = booking.Assignments.FirstOrDefault(a =>
                    a.StaffType == RoleEnums.REVIEWER.ToString() && a.BookingId == id);
                if (reviewer == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                if (reviewer.Status == AssignmentStatusEnums.ASSIGNED.ToString() &&
                   booking.IsReviewOnline == true)
                {
                    reviewer.Status = AssignmentStatusEnums.SUGGESTED.ToString();
                }
                else if (reviewer.Status == AssignmentStatusEnums.ARRIVED.ToString() &&
                         booking.IsReviewOnline == false)
                {
                    reviewer.Status = AssignmentStatusEnums.SUGGESTED.ToString();
                }

                else if (reviewer.Status != AssignmentStatusEnums.SUGGESTED.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignmentSuggeted);
                    return result;
                }

                return await UpdateBookingAsync(reviewer.Id, request);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<BookingResponse>> UpdateBookingAsync(int assignmentId,
            BookingServiceDetailsUpdateRequest request, bool isDriverUpdate = false)
        {
            var result = new OperationResult<BookingResponse>();
            try
            {
                var bookingDetail = await _unitOfWork.AssignmentsRepository.GetByIdAsync(assignmentId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                // Fetch existing booking from the database
                var existingBooking = await _unitOfWork.BookingRepository
                    .GetAsync(b => b.Id == (int)bookingDetail.BookingId,
                        include: b =>
                            b.Include(b => b.BookingDetails).Include(b => b.FeeDetails).Include(b => b.Vouchers));

                if (existingBooking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }
                ReflectionUtils.UpdateProperties(request, existingBooking);

                // Update the updated date
                existingBooking.UpdatedAt = DateTime.Now;
                var total = 0.0;
                var truckCategoryId = request.TruckCategoryId.HasValue
                    ? request.TruckCategoryId.Value
                    : existingBooking.TruckNumber.Value;

                double voucherTotal = 0.0;

                if (request.BookingDetails.Count == 0)
                {
                    voucherTotal = (double)existingBooking.Vouchers.Sum(v => v.Price);
                }
                
                // Handle Service Details
                if (request.BookingDetails != null && request.BookingDetails.Any())
                {
                    var (sumServices, listBookingDetails, driverNo, porterNo, feeServiceDetails) =
                        await CalculateServiceFees(request.BookingDetails,
                            (int)existingBooking.HouseTypeId, truckCategoryId
                            , existingBooking.FloorsNumber,
                            existingBooking.EstimatedDistance);
                    await CheckBookingDetailExist((int)bookingDetail.BookingId, existingBooking.BookingDetails.ToList(),
                        listBookingDetails);
                }

                // Calculate total services and fees
                var houseTypeId = (int)existingBooking.HouseTypeId;
                //var truckCategoryId = (int)existingBooking.TruckNumber;
                var floorsNumber = existingBooking.FloorsNumber;
                var estimatedDistance = existingBooking.EstimatedDistance;

                List<BookingDetailRequest> newBookingDetails =
                    _mapper.Map<List<BookingDetailRequest>>(existingBooking.BookingDetails);

                var (totalServices, bookingDetails, driverNumber, porterNumber, feeDetails) =
                    await CalculateServiceFees(
                        newBookingDetails,
                        houseTypeId,
                        truckCategoryId,
                        floorsNumber,
                        estimatedDistance);

                //var existingServiceTotal = existingBooking.ServiceDetails.Sum(f => f.Price);
                total += (double)totalServices;
                await CheckBookingDetailExist((int)bookingDetail.BookingId, existingBooking.BookingDetails.ToList(),
                    bookingDetails);

                existingBooking.DriverNumber = driverNumber;
                existingBooking.PorterNumber = porterNumber;
                existingBooking.TruckNumber = truckCategoryId;
                //fee detail
                _unitOfWork.FeeDetailRepository.RemoveRange(existingBooking.FeeDetails.ToList());
                var (totalFee, feeCommonDetails) = await CalculateAndAddFees((DateTime)existingBooking.BookingAt);

                if (request.IsRoundTrip == true)
                {
                    (double updatedTotal, List<FeeDetail> updatedFeeDetails) = await ApplyPercentFeesAsync(total);
                    total += updatedTotal;
                    feeCommonDetails.AddRange(updatedFeeDetails);
                }

                var isHoliday = await _unitOfWork.HolidaySettingRepository.IsHolidayAsync(existingBooking.BookingAt.Value);
                if (isHoliday)
                {

                    (double updatedTotal, List<FeeDetail> updatedFeeDetails) =
                        await ApplyPercentHolidayFeesAsync(total);
                    total += updatedTotal;
                    feeCommonDetails.AddRange(updatedFeeDetails);

                }

                existingBooking.FeeDetails = feeCommonDetails;

                total -= voucherTotal;
                
                total += (double)totalFee;

                var deposit = total * 30 / 100;

                if (existingBooking.IsReviewOnline == false)
                {
                    var feeReviewerOffline = await _unitOfWork.FeeSettingRepository.GetReviewerFeeSettingsAsync();
                    deposit = feeReviewerOffline!.Amount!.Value;
                }
                existingBooking.TotalFee = totalFee;
                
                // Ensure total includes service and fee totals
                if (isDriverUpdate)
                {
                    existingBooking.TotalReal = total - existingBooking.Deposit;
                    existingBooking.Status = BookingEnums.CONFIRMED.ToString();
                    existingBooking.Deposit = existingBooking.Total * 0.30;
                }
                else
                {
                    existingBooking.Total = total;
                    existingBooking.Deposit = existingBooking.Total * 0.30;
                    existingBooking.TotalReal = total;
                }
                existingBooking.Total = total;


                // Update booking type based on timing
                DateTime now = DateTime.Now;
                existingBooking.TypeBooking = ((request.UpdatedAt - now).TotalHours <= 3 &&
                                               (request.UpdatedAt - now).TotalHours >= 0)
                    ? TypeBookingEnums.NOW.ToString()
                    : TypeBookingEnums.DELAY.ToString();

                // logic booking detail

                await _unitOfWork.AssignmentsRepository.SaveOrUpdateAsync(bookingDetail);

                await _unitOfWork.BookingDetailRepository.SaveOrUpdateRangeAsync(
                    existingBooking.BookingDetails.ToList());

                await _unitOfWork.BookingRepository.SaveOrUpdateAsync(existingBooking);

                var bookingDetailsWithZeroQuantity = existingBooking.BookingDetails
                    .Where(bd => bd.Quantity == 0)
                    .AsEnumerable()
                    .ToList();

                foreach (var id in bookingDetailsWithZeroQuantity)
                {
                    var bookingRemove = await _unitOfWork.BookingDetailRepository.GetByIdAsync(id.Id);
                    if (bookingRemove != null)
                    {
                        _unitOfWork.BookingDetailRepository.Remove(bookingRemove);
                    }
                }

                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    existingBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1((int)bookingDetail.BookingId,
                        includeProperties:
                        "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments,Vouchers");
                    var response = _mapper.Map<BookingResponse>(existingBooking);
                    await _firebaseServices.SaveBooking(existingBooking, existingBooking.Id, "bookings");
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, $"An error occurred: {ex.Message}");
            }

            return result;
        }


        private async Task<List<BookingDetail>> CheckBookingDetailExist(int bookingId,
            List<BookingDetail> existBookingDetails, List<BookingDetail> newBookingDetails)
        {
            List<BookingDetail> bookingDetails = existBookingDetails;

            var existingServiceDict = existBookingDetails.ToDictionary(sd => sd.ServiceId);
            foreach (var requestService in newBookingDetails)
            {
                var service = await _unitOfWork.ServiceRepository.GetByIdAsync((int)requestService.ServiceId);
                if (existingServiceDict.TryGetValue(requestService.ServiceId, out var existingBookingDetail))
                {
                    // Update existing service detail properties
                    existingBookingDetail.Quantity = requestService.Quantity;
                    existingBookingDetail.Price = requestService.Price;
                    existingBookingDetail.Name = service.Name;
                    existingBookingDetail.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
                    existingBookingDetail.Description = service.Description;
                    existingBookingDetail.Type = service.Type;
                    existingBookingDetail.BookingId = bookingId;
                    bookingDetails.Add(existingBookingDetail);
                }
                else
                {
                    // Create new ServiceDetail with Amount from the Service
                    var newBookingDetail = new BookingDetail
                    {
                        ServiceId = requestService.ServiceId,
                        BookingId = bookingId,
                        Quantity = requestService.Quantity,
                        Price = requestService.Price,
                        Name = service.Name,
                        Status = BookingDetailStatusEnums.AVAILABLE.ToString(),
                        Description = service.Description,
                        Type = service.Type
                    };

                    bookingDetails.Add(newBookingDetail);
                }
            }

            return bookingDetails;
        }


        public async Task<OperationResult<BookingResponse>> ReviewChangeReviewAt(int bookingId, ReviewAtRequest request)
        {
            var result = new OperationResult<BookingResponse>();
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
                return result;
            }

            try
            {
                if (!request.IsReviewAtValid())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.IsValidTimeGreaterNow);
                    return result;
                }

                // Retrieve the existing booking by ID
                var existingBooking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, includeProperties: "Assignments");

                if (existingBooking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                if (existingBooking.IsReviewOnline == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingReviewOnline);
                    return result;
                }

                if (existingBooking.Status == BookingEnums.CANCEL.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCancel);
                    return result;
                }

                if (existingBooking.Status == BookingEnums.ASSIGNED.ToString())
                {
                    existingBooking.Status = BookingEnums.WAITING.ToString();
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingAssigned);
                    return result;
                }

                existingBooking.ReviewAt = request.ReviewAt;
                existingBooking.UpdatedAt = DateTime.Now;

                _unitOfWork.BookingRepository.Update(existingBooking);
                var saveResult = await _unitOfWork.SaveChangesAsync();

                if (saveResult > 0)
                {
                    var updatedBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1(bookingId,
                        includeProperties:
                        "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");
                    var response = _mapper.Map<BookingResponse>(updatedBooking);
                    await _firebaseServices.SaveBooking(existingBooking, existingBooking.Id, "bookings");
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, $"An error occurred: {ex.Message}");
            }

            return result;
        }

        public async Task<OperationResult<BookingResponse>> UserConfirm(int bookingId, int userId,
            StatusRequest request)
        {
            var result = new OperationResult<BookingResponse>();
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
                return result;
            }

            if (!request.AreVouchersUnique())
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherUnique);
                return result;
            }

            try
            {
                var assigment =
                    _unitOfWork.AssignmentsRepository.GetByStaffTypeAndBookingId(RoleEnums.REVIEWER.ToString(),
                        bookingId);
                if (assigment == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                var existingBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1(bookingId,
                    includeProperties: "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");

                if (existingBooking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                if (existingBooking.UserId != userId)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCannotPay);
                    return result;
                }


                if (existingBooking.Status == BookingEnums.CANCEL.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCancel);
                    return result;
                }

                switch (request.Status)
                {
                    case "ASSIGNED":
                        if (existingBooking.Status != BookingEnums.WAITING.ToString())
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingWaiting);
                            return result;
                        }

                        existingBooking.Status = BookingEnums.ASSIGNED.ToString();
                        break;

                    case "DEPOSITING":
                        if (existingBooking.Status != BookingEnums.WAITING.ToString() &&
                            existingBooking.IsReviewOnline == false)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingWaiting);
                            return result;
                        }

                        if (existingBooking.Status != BookingEnums.REVIEWED.ToString() &&
                            existingBooking.IsReviewOnline == true)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingReviewed);
                            return result;
                        }

                        /*if (existingBooking.IsReviewOnline == true)
                        {
                            var assignmentStatus = existingBooking.Assignments.FirstOrDefault(a =>
                                a.Status == AssignmentStatusEnums.SUGGESTED.ToString() &&
                                a.StaffType == RoleEnums.REVIEWER.ToString());
                            assignmentStatus!.Status = AssignmentStatusEnums.REVIEWED.ToString();
                            _unitOfWork.AssignmentsRepository.Update(assignmentStatus);
                        }*/
                        if (existingBooking.IsReviewOnline == false)
                        {
                            _producer.SendingMessage("movemate.setup_schedule_review", existingBooking.Id);
                        }

                        existingBooking.Status = BookingEnums.DEPOSITING.ToString();
                        var userOnlineVouchers = await _unitOfWork.VoucherRepository.GetUserVouchersAsync(userId);
                        var validOnlineVoucherIds = userOnlineVouchers.Select(v => v.Id).ToHashSet();

                        // Validate requested vouchers
                        var requestedVouchers =
                            request.Vouchers.Where(v => validOnlineVoucherIds.Contains(v.Id)).ToList();

                        if (requestedVouchers.Count != request.Vouchers.Count)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherNotUser);
                            return result;
                        }
                        //validate many promotions


                        var serviceOnlineIds = existingBooking.BookingDetails.Select(bd => bd.ServiceId).Distinct()
                            .ToList();
                        var validOnlinePromotionIds = new List<int>();

                        foreach (var serviceId in serviceOnlineIds)
                        {
                            var promotionId =
                                await _unitOfWork.PromotionCategoryRepository.GetPromotionIdByServiceId((int)serviceId);
                            if (promotionId.HasValue)
                            {
                                validOnlinePromotionIds.Add(promotionId.Value);
                            }
                        }

                        // Validate that each voucher's PromotionId matches a promotion for one of the services
                        foreach (var voucher in requestedVouchers)
                        {
                            if (!validOnlinePromotionIds.Contains(voucher.PromotionCategoryId))
                            {
                                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherNotMatch);
                                return result;
                            }
                        }

                        var voucherOnlines = new List<Voucher>();
                        double totalVoucherOnlinePrice = 0;

                        // Instead of mapping DTO to entity directly, retrieve existing vouchers
                        foreach (var voucher in requestedVouchers)
                        {
                            // Assuming you have a method to get the existing voucher by Id
                            var existingVoucher = await _unitOfWork.VoucherRepository.GetByIdAsync(voucher.Id);

                            if (existingVoucher != null)
                            {
                                // Attach the existing voucher to the booking
                                voucherOnlines.Add(existingVoucher);
                                totalVoucherOnlinePrice += existingVoucher.Price.Value;
                                //existingVoucher.IsActived = false;
                            }
                            else
                            {
                                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundVoucher);
                                return result;
                            }
                        }

                        existingBooking.Total -= totalVoucherOnlinePrice;
                        existingBooking.TotalReal = existingBooking.Total;
                        var deposit = existingBooking.Total * 30 / 100;
                        existingBooking.Deposit = deposit;
                        foreach (var voucher in voucherOnlines)
                        {
                            voucher.BookingId = existingBooking.Id;
                        }

                        _unitOfWork.VoucherRepository.UpdateRange(voucherOnlines);
                        BackgroundJob.Schedule(() => CheckAndCancelBooking(existingBooking.Id), TimeSpan.FromDays(1));
                        break;
                    case "COMING":
                        if (existingBooking.Status != BookingEnums.REVIEWED.ToString())
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingReviewed);
                            return result;
                        }

                        existingBooking.Status = BookingEnums.COMING.ToString();
                        var userVouchers = await _unitOfWork.VoucherRepository.GetUserVouchersAsync(userId);
                        var validVoucherIds = userVouchers.Select(v => v.Id).ToHashSet();

                        // Validate requested vouchers
                        var requestedOffVouchers = request.Vouchers.Where(v => validVoucherIds.Contains(v.Id)).ToList();

                        if (requestedOffVouchers.Count != request.Vouchers.Count)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherNotUser);
                            return result;
                        }
                        //validate many promotions


                        var serviceIds = existingBooking.BookingDetails.Select(bd => bd.ServiceId).Distinct().ToList();
                        var validPromotionIds = new List<int>();

                        foreach (var serviceId in serviceIds)
                        {
                            var promotionId =
                                await _unitOfWork.PromotionCategoryRepository.GetPromotionIdByServiceId((int)serviceId);
                            if (promotionId.HasValue)
                            {
                                validPromotionIds.Add(promotionId.Value);
                            }
                        }

                        // Validate that each voucher's PromotionId matches a promotion for one of the services
                        foreach (var voucher in requestedOffVouchers)
                        {
                            if (!validPromotionIds.Contains(voucher.PromotionCategoryId))
                            {
                                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherNotMatch);
                                return result;
                            }
                        }

                        var vouchers = new List<Voucher>();
                        double totalVoucherPrice = 0;

                        // Instead of mapping DTO to entity directly, retrieve existing vouchers
                        foreach (var voucher in requestedOffVouchers)
                        {
                            // Assuming you have a method to get the existing voucher by Id
                            var existingVoucher = await _unitOfWork.VoucherRepository.GetByIdAsync(voucher.Id);

                            if (existingVoucher != null)
                            {
                                // Attach the existing voucher to the booking
                                vouchers.Add(existingVoucher);
                                totalVoucherPrice += existingVoucher.Price.Value;
                                //existingVoucher.IsActived = false;
                            }
                            else
                            {
                                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundVoucher);
                                return result;
                            }
                        }

                        existingBooking.Total -= totalVoucherPrice;
                        existingBooking.TotalReal = existingBooking.Total;
                        var depositOff = existingBooking.Total * 30 / 100;
                        existingBooking.Deposit = depositOff;
                        foreach (var voucher in vouchers)
                        {
                            voucher.BookingId = existingBooking.Id;
                        }

                        _unitOfWork.VoucherRepository.UpdateRange(vouchers);
                        break;
                    case "IN_PROGRESS":
                        if (existingBooking.Status != BookingEnums.CONFIRMED.ToString())
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingConfirmed);
                            return result;
                        }

                        existingBooking.Status = BookingEnums.IN_PROGRESS.ToString();
                        break;

                    default:
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidStatus);
                        return result;
                }

                existingBooking.UpdatedAt = DateTime.Now;
                _unitOfWork.BookingRepository.Update(existingBooking);
                var saveResult = await _unitOfWork.SaveChangesAsync();

                if (saveResult > 0)
                {
                    var updatedBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1(bookingId,
                        includeProperties:
                        "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments,Vouchers");
                    var response = _mapper.Map<BookingResponse>(updatedBooking);
                    await _firebaseServices.SaveBooking(existingBooking, existingBooking.Id, "bookings");
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, $"An error occurred: {ex.Message}");
            }

            return result;
        }

        public async Task<OperationResult<AssignmentResponse>> AssignedLeader(int userId, int assignmentId)
        {
            var result = new OperationResult<AssignmentResponse>();
            try
            {


                var assignment = await _unitOfWork.AssignmentsRepository.GetByIdAsync(assignmentId);
                if (assignment == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }

                var reviewer = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndBookingId(RoleEnums.REVIEWER.ToString(), (int)assignment.BookingId);
                if (reviewer.UserId != userId)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignWrongReview);
                    return result;
                }

                if (assignment.Status != AssignmentStatusEnums.WAITING.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignmentWaiting);
                    return result;
                }
                //Assign lead
                var checkLeader =
                    _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(assignment.StaffType,
                        (int)assignment.BookingId);
                if (checkLeader != null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignedLeader);
                    return result;
                }

                assignment.IsResponsible = true;
                assignment.Status = AssignmentStatusEnums.ASSIGNED.ToString();
                //Change status Assignment for other ASSIGNED
                var assigned = await _unitOfWork.AssignmentsRepository.GetByStaffTypeAsync(assignment.StaffType, (int)assignment.BookingId, assignmentId);
                foreach (var assign in assigned)
                {
                    assign.Status = AssignmentStatusEnums.ASSIGNED.ToString();
                }

                await _unitOfWork.AssignmentsRepository.SaveOrUpdateAsync(assignment);
                await _unitOfWork.AssignmentsRepository.SaveOrUpdateRangeAsync(assigned);
                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    assignment = await _unitOfWork.AssignmentsRepository.GetByIdAsync(assignment.Id);
                    var response = _mapper.Map<AssignmentResponse>(assignment);
                    var entity = await _unitOfWork.BookingRepository.GetByIdAsync((int)assignment.BookingId, includeProperties: "Assignments");
                    await _firebaseServices.SaveBooking(entity, entity.Id, "bookings");

                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateAssignment,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignmentUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }

            return result;
        }

        public async Task<OperationResult<BookingResponse>> UserChangeBooingAt(int booingId, int userId,
            ChangeBookingAtRequest request)
        {
            var result = new OperationResult<BookingResponse>();

            if (!request.IsBookingAtValid())
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.IsValidTimeGreaterNow);
                return result;
            }

            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
                return result;
            }

            try
            {
                var existingBooking =
                    await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(booingId, userId);

                if (existingBooking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.BookingCannotPay);
                    return result;
                }

                if (existingBooking.Status == BookingEnums.CANCEL.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCancel);
                    return result;
                }

                if (existingBooking.IsUpdated == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingHasBeenUpdated);
                    return result;
                }

                if (existingBooking.IsReviewOnline == true)
                {
                    if (existingBooking.Status == BookingEnums.PENDING.ToString() ||
                        existingBooking.Status == BookingEnums.ASSIGNED.ToString()
                        || existingBooking.Status == BookingEnums.REVIEWING.ToString() ||
                        existingBooking.Status == BookingEnums.REVIEWED.ToString())
                    {
                        existingBooking.BookingAt = request.BookingAt;
                        existingBooking.IsUpdated = true;
                        existingBooking.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ChangeBookingAtFail);
                        return result;
                    }
                }

                if (existingBooking.IsReviewOnline == false)
                {
                    if (existingBooking.Status == BookingEnums.PENDING.ToString() ||
                        existingBooking.Status == BookingEnums.ASSIGNED.ToString()
                        || existingBooking.Status == BookingEnums.WAITING.ToString() ||
                        existingBooking.Status == BookingEnums.DEPOSITING.ToString())
                    {
                        existingBooking.BookingAt = request.BookingAt;
                        existingBooking.IsUpdated = true;
                        existingBooking.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ChangeBookingAtFail);
                        return result;
                    }
                }

                await _unitOfWork.BookingRepository.SaveOrUpdateAsync(existingBooking);
                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    existingBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1((int)existingBooking.Id,
                        includeProperties:
                        "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments,Vouchers");
                    var response = _mapper.Map<BookingResponse>(existingBooking);
                    await _firebaseServices.SaveBooking(existingBooking, existingBooking.Id, "bookings");
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }

            return result;
        }

        public async Task<OperationResult<BookingDetailsResponse>> StaffReportFail(int bookingDetailId, int userId, FailReportRequest request)
        {
            var result = new OperationResult<BookingDetailsResponse>();
            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingDetailId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingDetail);
                    return result;
                }

                var checkLeader = await _unitOfWork.AssignmentsRepository.GetByUserIdAndStaffTypeAndIsResponsible(userId, RoleEnums.PORTER.ToString(), (int)bookingDetail.BookingId);
                if (checkLeader == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.AssignedLeader);
                    return result;
                }
                var staffLeader = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (staffLeader == null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)bookingDetail.BookingId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }



                if (booking.Status != BookingEnums.IN_PROGRESS.ToString() && (checkLeader.Status != AssignmentStatusEnums.IN_PROGRESS.ToString()
                    || checkLeader.Status != AssignmentStatusEnums.ONGOING.ToString()
                    || checkLeader.Status != AssignmentStatusEnums.DELIVERED.ToString()
                    || checkLeader.Status != AssignmentStatusEnums.UNLOAD.ToString()))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateTimeNotAllowed);
                    return result;
                }

                bookingDetail.Status = request.Status;
                bookingDetail.FailReason = request.FailReason;

                await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetail);
                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync((int)bookingDetail.Id);
                    var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                    var user = await _unitOfWork.UserRepository.GetManagerAsync();

                    var notification = new Notification
                    {

                        UserId = user.Id,
                        SentFrom = staffLeader.Name,
                        Receive = user.Name,
                        Name = "Service Fail Notification",
                        Description = $"Booking with id {booking.Id} have some problem {request.FailReason}",
                        Topic = "StaffReportFail",
                        IsRead = false
                    };
                    await _unitOfWork.NotificationRepository.AddAsync(notification);
                    await _unitOfWork.SaveChangesAsync();
                    // Save the notification to Firestore
                    await _firebaseServices.SaveMailManager(notification, notification.Id, "reports");
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingDetailUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }

            return result;
        }

        public async Task<OperationResult<BookingDetailsResponse>> ManagerFix(int bookingDetailId, int userId)
        {
            var result = new OperationResult<BookingDetailsResponse>();
            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingDetailId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingDetail);
                    return result;
                }

                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }
                if (user.RoleId != 6 || user.RoleId != 1)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotManager);
                    return result;
                }
                if (bookingDetail.Status != BookingDetailStatusEnums.WAITING.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CanNotFix);
                    return result;
                }

                bookingDetail.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
                await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetail);
                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync((int)bookingDetail.Id);
                    var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);

                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingDetailUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }

            return result;
        }

        public async Task<OperationResult<BookingResponse>> TrackerReport(int userId, int bookingId, TrackerSourceRequest request)
        {
            var result = new OperationResult<BookingResponse>();
            try
            {
                // Retrieve the booking with related properties
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);

                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                var notificationUser =
                        await _unitOfWork.NotificationRepository.GetByUserIdAsync((int)booking.UserId);

                var staffLeader = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (staffLeader == null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }

                var checkLeader = await _unitOfWork.AssignmentsRepository.GetByUserIdAndIsResponsible(userId, bookingId);
                if (checkLeader == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.AssignedLeader);
                    return result;
                }

                if (booking.Status != BookingEnums.IN_PROGRESS.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateTimeNotAllowed);
                    return result;
                }

                // Check if the leader's status allows updates
                if (booking.Status != BookingEnums.IN_PROGRESS.ToString() &&
                    (checkLeader.Status != AssignmentStatusEnums.IN_PROGRESS.ToString()
                    || checkLeader.Status != AssignmentStatusEnums.ONGOING.ToString()
                    || checkLeader.Status != AssignmentStatusEnums.DELIVERED.ToString()
                    || checkLeader.Status != AssignmentStatusEnums.UNLOAD.ToString()))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateTimeNotAllowed);
                    return result;
                }

                // Create a new booking tracker
                var tracker = new BookingTracker
                {
                    BookingId = booking.Id,
                    Type = TrackerEnums.REPORT.ToString(),
                    Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss"),
                    TrackerSources = _mapper.Map<List<TrackerSource>>(request.ResourceList)
                };
                
                await _unitOfWork.BookingTrackerRepository.AddAsync(tracker);
                await _unitOfWork.BookingRepository.SaveOrUpdateAsync(booking);

                var user = await _unitOfWork.UserRepository.GetManagerAsync();
                var notification = new MoveMate.Domain.Models.Notification
                {

                    UserId = user.Id,
                    SentFrom = staffLeader.Name,
                    Receive = user.Name,
                    Name = "Report Damage Items Notification",
                    Description = $"Items damaged during transportation in booking {booking.Id}",
                    Topic = "TrackerReport",
                    IsRead = false
                };
                await _unitOfWork.NotificationRepository.AddAsync(notification);


                var saveResult = _unitOfWork.Save();

                if (saveResult > 0)
                {
                    booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)booking.Id,
                        includeProperties: "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments,Vouchers");

                    var response = _mapper.Map<BookingResponse>(booking);

                    // Save the notification to Firestore
                    _firebaseServices.SaveMailManager(notification, notification.Id, "reports");

                    var title = "Báo cáo hư hại: Đã phân công trách nhiệm";
                    var body = $"Thông báo: Đã phân công trách nhiệm cho việc hư hại đồ vật trong quá trình vận chuyển hoặc đóng gói cho đơn hàng {booking.Id}.";
                    var fcmToken = notificationUser.FcmToken;
                    var data = new Dictionary<string, string>
                    {
                        { "bookingId", booking.Id.ToString() },
                        { "status", booking.Status.ToString() },
                        { "message", "Đã phân công trách nhiệm cho việc hư hại." }
                    };

                    // Gửi thông báo đến Firebase
                    await _firebaseServices.SendNotificationAsync(title, body, fcmToken, data);


                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingUpdateSuccess, response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }

            return result;
        }


        public async Task<OperationResult<BookingResponse>> UpdateLimitedBookingAsync(int userId, int bookingId, DriverUpdateBookingRequest request)
        {
            var result = new OperationResult<BookingResponse>();
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, "Assignments");
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                var assignment = await _unitOfWork.AssignmentsRepository.GetByUserIdAndStaffTypeAndIsResponsible(userId, RoleEnums.DRIVER.ToString(), bookingId);
                if (assignment == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }



                if (booking.Status == BookingEnums.CANCEL.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCancel);
                    return result;
                }

                var limitedUpdateRequest = new BookingServiceDetailsUpdateRequest
                {
                    TruckCategoryId = request.TruckCategoryId,
                    BookingDetails = request.BookingDetails ?? new List<BookingDetailRequest>()
                };


                return await UpdateBookingAsync(assignment.Id, limitedUpdateRequest, isDriverUpdate: true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }


        }

        public async Task<OperationResult<BookingResponse>> DriverUpdateBooking(int userId, int bookingId, DriverUpdateBookingRequest request)
        {
            var result = new OperationResult<BookingResponse>();
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, "Assignments");
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                var assignment = await _unitOfWork.AssignmentsRepository.GetByUserIdAndStaffTypeAndIsResponsible(userId, RoleEnums.DRIVER.ToString(), bookingId);
                if (assignment == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }



                if (booking.Status == BookingEnums.CANCEL.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCancel);
                    return result;
                }

                var limitedUpdateRequest = new BookingServiceDetailsUpdateRequest
                {
                    TruckCategoryId = request.TruckCategoryId,
                    BookingDetails = request.BookingDetails ?? new List<BookingDetailRequest>()
                };


                return await UpdateBookingAsync(assignment.Id, limitedUpdateRequest, isDriverUpdate: true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }


        }

        public async Task<OperationResult<BookingResponse>> PorterUpdateBooking(int userId, int bookingId, PorterUpdateDriverRequest request)
        {
            var result = new OperationResult<BookingResponse>();
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, "Assignments");
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return result;
                }

                var assignment = await _unitOfWork.AssignmentsRepository.GetByUserIdAndStaffTypeAndIsResponsible(userId, RoleEnums.PORTER.ToString(), bookingId);
                if (assignment == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                    return result;
                }



                if (booking.Status == BookingEnums.CANCEL.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingCancel);
                    return result;
                }

                var limitedUpdateRequest = new BookingServiceDetailsUpdateRequest
                {
                    BookingDetails = request.BookingDetails ?? new List<BookingDetailRequest>()
                };


                return await UpdateBookingAsync(assignment.Id, limitedUpdateRequest, isDriverUpdate: true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }
    }
}