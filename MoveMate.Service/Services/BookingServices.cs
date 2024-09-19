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
                    
                    double totalFee = 0;

                    foreach (var feeSetting in service.FeeSettings)
                    {
                        var feeDetail = new FeeDetail
                        {
                            FeeSettingId = feeSetting.Id,
                            Name = feeSetting.Name,
                            Description = feeSetting.Description,
                            Amount = feeSetting.Amount * quantity,
                            Quantity = quantity
                        };

                        feeDetails.Add(feeDetail);
                    }

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

        private (double totalFee, List<FeeDetail> feeDetails) CalculateFee(int truckCategoryId, double estimatedDistance,
            List<FeeSetting> feeSettings, int quantity)
        {
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
                    if (remainingDistance > rangeMin)
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
    }
}