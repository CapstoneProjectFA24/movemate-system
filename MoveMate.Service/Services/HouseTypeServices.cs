﻿using AutoMapper;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class HouseTypeServices : IHouseTypeServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<HouseTypeServices> _logger;
        public HouseTypeServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HouseTypeServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<List<HouseTypeResponse>>> GetAll(GetAllHouseTypeRequest request)
        {
            var result = new OperationResult<List<HouseTypeResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.HouseTypeRepository.Get(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()

                );
                var listResponse = _mapper.Map<List<HouseTypeResponse>>(entities);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "List House Type is Empty!", listResponse);
                    return result;
                }

                pagin.PageSize = request.per_page;
                pagin.TotalItemsCount = listResponse.Count();

                result.AddResponseStatusCode(StatusCode.Ok, "Get List Auctions Done", listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<HouseTypesResponse>> GetById(int id)
        {
            var result = new OperationResult<HouseTypesResponse>();
            try
            {
                var entity = await _unitOfWork.HouseTypeRepository.GetByIdAsyncV1(id, includeProperties: "HouseTypeSettings");

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound,"House Type not found");
                }
                else
                {
                    var productResponse = _mapper.Map<HouseTypesResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, "Get House Type success", productResponse);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Get House Type By Id service method for ID: {id}");
                throw;
            }
        }

       

    }
}
