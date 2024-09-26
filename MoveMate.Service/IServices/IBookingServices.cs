﻿using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IBookingServices
    {
        public Task<OperationResult<List<BookingResponse>>> GetAll(GetAllBookingRequest request);
        Task<OperationResult<BookingRegisterResponse>> RegisterBooking(BookingRegisterRequest request);
        Task<OperationResult<BookingValuationResponse>> ValuationDistanceBooking(BookingValuationRequest request);
        Task<OperationResult<BookingValuationResponse>> ValuationFloorBooking(BookingValuationRequest request);

        Task<OperationResult<BookingValuationResponse>> ValuationBooking(BookingValuationRequest request);

    }
}
