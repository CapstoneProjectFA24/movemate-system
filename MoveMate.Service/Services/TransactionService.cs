﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using MoveMate.Domain.Enums;
using MoveMate.Service.ViewModels.ModelRequests.Statistics;
using DateUtil = MoveMate.Service.Utils.DateUtil;

namespace MoveMate.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;


        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TransactionService> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<List<TransactionResponse>>> GetAll(GetAllTransactionRequest request)
        {
            var result = new OperationResult<List<TransactionResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.TransactionRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<TransactionResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTransactionEmpty,
                        listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTransactionSuccess,
                    listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }


        /// <summary>
        /// CHORE: Calculate and summarize transactions by shard range.
        /// </summary>
        /// <remarks>
        /// This method performs transaction statistics based on the shard range provided in the `request` parameter. 
        /// If both the `Shard` and `Type` parameters are provided, it will return a 400 (BadRequest) error. 
        /// Validation rules:
        /// - If both `Shard` and `Type` are provided in the request, it will return a `400 BadRequest` with the error message "Shard and Type cannot be provided together."
        /// - Based on the `Type` value, the shard range can be:
        ///   - **NOW**: Statistics for the current time.
        ///   - **WEEKNOW**: Statistics for the current week.
        ///   - **MONTHNOW**: Statistics for the current month. If `IsSummary` is `true`, it will return a summary; otherwise, it will return detailed statistics.
        /// </remarks>
        /// <param name="request">Transaction statistics request, including `Shard`, `Type`, and `IsSummary`.</param>
        /// <returns>
        /// Returns a result with transaction statistics information if successful, or an error if there is an issue.
        /// </returns>
        public async Task<OperationResult<object>> StatisticTransaction(StatisticRequest request)
        {
            var result = new OperationResult<object>();

            var shard = request.Shard;

            if (request.Shard != null && request.Type != null)
            {
                // throw 400 1 trong 2 thui đừng có tham lam
                result.AddError(StatusCode.BadRequest,
                    MessageConstant.ShardErrorMessage.ShardAndTypeCannotBeProvidedTogether);
                return result;
            }

            if (request.Type != null)
            {
                switch (request.Type)
                {
                    case var status when status == StatisticEnums.NOW.ToString():
                        Console.WriteLine("Handle statistics for current time.");
                        shard = DateUtil.GetShardNow();
                        break;

                    case var status when status == StatisticEnums.WEEKNOW.ToString():
                        Console.WriteLine("Handle statistics for the current week.");
                        shard = DateUtil.GetCurrentWeekDaysShard();
                        break;

                    case var status when status == StatisticEnums.MONTHNOW.ToString():
                        Console.WriteLine("Handle statistics for the current month.");
                        if (request.IsSummary)
                        {
                            shard = DateUtil.GetCurrentMonthShard();
                        }
                        else
                        {
                            shard = DateUtil.GetCurrentMonthDaysShard();
                        }

                        break;

                    default:
                        Console.WriteLine("Invalid statistic type.");
                        break;
                }
            }


            //var shards = DateUtil.GenerateShardRange(shard);
            var (isError, msg, shards) = DateUtil.GenerateShardRangeV2(shard);

            if (isError)
            {
                result.AddError(StatusCode.BadRequest, msg);
                return result;
            }

            List<object> shardList = new List<object>();
            double sumCompensation = 0d;
            double sumIncome = 0d;

            foreach (var shardDetail in shards)
            {
                var dto = await
                    _unitOfWork.TransactionRepository.CalculateStatisticTransactionsAsync(shardDetail);

                sumCompensation += dto.TotalCompensation;
                sumIncome += dto.TotalIncome;

                if (request.IsSummary)
                {
                }
                else
                {
                    shardList.Add(dto);
                }
            }

            if (request.IsSummary)
            {
                shardList.Add(new
                {
                    Shard = shard,
                    TotalCompensation = sumCompensation,
                    TotalIncome = sumIncome
                });
            }

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTransactionSuccess,
                shardList);
            return result;
        }
    }
}