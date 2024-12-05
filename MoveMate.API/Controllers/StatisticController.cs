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
    private readonly ILogger<StatisticController> _logger;
    private readonly IStatisticService _statisticService;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="statisticService"></param>
    public StatisticController(IStatisticService statisticService, ILogger<StatisticController> logger)
    {
        _logger = logger;
        _statisticService = statisticService;
    }

    /// <summary>
    /// CHORE: Method to handle transaction statistics through the API endpoint.
    /// </summary>
    /// <remarks>
    /// This is a GET HTTP endpoint to request transaction statistics.
    /// Validation rules:
    /// - The method receives `Shard`, `Type`, and `IsSummary` as query parameters.
    /// - If both `Shard` and `Type` are provided, it will return a 400 (BadRequest) error.
    /// - If `Type` is invalid, it will return a 400 (BadRequest) error with the message "Invalid statistic type."
    /// - It returns the statistic results if parameters are valid.
    /// Validation rules:
    /// - Checks if the shard range string is null or empty.
    /// - The shard range must have a valid format, such as `yyyy-yyyy`, `yyyyMM-yyyyMM`, `yyyyMMdd-yyyyMMdd`.
    /// - If the format is invalid, it returns an error with a suitable message.
    /// - If both `Shard` and `Type` are provided in the request, it will return a `400 BadRequest` with the error message "Shard and Type cannot be provided together."
    /// - Based on the `Type` value, the shard range can be:
    ///   - **NOW**: Statistics for the current time.
    ///   - **WEEKNOW**: Statistics for the current week.
    ///   - **MONTHNOW**: Statistics for the current month. If `IsSummary` is `true`, it will return a summary; otherwise, it will return detailed statistics.
    /// </remarks>
    /// <param name="request">Transaction statistics request, including `Shard`, `Type`, and `IsSummary`.</param>
    /// <returns>
    /// Returns the statistics results or error messages if any.
    /// </returns>
    [HttpGet("manager/transactions")]
    public async Task<IActionResult> StatisticTransactions([FromQuery]StatisticRequest request)
    {
        var response = await _statisticService.StatisticTransaction(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
}