using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests.Statistics;

namespace MoveMate.API.Controllers;

/// <summary>
/// This is statistic controller for dash board
/// </summary>
[ApiController]
public class StatisticController : BaseController
{
    private readonly IUserServices _userServices;
    private readonly ILogger<StatisticController> _logger;
    private readonly ITransactionService _transactionService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userServices"></param>
    /// <param name="transactionService"></param>
    /// <param name="logger"></param>
    public StatisticController(IUserServices userServices, ITransactionService transactionService, ILogger<StatisticController> logger)
    {
        _userServices = userServices;
        _transactionService = transactionService;
        _logger = logger;
    }

    /// <summary>
    /// CHORE: Statistic transaction
    /// </summary>
    /// <returns></returns>
    [HttpGet("manager/transactions")]
    public async Task<IActionResult> StatisticTransactions([FromQuery]StatisticRequest request)
    {
        var response = await _transactionService.StatisticTransaction(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
}