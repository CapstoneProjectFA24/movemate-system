using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelRequests.Statistics;

namespace MoveMate.Service.Services;

public class StatisticService : IStatisticService
{
    private UnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly ILogger<TransactionService> _logger;


    public StatisticService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TransactionService> logger)
    {
        _unitOfWork = (UnitOfWork)unitOfWork;
        _mapper = mapper;
        _logger = logger;
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

        if (request.Shard == null && request.Type == null)
        {
            request.Shard = DateUtil.GetShardNow();
        }

        if (request.Shard != null && request.Type != null)
        {
            // throw 400 1 trong 2 thui đừng có tham lam
            result.AddError(StatusCode.BadRequest,
                MessageConstant.ShardErrorMessage.ShardAndTypeCannotBeProvidedTogether);
            return result;
        }

        var shard = request.Shard;

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

    /// <summary>
    /// Calculates and summarizes booking statistics based on the provided shard range.
    /// </summary>
    /// <remarks>
    /// This method processes booking statistics according to the criteria specified in the `request` parameter. 
    /// Validation rules:
    /// - If both `Shard` and `Type` are provided, a `400 BadRequest` error will be returned with the message 
    ///   "Shard and Type cannot be provided together."
    /// - Based on the `Type` value, the following shard ranges are handled:
    ///   - **NOW**: Statistics for the current time.
    ///   - **WEEKNOW**: Statistics for the current week.
    ///   - **MONTHNOW**: Statistics for the current month. If `IsSummary` is `true`, only summary statistics are returned;
    ///     otherwise, detailed statistics are provided.
    /// </remarks>
    /// <param name="request">
    /// A <see cref="StatisticRequest"/> object containing the details for statistics calculation, including `Shard`, `Type`,
    /// and `IsSummary` to specify the desired level of detail.
    /// </param>
    /// <returns>
    /// Returns an <see cref="OperationResult{object}"/> containing the statistics data if successful, or an error if the 
    /// input is invalid or another issue occurs.
    /// </returns>
    public async Task<OperationResult<object>> StatisticBooking(StatisticRequest request)
    {
        var result = new OperationResult<object>();

        if (request.Shard == null && request.Type == null)
        {
            request.Shard = DateUtil.GetShardNow();
        }

        if (request.Shard != null && request.Type != null)
        {
            // throw 400 1 trong 2 thui đừng có tham lam
            result.AddError(StatusCode.BadRequest,
                MessageConstant.ShardErrorMessage.ShardAndTypeCannotBeProvidedTogether);
            return result;
        }

        var shard = request.Shard;

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

        if (request.IsSummary)
        {
            var data = await _unitOfWork.BookingRepository.CalculateOverallStatisticsAsync(shards);
            data.Shard = shard;
            shardList.Add(data);
        }
        else
        {
            var datas = await _unitOfWork.BookingRepository.CalculateStatisticsPerShardAsync(shards);
            shardList.AddRange(datas);
        }

        result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTransactionSuccess,
            shardList);
        return result;
    }

    /// <summary>
    /// Retrieves a statistical summary of truck categories, including the total number of trucks and bookings for each category.
    /// </summary>
    /// <returns>
    /// Returns an operation result with the statistical data for truck categories.
    /// The result includes a status code, a message, and the data for truck categories.
    /// </returns>
    public async Task<OperationResult<object>> StatisticTruckCategory()
    {
        var result = new OperationResult<object>();
        var data = await _unitOfWork.TruckCategoryRepository.GetTruckCategorySummaryAsync();
        result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTransactionSuccess, data);
        return  result;
    }

    public async Task<OperationResult<object>> StatisticUser(StatisticRequest request)
    {
        var result = new OperationResult<object>();
        List<object> shardList = new List<object>();

        if (request.Shard == null && request.Type == null)
        {
            request.Shard = DateUtil.GetShardNow();
            var data = await _unitOfWork.UserRepository.CalculateStatisticsWithoutShardAsync();
            data.Shard = "All";
            shardList.Add(data);
            
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTransactionSuccess,
                shardList);
            return result;
        }

        if (request.Shard != null && request.Type != null)
        {
            // throw 400 1 trong 2 thui đừng có tham lam
            result.AddError(StatusCode.BadRequest,
                MessageConstant.ShardErrorMessage.ShardAndTypeCannotBeProvidedTogether);
            return result;
        }

        var shard = request.Shard;

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

        
        if (request.IsSummary)
        {
            var data = await _unitOfWork.UserRepository.CalculateOverallStatisticsAsync(shards);
            data.Shard = shard;
            shardList.Add(data);
        }
        else
        {
            var datas = await _unitOfWork.UserRepository.CalculateStatisticsPerShardAsync(shards);
            shardList.AddRange(datas);
        }

        result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTransactionSuccess,
            shardList);
        return result;

    }

    /// <summary>
    /// Retrieves statistics of users grouped by their roles within each group.
    /// </summary>
    /// <remarks>
    /// This method fetches statistics for each group, including:
    /// - The total number of groups.
    /// - For each group, it retrieves the total number of users and the breakdown of users by their roles (e.g., Admin, User, etc.).
    /// It uses the `GroupRepository` to get the required statistics data.
    /// </remarks>
    /// <returns>
    /// Returns an <see cref="OperationResult{T}"/> containing:
    /// - `StatusCode`: HTTP status code, which is OK (200) if the operation is successful.
    /// - `Message`: A success message, indicating that the data retrieval was successful.
    /// - `Data`: The retrieved statistics, which includes total groups and detailed statistics for each group, including the number of users and their role distribution.
    /// </returns>
    public async Task<OperationResult<object>> StatisticGroup()
    {
        var result = new OperationResult<object>();
        var data = await _unitOfWork.GroupRepository.GetGroupUserRoleStatistics();
        result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTransactionSuccess, data);
        return  result;
    }

    /// <summary>
    /// Retrieves promotion statistics by delegating the operation to the repository layer.
    /// </summary>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing:
    /// - A response status code indicating success or failure.
    /// - A message providing context for the result.
    /// - The promotion statistics data, represented as a detailed <see cref="PromotionStatisticsResponse"/> object.
    /// </returns>
    /// <remarks>
    /// This method serves as an abstraction layer between the service and repository. It performs the following steps:
    /// 1. Invokes the `GetPromotionStatisticsAsync` method in the repository to retrieve data.
    /// 2. Wraps the result in an `OperationResult` object with a status code and message.
    /// 3. Returns the processed result to the caller.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// var result = await StatisticPromotion();
    /// if (result.IsSuccess)
    /// {
    ///     var statistics = result.Data as PromotionStatisticsResponse;
    ///     Console.WriteLine($"Total Promotions: {statistics.TotalPromotions}");
    ///     Console.WriteLine($"Active Promotions: {statistics.ActivePromotions}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"Error: {result.Message}");
    /// }
    /// </code>
    /// </example>
    public async Task<OperationResult<object>> StatisticPromotion()
    {
        var result = new OperationResult<object>();
        var data = await _unitOfWork.PromotionCategoryRepository.GetPromotionStatisticsAsync();
        result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTransactionSuccess, data);
        return  result;
    }
}