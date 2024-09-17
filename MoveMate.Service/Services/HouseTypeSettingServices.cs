﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class HouseTypeSettingServices : IHouseTypeSettingServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<HouseTypeSettingServices> _logger;
        public HouseTypeSettingServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HouseTypeSettingServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<bool>> CreateEntity(CreateHouseTypeSetting request)
        {
            var result = new OperationResult<bool>();

            try
            {
                var entityHouseType = await _unitOfWork.HouseTypeRepository.GetByIdAsync(request.HouseTypeId);
                if (entityHouseType == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found House Type with Id: {request.HouseTypeId}");
                }

                var entityTruck = await _unitOfWork.TruckCategoryRepository.GetByIdAsync(request.TruckCategoryId);
                if (entityTruck == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Truck Category with Id: {request.TruckCategoryId}");
                }

                var entityHouseTypeSetting = _mapper.Map<HouseTypeSetting>(request);
                // Save entity HouseTypeSetting
                entityHouseTypeSetting = await _unitOfWork.HouseTypeSettingRepository.AddAsync(entityHouseTypeSetting);

                // Ensure product images are saved correctly
                var checkResult = _unitOfWork.Save();

                if (checkResult > 0)
                {
                    result.AddResponseStatusCode(StatusCode.Created, "Add HouseTypeSetting Success!", true); // Return status 201
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add HouseTypeSetting Failed!");
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred in Create HouseTypeSetting service method");
                result.AddError(StatusCode.ServerError, "An error occurred while creating the house type setting.");
                return result;
            }
        }
    }
}