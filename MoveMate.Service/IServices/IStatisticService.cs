using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests.Statistics;

namespace MoveMate.Service.IServices;

public interface IStatisticService
{
    Task<OperationResult<object>> StatisticTransaction(StatisticRequest request);
    Task<OperationResult<object>> StatisticBooking(StatisticRequest request);
    Task<OperationResult<object>> StatisticTruckCategory();
    Task<OperationResult<object>> StatisticUser(StatisticRequest request);
    Task<OperationResult<object>> StatisticGroup();
    Task<OperationResult<object>> StatisticPromotion();
}