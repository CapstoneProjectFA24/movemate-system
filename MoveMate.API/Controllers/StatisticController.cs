using Microsoft.AspNetCore.Mvc;
using MoveMate.Repository.Repositories.Dtos;
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
    public async Task<IActionResult> StatisticTransactions([FromQuery] StatisticRequest request)
    {
        var response = await _statisticService.StatisticTransaction(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    /// <summary>
    /// CHORE: Retrieves booking statistics through the API endpoint.
    /// </summary>
    /// <remarks>
    /// This is a GET HTTP endpoint that calculates and returns booking statistics based on the provided query parameters.
    /// Validation rules:
    /// - Accepts `Shard`, `Type`, and `IsSummary` as query parameters.
    /// - If both `Shard` and `Type` are provided, a `400 BadRequest` error is returned with the message 
    ///   "Shard and Type cannot be provided together."
    /// - If `Type` is invalid, a `400 BadRequest` error is returned with the message "Invalid statistic type."
    /// - The shard range must have a valid format, such as `yyyy-yyyy`, `yyyyMM-yyyyMM`, or `yyyyMMdd-yyyyMMdd`. If the format
    ///   is invalid, an appropriate error message is returned.
    /// - Based on the `Type` value, the shard range can be:
    ///   - **NOW**: Statistics for the current time.
    ///   - **WEEKNOW**: Statistics for the current week.
    ///   - **MONTHNOW**: Statistics for the current month. If `IsSummary` is `true`, a summary is returned; otherwise,
    ///     detailed statistics are provided.
    /// 
    /// Response details:
    /// The response contains statistics in the form of a list of <see cref="CalculateStatisticBookingDto"/> objects, including:
    /// - **Shard**: The shard range used for the statistics (e.g., "2024").
    /// - **TotalBookings**: Total number of bookings.
    /// - **TotalInProcessBookings**: Total number of bookings currently in process (neither completed nor canceled).
    /// - **TotalCancelBookings**: Total number of canceled bookings.
    /// - **MostBookedHouseType**: ID of the house type booked the most (e.g., 1 for `HouseTypeId=1`).
    /// - **MostBookedTruck**: Number of the truck booked the most (e.g., 2 for `TruckNumber=2`).
    /// - **MostBookedTime**: The most booked hour (e.g., 9 for 9:00 AM).
    /// - **MostBookedDayOfWeek**: The most booked day of the week (e.g., "Friday").
    /// - **MostBookedDate**: The specific date booked the most (e.g., "2024-11-01T00:00:00").
    /// </remarks>
    /// <param name="request">
    /// A <see cref="StatisticRequest"/> object containing the request parameters, including `Shard`, `Type`, and `IsSummary`.
    /// </param>
    /// <returns>
    /// Returns a response with a list of <see cref="CalculateStatisticBookingDto"/> objects containing the booking statistics,
    /// or an error if the input is invalid.
    /// </returns>
    [HttpGet("manager/bookings")]
    public async Task<IActionResult> StatisticBookings([FromQuery] StatisticRequest request)
    {
        var response = await _statisticService.StatisticBooking(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    /// <summary>
    /// CHORE: Retrieves a statistical summary of truck categories, including the total number of trucks and bookings for each category.
    /// </summary>
    /// <remarks>
    /// Returns an HTTP response with either a success status and the truck category statistics, or an error response if something goes wrong.
    /// The response contains the following:
    /// - Total number of truck categories
    /// - A list of truck categories, each containing:
    ///   - Truck category ID
    ///   - Name of the truck category
    ///   - Total number of trucks in that category
    ///   - Total number of bookings associated with that category
    /// </remarks>
    /// <returns>
    /// Returns a response with a <see cref="StatisticTruckCategoryResult"/> objects containing the TruckCategory statistics,
    /// or an error if the input is invalid.
    /// </returns>
    [HttpGet("manager/TruckCategoris")]
    public async Task<IActionResult> StatisticTruckCategoris()
    {
        var response = await _statisticService.StatisticTruckCategory();

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}