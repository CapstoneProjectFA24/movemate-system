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

namespace MoveMate.Service.Services
{
    public class BookingServices : IBookingServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<BookingServices> _logger;
        private readonly IMessageProducer _producer;
        private readonly IFirebaseServices _firebaseServices;

        public BookingServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookingServices> logger,
            IMessageProducer producer, IFirebaseServices firebaseServices)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
            _producer = producer;
            _firebaseServices = firebaseServices;
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
                var entities = _unitOfWork.BookingRepository.Get(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder(),
                    includeProperties: "BookingDetails,FeeDetails,BookingTrackers.TrackerSources,ServiceDetails"
                );
                var listResponse = _mapper.Map<List<BookingResponse>>(entities);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "List Booking is Empty!", listResponse);
                    return result;
                }

                pagin.PageSize = request.per_page;
                pagin.TotalItemsCount = listResponse.Count();

                result.AddResponseStatusCode(StatusCode.Ok, "Get List Auctions Done.", listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
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
                    await _unitOfWork.BookingRepository.GetByIdAsyncV1(id,
                        includeProperties: "BookingTrackers.TrackerSources,BookingDetails,FeeDetails,ServiceDetails");

                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Booking with Id: {id}");
                }
                else
                {
                    var productResponse = _mapper.Map<BookingResponse>(booking);
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get Booking by Id: {id} Success!", productResponse);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Get Service By Id service method for ID: {id}");
                throw;
            }
        }

        #endregion

        // REGISTER

        #region FEATURE: Register a new booking in the system.

        public async Task<OperationResult<BookingResponse>> RegisterBooking(BookingRegisterRequest request)
        {
            var result = new OperationResult<BookingResponse>();
            string status = BookingEnums.PENDING.ToString();

            if (!request.IsBookingAtValid())
            {
                result.AddError(StatusCode.BadRequest,
                    $"BookingAt is not null and whether the value is greater than or equal to the current time.");
                return result;
            }

            try
            {
                var existingHouseType =
                    await _unitOfWork.HouseTypeRepository.GetByIdAsyncV1(request.HouseTypeId, "HouseTypeSettings");

                // check houseType
                if (existingHouseType == null)
                {
                    result.AddError(StatusCode.NotFound, $"HouseType with id: {request.HouseTypeId} not found!");
                    return result;
                }

                // setting var
                var serviceDetails = new List<ServiceDetail>();
                var feeDetails = new List<FeeDetail>();
                double total = 0;

                // mapping dto to entity
                var entity = _mapper.Map<Booking>(request);

                // logic services and fee set amount
                var (totalServices, listServiceDetails) = await CalculateServiceFees(request.ServiceDetails,
                    request.HouseTypeId,
                    request.TruckCategoryId, request.FloorsNumber, request.EstimatedDistance);

                total += totalServices;
                serviceDetails.AddRange(listServiceDetails);

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

                // resource logic 

                var tracker = new BookingTracker();
                tracker.Type = TrackerEnums.PENDING.ToString();
                tracker.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

                List<TrackerSource> resourceList = _mapper.Map<List<TrackerSource>>(request.ResourceList);
                tracker.TrackerSources = resourceList;

                // save
                entity.Status = status;

                entity.BookingTrackers.Add(tracker);
                entity.ServiceDetails = serviceDetails;
                entity.FeeDetails = feeDetails;

                entity.TotalFee = totalFee;
                entity.TotalReal = total;
                entity.Total = total;

                entity.TypeBooking = TypeBookingEnums.NOW.ToString();

                await _unitOfWork.BookingRepository.AddAsync(entity);
                var checkResult = _unitOfWork.Save();

                // create a job check booking time, if now = bookingtime and status still APPROVED then change Stastus to CANCEL
                // logic 

                if (checkResult > 0)
                {
                    BackgroundJob.Schedule(() => CheckAndCancelBooking(entity.Id), entity.BookingAt ?? DateTime.Now);
                    var response = _mapper.Map<BookingResponse>(entity);

                    _producer.SendingMessage("movemate.booking_assign_review_local", entity.Id);
                    _firebaseServices.SaveBooking(entity, entity.Id, "bookings");
                    result.AddResponseStatusCode(StatusCode.Created, "Add Booking Success!", response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add Booking Failed!");
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

        public async Task<OperationResult<BookingValuationResponse>> ValuationBooking(BookingValuationRequest request)
        {
            var result = new OperationResult<BookingValuationResponse>();

            var existingHouseType =
                await _unitOfWork.HouseTypeRepository.GetByIdAsyncV1(request.HouseTypeId, "HouseTypeSettings");

            // check houseType
            if (existingHouseType == null)
            {
                result.AddError(StatusCode.NotFound, $"HouseType with id: {request.HouseTypeId} not found!");
                return result;
            }

            double totalFee = 0;
            var serviceDetails = new List<ServiceDetail>();
            var feeDetails = new List<FeeDetail>();

            try
            {
                var (totalServices, listServiceDetails) = await CalculateServiceFees(request.ServiceDetails,
                    request.HouseTypeId,
                    request.TruckCategoryId, request.FloorsNumber, request.EstimatedDistance);
                totalFee += totalServices;
                serviceDetails.AddRange(listServiceDetails);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            var response = new BookingValuationResponse();

            response.Amount = totalFee;
            response.ServiceDetails = _mapper.Map<List<ServiceDetailsResponse>>(serviceDetails);

            result.AddResponseStatusCode(StatusCode.Ok, "valuation!", response);

            return result;
        }

        #endregion

        // CANCEL

        #region FEATURE: Cancel a booking in the system.

        public async Task<OperationResult<BookingResponse>> CancelBooking(BookingCancelRequest request)
        {
            var result = new OperationResult<BookingResponse>();

            // 
            try
            {
                var entity = await _unitOfWork.BookingRepository.GetByIdAsync(request.Id);
                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, $"booking with id: {request.Id} not found!");
                    return result;
                }

                entity.Status = BookingEnums.CANCEL.ToString();
                entity.IsCancel = true;
                entity.CancelReason = request.CancelReason;
                _unitOfWork.BookingRepository.Update(entity);
                _unitOfWork.Save();

                //
                var response = _mapper.Map<BookingResponse>(entity);

                result.AddResponseStatusCode(StatusCode.Ok, "Cancel Success!", response);

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

        #region TEST: ValuationDistanceBooking for a new booking in the system.

        public async Task<OperationResult<BookingValuationResponse>> ValuationDistanceBooking(
            BookingValuationRequest request)
        {
            var result = new OperationResult<BookingValuationResponse>();

            var existingHouseType =
                await _unitOfWork.HouseTypeRepository.GetByIdAsyncV1(request.HouseTypeId, "HouseTypeSettings");

            // check houseType
            if (existingHouseType == null)
            {
                result.AddError(StatusCode.NotFound, $"HouseType with id: {request.HouseTypeId} not found!");
                return result;
            }

            double totalFee = 0;

            foreach (var serviceDetailRequest in request.ServiceDetails)
            {
                var service =
                    await _unitOfWork.ServiceRepository.GetByIdAsyncV1(serviceDetailRequest.Id, "FeeSettings");

                if (service == null)
                {
                    result.AddError(StatusCode.NotFound, $"Service with id: {serviceDetailRequest.Id} not found!");
                    return result;
                }

                // Tạo `ServiceDetail`
                var quantity = serviceDetailRequest.Quantity;
                var price = service.Amount * quantity - service.Amount * quantity * service.DiscountRate / 100;

                // logic fee 
                var (nullUnitFees, kmUnitFees, floorUnitFees) =
                    SeparateFeeSettingsByUnit(service.FeeSettings.ToList(), request.HouseTypeId,
                        request.TruckCategoryId);

                var (totalTruckFee, feeTruckDetails) = CalculateDistanceFee(request.TruckCategoryId,
                    double.Parse(request.EstimatedDistance), kmUnitFees, quantity ?? 1);
                totalFee += totalTruckFee;
                //feeDetails.AddRange(feeTruckDetails);

                var (nullTotalFee, nullUnitFeeDetails) = CalculateBaseFee(nullUnitFees, quantity ?? 1);
                totalFee += nullTotalFee;
                //feeDetails.AddRange(nullUnitFeeDetails);

                totalFee = (double)(totalFee * quantity - totalFee * quantity * service.DiscountRate / 100);
            }

            var response = new BookingValuationResponse();

            response.Amount = totalFee;

            result.AddResponseStatusCode(StatusCode.Ok, "valuation!", response);

            return result;
        }

        #endregion

        #region TEST: ValuationFloorBooking for a new booking in the system.

        public async Task<OperationResult<BookingValuationResponse>> ValuationFloorBooking(
            BookingValuationRequest request)
        {
            var result = new OperationResult<BookingValuationResponse>();

            var existingHouseType =
                await _unitOfWork.HouseTypeRepository.GetByIdAsyncV1(request.HouseTypeId, "HouseTypeSettings");

            // check houseType
            if (existingHouseType == null)
            {
                result.AddError(StatusCode.NotFound, $"HouseType with id: {request.HouseTypeId} not found!");
                return result;
            }

            double totalFee = 0;

            foreach (var serviceDetailRequest in request.ServiceDetails)
            {
                var service =
                    await _unitOfWork.ServiceRepository.GetByIdAsyncV1(serviceDetailRequest.Id, "FeeSettings");

                if (service == null)
                {
                    result.AddError(StatusCode.NotFound, $"Service with id: {serviceDetailRequest.Id} not found!");
                    return result;
                }

                // Tạo `ServiceDetail`
                var quantity = serviceDetailRequest.Quantity;
                var price = service.Amount * quantity - service.Amount * quantity * service.DiscountRate / 100;

                // logic fee 
                var (nullUnitFees, kmUnitFees, floorUnitFees) =
                    SeparateFeeSettingsByUnit(service.FeeSettings.ToList(), request.HouseTypeId,
                        request.TruckCategoryId);

                var (floorTotalFee, floorUnitFeeDetails) = CalculateFloorFeeV2(request.TruckCategoryId,
                    int.Parse(request.FloorsNumber ?? "1"), floorUnitFees, quantity ?? 1);
                totalFee += floorTotalFee;
                //feeDetails.AddRange(floorUnitFeeDetails);

                var (nullTotalFee, nullUnitFeeDetails) = CalculateBaseFee(nullUnitFees, quantity ?? 1);
                totalFee += nullTotalFee;
                //feeDetails.AddRange(nullUnitFeeDetails);

                //totalFee = (double)(totalFee * quantity - totalFee * quantity * service.DiscountRate/100);
            }

            var response = new BookingValuationResponse();

            response.Amount = totalFee;

            result.AddResponseStatusCode(StatusCode.Ok, "valuation!", response);

            return result;
        }

        #endregion

        // CRONJOB

        #region CRONJOB: CheckAndCancelBooking a booking in the system.

        public async Task CheckAndCancelBooking(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);

            if (booking != null && booking.Status == BookingEnums.PENDING.ToString())
            {
                booking.Status = BookingEnums.CANCEL.ToString();
                booking.IsCancel = true;
                booking.CancelReason = "Is expired, Cancel by System";
                _unitOfWork.BookingRepository.Update(booking);
                _unitOfWork.Save();
                _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
            }
        }

        #endregion

        // PRIVATE

        #region PRIVATE: ApplyPercentFeesAsync for a new booking in the system.

        private async Task<(double updatedTotal, List<FeeDetail> feeDetails)> ApplyPercentFeesAsync(double total)
        {
            var feePercentSettings = await _unitOfWork.FeeSettingRepository.GetPercentFeeSettingsAsync();

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
                .Where(f => f.TruckCategoryId == truckCategoryId && f.Unit == "KM" && f.IsActived == true)
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

            feeSettings.AddRange(_unitOfWork.FeeSettingRepository.GetTruckFeeSettings(CateTruckId));

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

        private async Task<(double totalServices, List<ServiceDetail> serviceDetails)> CalculateServiceFees(
            List<ServiceDetailRequest> serviceDetailRequests,
            int houseTypeId,
            int truckCategoryId,
            string floorsNumber,
            string estimatedDistance)
        {
            double totalServices = 0;
            var serviceDetails = new List<ServiceDetail>();

            foreach (var serviceDetailRequest in serviceDetailRequests)
            {
                // Check Service
                var service =
                    await _unitOfWork.ServiceRepository.GetByIdAsyncV1(serviceDetailRequest.Id, "FeeSettings");

                if (service == null)
                {
                    throw new NotFoundException(
                        $"Service with id: {serviceDetailRequest.Id} not found!"); // Consider throwing an exception for better error handling
                }

                // Set var
                var quantity = serviceDetailRequest.Quantity;
                var price = service.Amount * quantity - service.Amount * quantity * (service.DiscountRate / 100);

                if (service.Type == TypeServiceEnums.TRUCK.ToString() ||
                    service.Type == TypeServiceEnums.PORTER.ToString())
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
                            var (totalTruckFee, feeTruckDetails) = CalculateDistanceFee(truckCategoryId,
                                double.Parse(estimatedDistance.ToString()), kmUnitFees, quantity ?? 1);
                            amount += totalTruckFee;
                            break;
                        case "PORTER":
                            // FEE FLOOR
                            var (floorTotalFee, floorUnitFeeDetails) = CalculateFloorFeeV2(truckCategoryId,
                                int.Parse(floorsNumber ?? "1"), floorUnitFees, quantity ?? 1);
                            amount += floorTotalFee;

                            break;
                    }

                    // FEE BASE
                    var (nullTotalFee, nullUnitFeeDetails) = CalculateBaseFee(nullUnitFees, quantity ?? 1);
                    amount += nullTotalFee;

                    amount = (double)(amount -
                                      amount * (service.DiscountRate ?? 0) / 100)!;

                    totalServices += amount;

                    var serviceDetail = new ServiceDetail
                    {
                        ServiceId = service.Id,
                        Quantity = quantity,
                        Price = amount,
                        IsQuantity = service.IsQuantity,
                    };

                    serviceDetails.Add(serviceDetail);
                }
                else
                {
                    var serviceDetail = new ServiceDetail
                    {
                        ServiceId = service.Id,
                        Quantity = quantity,
                        Price = price,
                        IsQuantity = service.IsQuantity,
                    };

                    serviceDetails.Add(serviceDetail);
                }
            }

            return (totalServices, serviceDetails);
        }

        #endregion

        //
        //Driver update status booking  detail
        public async Task<OperationResult<BookingDetailsResponse>> DriverUpdateStatusBooking(int bookingId)
        {
            var result = new OperationResult<BookingDetailsResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, "BookingDetail not found.");
                    return result;
                }

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)bookingDetail.BookingId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, "Booking not found.");
                    return result;
                }

                string nextStatus = bookingDetail.Status;

                switch (bookingDetail.Status)
                {
                    case var status when status == BookingDetailStatus.WAITING.ToString():
                        nextStatus = BookingDetailStatus.ASSIGNED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ASSIGNED.ToString():
                        nextStatus = BookingDetailStatus.ENROUTE.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ENROUTE.ToString():
                        nextStatus = BookingDetailStatus.ARRIVED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ARRIVED.ToString():
                        nextStatus = BookingDetailStatus.IN_PROGRESS.ToString();
                        break;

                    case var status when status == BookingDetailStatus.IN_PROGRESS.ToString():
                        nextStatus = BookingDetailStatus.COMPLETED.ToString();
                        break;
                    case var status when status == BookingDetailStatus.COMPLETED.ToString():
                        if (booking.IsRoundTrip == true && bookingDetail.IsRoundTripCompleted == false)
                        {
                            nextStatus = BookingDetailStatus.ROUND_TRIP.ToString();
                            bookingDetail.IsRoundTripCompleted = true;
                        }

                        break;

                    case var status when status == BookingDetailStatus.ROUND_TRIP.ToString():
                        nextStatus = BookingDetailStatus.ARRIVED.ToString();
                        break;

                    default:
                        result.AddError(StatusCode.BadRequest,
                            "Cannot update to the next status from the current status.");
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                _unitOfWork.BookingRepository.Update(booking);

                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, "Status updated successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<BookingDetailsResponse>> DriverUpdateRoundTripBooking(int bookingId)
        {
            var result = new OperationResult<BookingDetailsResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, "BookingDetail not found.");
                    return result;
                }

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)bookingDetail.BookingId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, "Booking not found.");
                    return result;
                }

                string nextStatus = bookingDetail.Status;

                switch (bookingDetail.Status)
                {
                    case var status when status == BookingDetailStatus.WAITING.ToString():
                        nextStatus = BookingDetailStatus.ASSIGNED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ASSIGNED.ToString():
                        nextStatus = BookingDetailStatus.ENROUTE.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ENROUTE.ToString():
                        nextStatus = BookingDetailStatus.ARRIVED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ARRIVED.ToString():
                        nextStatus = BookingDetailStatus.IN_PROGRESS.ToString();
                        break;

                    case var status when status == BookingDetailStatus.IN_PROGRESS.ToString():
                        nextStatus = BookingDetailStatus.COMPLETED.ToString();
                        break;
                    case var status when status == BookingDetailStatus.COMPLETED.ToString():
                        if (booking.IsUserConfirm == true && booking.IsRoundTrip == false)
                        {
                            nextStatus = BookingDetailStatus.CONFIRM.ToString();
                            booking.IsRoundTrip = true;
                            bookingDetail.IsRoundTripCompleted = true;
                        }

                        break;
                    case var status when status == BookingDetailStatus.CONFIRM.ToString():
                        nextStatus = BookingDetailStatus.ROUND_TRIP.ToString();
                        break;
                    case var status when status == BookingDetailStatus.ROUND_TRIP.ToString():
                        nextStatus = BookingDetailStatus.ARRIVED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest,
                            "Cannot update to the next status from the current status.");
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, "Status updated successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<BookingDetailsResponse>> ReportFail(int bookingId)
        {
            var result = new OperationResult<BookingDetailsResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, "BookingDetail not found.");
                    return result;
                }

                string nextStatus = bookingDetail.Status;

                switch (bookingDetail.Status)
                {
                    case var status when status == BookingDetailStatus.ASSIGNED.ToString():
                        nextStatus = BookingDetailStatus.FAILED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ENROUTE.ToString():
                        nextStatus = BookingDetailStatus.FAILED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest,
                            "Cannot update to the next status from the current status.");
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, "Status updated successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }


        public async Task<OperationResult<BookingDetailsResponse>> PorterUpdateStatusBooking(int bookingId)
        {
            var result = new OperationResult<BookingDetailsResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, "BookingDetail not found.");
                    return result;
                }

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)bookingDetail.BookingId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, "Booking not found.");
                    return result;
                }

                string nextStatus = bookingDetail.Status;

                switch (bookingDetail.Status)
                {
                    case var status when status == BookingDetailStatus.WAITING.ToString():
                        nextStatus = BookingDetailStatus.ASSIGNED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ASSIGNED.ToString():
                        nextStatus = BookingDetailStatus.ENROUTE.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ENROUTE.ToString():
                        nextStatus = BookingDetailStatus.ARRIVED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ARRIVED.ToString():
                        nextStatus = BookingDetailStatus.IN_PROGRESS.ToString();
                        break;

                    case var status when status == BookingDetailStatus.IN_PROGRESS.ToString():
                        nextStatus = BookingDetailStatus.IN_TRANSIT.ToString();
                        break;
                    case var status when status == BookingDetailStatus.IN_TRANSIT.ToString():
                        nextStatus = BookingDetailStatus.DELIVERED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.DELIVERED.ToString():
                        nextStatus = BookingDetailStatus.UNLOAD.ToString();
                        break;
                    case var status when status == BookingDetailStatus.UNLOAD.ToString():
                        nextStatus = BookingDetailStatus.COMPLETED.ToString();
                        break;
                    case var status when status == BookingDetailStatus.COMPLETED.ToString():
                        if (booking.IsRoundTrip == true && bookingDetail.IsRoundTripCompleted == false)
                        {
                            nextStatus = BookingDetailStatus.ROUND_TRIP.ToString();
                            bookingDetail.IsRoundTripCompleted = true;
                        }

                        break;

                    case var status when status == BookingDetailStatus.ROUND_TRIP.ToString():
                        nextStatus = BookingDetailStatus.ARRIVED.ToString();
                        break;

                    default:
                        result.AddError(StatusCode.BadRequest,
                            "Cannot update to the next status from the current status.");
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                _unitOfWork.BookingRepository.Update(booking);

                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, "Status updated successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<BookingDetailsResponse>> PorterRoundTripBooking(int bookingId)
        {
            var result = new OperationResult<BookingDetailsResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, "BookingDetail not found.");
                    return result;
                }

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync((int)bookingDetail.BookingId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, "Booking not found.");
                    return result;
                }

                string nextStatus = bookingDetail.Status;

                switch (bookingDetail.Status)
                {
                    case var status when status == BookingDetailStatus.WAITING.ToString():
                        nextStatus = BookingDetailStatus.ASSIGNED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ASSIGNED.ToString():
                        nextStatus = BookingDetailStatus.ENROUTE.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ENROUTE.ToString():
                        nextStatus = BookingDetailStatus.ARRIVED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ARRIVED.ToString():
                        nextStatus = BookingDetailStatus.IN_PROGRESS.ToString();
                        break;

                    case var status when status == BookingDetailStatus.IN_PROGRESS.ToString():
                        nextStatus = BookingDetailStatus.IN_TRANSIT.ToString();
                        break;
                    case var status when status == BookingDetailStatus.IN_TRANSIT.ToString():
                        nextStatus = BookingDetailStatus.DELIVERED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.DELIVERED.ToString():
                        nextStatus = BookingDetailStatus.UNLOAD.ToString();
                        break;
                    case var status when status == BookingDetailStatus.UNLOAD.ToString():
                        nextStatus = BookingDetailStatus.COMPLETED.ToString();
                        break;
                    case var status when status == BookingDetailStatus.COMPLETED.ToString():
                        if (booking.IsUserConfirm == true && booking.IsRoundTrip == false)
                        {
                            nextStatus = BookingDetailStatus.CONFIRM.ToString();
                            bookingDetail.IsRoundTripCompleted = true;
                            booking.IsRoundTrip = true;
                        }

                        break;
                    case var status when status == BookingDetailStatus.CONFIRM.ToString():
                        nextStatus = BookingDetailStatus.ROUND_TRIP.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ROUND_TRIP.ToString():
                        nextStatus = BookingDetailStatus.ARRIVED.ToString();
                        break;

                    default:
                        result.AddError(StatusCode.BadRequest,
                            "Cannot update to the next status from the current status.");
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                _unitOfWork.BookingRepository.Update(booking);

                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, "Status updated successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }


        public async Task<OperationResult<BookingDetailsResponse>> ReviewerOnlineUpdateStatusBooking(int bookingId)
        {
            var result = new OperationResult<BookingDetailsResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, "BookingDetail not found.");
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
                    case var status when status == BookingDetailStatus.ASSIGNED.ToString():
                        nextStatus = BookingDetailStatus.SUGGESTED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.SUGGESTED.ToString():
                        nextStatus = BookingDetailStatus.REVIEWED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest,
                            "Cannot update to the next status from the current status.");
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                //_unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, "Status updated successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }


        public async Task<OperationResult<BookingDetailsResponse>> ReviewerOfflineUpdateStatusBooking(int bookingId)
        {
            var result = new OperationResult<BookingDetailsResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, "BookingDetail not found.");
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
                    case var status when status == BookingDetailStatus.ASSIGNED.ToString():
                        nextStatus = BookingDetailStatus.ENROUTE.ToString();
                        break;

                    case var status when status == BookingDetailStatus.ENROUTE.ToString():
                        nextStatus = BookingDetailStatus.ARRIVED.ToString();
                        break;
                    case var status when status == BookingDetailStatus.ARRIVED.ToString():
                        nextStatus = BookingDetailStatus.SUGGESTED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.SUGGESTED.ToString():
                        nextStatus = BookingDetailStatus.REVIEWED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest,
                            "Cannot update to the next status from the current status.");
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                //_unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, "Status updated successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<BookingDetailsResponse>> ReviewerCancelBooking(int bookingId)
        {
            var result = new OperationResult<BookingDetailsResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, "BookingDetail not found.");
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
                    case var status when status == BookingDetailStatus.SUGGESTED.ToString():
                        nextStatus = BookingDetailStatus.CANCELLED.ToString();
                        break;

                    case var status when status == BookingDetailStatus.CANCELLED.ToString():
                        nextStatus = BookingDetailStatus.REFUNDED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest,
                            "Cannot update to the next status from the current status.");
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                //_unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, "Status updated successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status");
                throw;
            }

            return result;
        }

        public async Task<OperationResult<BookingDetailsResponse>> ReviewerCompletedBooking(int bookingId)
        {
            var result = new OperationResult<BookingDetailsResponse>();

            try
            {
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    result.AddError(StatusCode.NotFound, "BookingDetail not found.");
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
                    case var status when status == BookingDetailStatus.ASSIGNED.ToString():
                        nextStatus = BookingDetailStatus.REVIEWED.ToString();
                        break;
                    default:
                        result.AddError(StatusCode.BadRequest,
                            "Cannot update to the next status from the current status.");
                        return result;
                }

                bookingDetail.Status = nextStatus;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
                //_unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<BookingDetailsResponse>(bookingDetail);
                result.AddResponseStatusCode(StatusCode.Ok, "Status updated successfully.", response);
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
                    result.AddError(StatusCode.NotFound, "Booking not found.");
                    return result;
                }

                booking.IsUserConfirm = true;
                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<BookingResponse>(booking);
                result.AddResponseStatusCode(StatusCode.Ok, "Confirm round trip successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirm round trip");
                throw;
            }

            return result;
        }
    }
}