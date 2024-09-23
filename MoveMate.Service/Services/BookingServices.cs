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
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;

namespace MoveMate.Service.Services
{
    public class BookingServices : IBookingServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<BookingServices> _logger;

        public BookingServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookingServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

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
                    orderBy: request.GetOrder()
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

        public async Task<OperationResult<BookingRegisterResponse>> RegisterBooking(BookingRegisterRequest request)
        {
            var result = new OperationResult<BookingRegisterResponse>();
            string status = BookingEnums.APPROVED.ToString();

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
                // done check houseType


                // check matching HouseTypeSettings of existingHouseType
                // logic matching
                // if null then 
                // if !null then
                bool isValidTruckSelection = _unitOfWork.HouseTypeSettingRepository.IsValidTruckSelection(
                    request.HouseTypeId,
                    request.TruckCategoryId,
                    request.TruckNumber,
                    string.IsNullOrEmpty(request.FloorsNumber) ? null : int.Parse(request.FloorsNumber),
                    string.IsNullOrEmpty(request.RoomNumber) ? null : int.Parse(request.RoomNumber)
                );

                if (!isValidTruckSelection)
                {
                    /*result.AddError(StatusCode.BadRequest, "Truck selection is not valid based on house type and truck settings.");
                    return result;*/
                    status = BookingEnums.RECOMMEND.ToString();
                    
                }
                // done check matching

                // mapping dto to entity
                var entity = _mapper.Map<Booking>(request);

                // logic services and fee set amount

                var serviceDetails = new List<ServiceDetail>();
                var feeDetails = new List<FeeDetail>();
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
                    var price = service.Amount * quantity - service.Amount * quantity * service.DiscountRate;

                    var serviceDetail = new ServiceDetail
                    {
                        ServiceId = service.Id,
                        Quantity = quantity,
                        Price = price,
                        IsQuantity = serviceDetailRequest.IsQuantity,
                    };

                    // logic fee 
                    var feeSettingList = service.FeeSettings;
                    var (nullUnitFees, kmUnitFees, floorUnitFees) =
                        SeparateFeeSettingsByUnit(service.FeeSettings.ToList(), request.HouseTypeId);

                    var (totalTruckFee, feeTruckDetails) = this.CalculateDistanceFee(request.TruckCategoryId,
                        double.Parse(request.EstimatedDistance), kmUnitFees, request.TruckNumber);
                    totalFee += totalTruckFee;
                    feeDetails.AddRange(feeTruckDetails);

                    var (nullTotalFee, nullUnitFeeDetails) = CalculateBaseFee(nullUnitFees, quantity ?? 1);
                    totalFee += nullTotalFee;
                    feeDetails.AddRange(nullUnitFeeDetails);

                    var (floorTotalFee, floorUnitFeeDetails) = CalculateFloorFeeV2(request.TruckCategoryId,
                        int.Parse(request.FloorsNumber ?? "1"), floorUnitFees, quantity ?? 1);
                    totalFee += floorTotalFee;
                    feeDetails.AddRange(floorUnitFeeDetails);
                    // check type 
                    // if type != common
                    // create fee details based on service

                    serviceDetails.Add(serviceDetail);
                }
                // done services

                // list lên fee common
                    var feeSettings = await _unitOfWork.FeeSettingRepository.GetCommonFeeSettingsAsync();
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
                    }
                // done fee

                // save
                entity.Status = status;
                // services
                entity.ServiceDetails = serviceDetails;
                // fees
                entity.FeeDetails = feeDetails;
                entity.TotalFee = totalFee;
                await _unitOfWork.BookingRepository.AddAsync(entity);
                var checkResult = _unitOfWork.Save();

                // create a job check booking time, if now = bookingtime and status still APPROVED then change Stastus to CANCEL
                // logic 

                if (checkResult > 0)
                {
                    var response = _mapper.Map<BookingRegisterResponse>(entity);
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
                    SeparateFeeSettingsByUnit(service.FeeSettings.ToList(), request.HouseTypeId);

                var (totalTruckFee, feeTruckDetails) = CalculateDistanceFee(request.TruckCategoryId,
                    double.Parse(request.EstimatedDistance), kmUnitFees, request.TruckNumber);
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
                    SeparateFeeSettingsByUnit(service.FeeSettings.ToList(), request.HouseTypeId);

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

        // VAlUA
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

            foreach (var serviceDetailRequest in request.ServiceDetails)
            {
                var service =
                    await _unitOfWork.ServiceRepository.GetByIdAsyncV1(serviceDetailRequest.Id, "FeeSettings");

                if (service == null)
                {
                    result.AddError(StatusCode.NotFound, $"Service with id: {serviceDetailRequest.Id} not found!");
                    return result;
                }
                
                var quantity = serviceDetailRequest.Quantity;
                var price = service.Amount * quantity - service.Amount * quantity * service.DiscountRate / 100;
                
                if (service.Amount == 0d)
                {
                    // logic fee 
                    var (nullUnitFees, kmUnitFees, floorUnitFees) =
                        SeparateFeeSettingsByUnit(service.FeeSettings.ToList(), request.HouseTypeId);

                    // FEE FLOOR
                    var (floorTotalFee, floorUnitFeeDetails) = CalculateFloorFeeV2(request.TruckCategoryId,
                        int.Parse(request.FloorsNumber ?? "1"), floorUnitFees, quantity ?? 1);
                    totalFee += floorTotalFee;
                    //feeDetails.AddRange(floorUnitFeeDetails);

                    // FEE DISTANCE
                    var (totalTruckFee, feeTruckDetails) = CalculateDistanceFee(request.TruckCategoryId,
                        double.Parse(request.EstimatedDistance), kmUnitFees, request.TruckNumber);
                    totalFee += totalTruckFee;

                    // FEE BASE
                    var (nullTotalFee, nullUnitFeeDetails) = CalculateBaseFee(nullUnitFees, quantity ?? 1);
                    totalFee += nullTotalFee;
                    //feeDetails.AddRange(nullUnitFeeDetails);

                    totalFee = (double)(totalFee * quantity - totalFee * quantity * service.DiscountRate / 100);
                    
                    var serviceDetail = new ServiceDetail
                    {
                        ServiceId = service.Id,
                        Quantity = quantity,
                        Price = totalFee,
                        IsQuantity = serviceDetailRequest.IsQuantity,
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
                        IsQuantity = serviceDetailRequest.IsQuantity,
                    };
                    
                    serviceDetails.Add(serviceDetail);

                }
                
            }

            var response = new BookingValuationResponse();

            response.Amount = totalFee;
            response.ServiceDetails = _mapper.Map<List<ServiceDetailsResponse>>(serviceDetails);
            
            result.AddResponseStatusCode(StatusCode.Ok, "valuation!", response);

            return result;
        }

        //
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

            return (totalFee, feeDetails);
        }

        private (List<FeeSetting>? nullUnitFees, List<FeeSetting>? kmUnitFees, List<FeeSetting>? floorUnitFees)
            SeparateFeeSettingsByUnit(List<FeeSetting> feeSettings, int HouseTypeId)
        {
            var nullUnitFees = new List<FeeSetting>();
            var kmUnitFees = new List<FeeSetting>();
            var floorUnitFees = new List<FeeSetting>();

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
    }
}