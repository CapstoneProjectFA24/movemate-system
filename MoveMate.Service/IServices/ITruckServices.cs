﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.Service.IServices
{
    public interface ITruckServices
    {
        
        public Task<OperationResult<List<TruckResponse>>> GetAll(GetAllTruckRequest request);

        public Task<OperationResult<List<TruckCateResponse>>> GetAllCate();
        public Task<OperationResult<TruckCateDetailResponse>> GetCateById(int id);
    }
}
