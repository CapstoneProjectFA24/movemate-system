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

            try
            {
                var existingHouseType =
                    await _unitOfWork.HouseTypeRepository.GetByIdAsyncV1(request.HouseTypeId, "HouseTypeSettings");

                // check houseType
                if (existingHouseType == null)
                {
                    result.AddError(StatusCode.NotFound, $"Category with id: {request.HouseTypeId} not found!");
                    return result;
                }
                
                // check matching HouseTypeSettings of existingHouseType
                var matchingSetting = existingHouseType.HouseTypeSettings
                    .FirstOrDefault(setting =>
                        setting.TruckCategoryId == request.TruckCategoryId &&
                        setting.NumberOfFloors == (string.IsNullOrEmpty(request.FloorsNumber) ? null : int.Parse(request.FloorsNumber)) &&
                        setting.NumberOfRooms == (string.IsNullOrEmpty(request.RoomNumber) ? null : int.Parse(request.RoomNumber)) &&
                        setting.NumberOfTrucks == request.TruckNumber
                    );
                
                // logic matching
                // if null then 
                // if !null then
                var entity = _mapper.Map<Booking>(request);
                var status = BookingEnums.PENDING.ToString();
                if (matchingSetting == null)
                {
                    status = BookingEnums.RECOMMEND.ToString();
                }
                // logic services and set amount
                
                var serviceDetails = new List<ServiceDetail>();
                foreach (var serviceDetailRequest in request.ServiceDetails)
                {
                    var service = await _unitOfWork.ServiceRepository.GetByIdAsync(serviceDetailRequest.Id);
            
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
                        Price =  price,
                        IsQuantity = serviceDetailRequest.IsQuantity,
                        
                    };
                    
                    // check type 
                    // if type != common
                    // create fee details based on service
                    
                    serviceDetails.Add(serviceDetail);
                }
                
                // list lên fee common
                var feeSettings = await _unitOfWork.FeeSettingRepository.GetCommonFeeSettingsAsync();
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
                }
                
                // save
                status = BookingEnums.APPROVED.ToString();
                entity.Status = status;
                entity.ServiceDetails = serviceDetails;
                //entity.FeeDetails = feeDetails;
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
    }
}
