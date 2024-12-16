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

    /// <summary>
    /// Retrieves statistics for users, including the total number of users and the distribution of users by role.
    /// </summary>
    /// <remarks>
    /// This is a GET HTTP endpoint to request user statistics.
    /// Validation rules:
    /// - The method receives `StatisticRequest` as a query parameter, which includes necessary filters.
    /// - It returns the total number of users, the number of banned users, the number of active users, and the distribution of users by role.
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
    /// </remarks>
    /// <param name="request">The request containing the necessary parameters for user statistics calculation.</param>
    /// <returns>
    /// Returns a `CalculateStatisticUserDto` object containing:
    /// - TotalUsers: Total number of users.
    /// - TotalBannedUsers: Number of banned users.
    /// - TotalActiveUsers: Number of active users.
    /// - UsersByRole: A list of `RoleUserCount` objects, each containing a role name and the number of users in that role.
    /// </returns>
    [HttpGet("manager/users")]
    public async Task<IActionResult> StatisticUsers([FromQuery] StatisticRequest request)
    {
        var response = await _statisticService.StatisticUser(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    /// <summary>
    /// CHORE: Retrieves statistics of users grouped by their roles within each group.
    /// </summary>
    /// <remarks>
    /// This HTTP GET endpoint retrieves statistics for each group, including:
    /// - Total number of groups.
    /// - For each group, the total number of users and the breakdown of users by their roles (e.g., Admin, User, etc.).
    /// </remarks>
    /// <returns>
    /// Returns an <see cref="OkObjectResult"/> containing a <see cref="GroupUserRoleStatisticsResponse"/> object, which includes:
    /// - TotalGroups: The total number of groups.
    /// - Groups: A list of group statistics where each group contains:
    ///   - GroupName: The name of the group.
    ///   - TotalUsers: The total number of users in the group.
    ///   - UsersByRole: A list of roles and the count of users assigned to each role.
    /// </returns>
    [HttpGet("manager/groups")]
    public async Task<IActionResult> StatisticGroups()
    {
        var response = await _statisticService.StatisticGroup();

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    /// <summary>
    /// CHORE: API endpoint to retrieve promotion statistics for managerial purposes.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> containing:
    /// - HTTP 200 status with a detailed <see cref="PromotionStatisticsResponse"/> if the operation is successful.
    /// - An error response with appropriate status code and error details if the operation fails.
    /// </returns>
    /// <remarks>
    /// This API endpoint provides a high-level interface for fetching promotion statistics from the service layer. 
    /// The primary use case is for managerial insights, enabling analysis of:
    /// - Total promotions.
    /// - Active promotions.
    /// - User participation in vouchers.
    /// - Voucher usage statistics.
    /// - Financial impact of promotions.
    /// 
    /// This method:
    /// 1. Calls the <see cref="StatisticPromotion"/> service method to fetch promotion statistics.
    /// 2. Checks for errors in the response.
    /// 3. Returns the appropriate HTTP response based on the operation's success or failure.
    /// 
    /// `Fields`
    /// The following fields are included in the <see cref="PromotionStatisticsResponse"/> object:
    /// - **TotalPromotions**: The total number of promotions available in the system.
    /// - **ActivePromotions**: The number of promotions currently active (within start and end dates).
    /// - **TotalAmountRunningVouchers**: The total value of vouchers that are still running.
    /// - **TotalUsersTakingVouchers**: The total number of users who have claimed vouchers from any promotion.
    /// - **TotalUsedVouchers**: The total number of vouchers that have been redeemed by users.
    /// - **TotalAmountUsedPromotions**: The total monetary value of all redeemed vouchers.
    /// - **PromotionDetails**: A list of detailed statistics for each promotion, including:
    ///   - **PromotionId**: The unique identifier for the promotion.
    ///   - **PromotionName**: The name of the promotion.
    ///   - **TotalUsersTakingVouchers**: The number of users who have claimed vouchers from this promotion.
    ///   - **TotalUsedVouchers**: The number of vouchers redeemed for this promotion.
    ///   - **TotalAmountUsedPromotions**: The monetary value of redeemed vouchers for this promotion.
    ///   - **TotalAmountRunningPromotion**: The monetary value of vouchers still available for this promotion.
    /// - **PromotionActiveDetails**: A list of detailed statistics for currently active promotions (same structure as PromotionDetails).
    /// - **TotalActiveUsersTakingVouchers**: The total number of users who have claimed vouchers from active promotions.
    /// - **TotalActiveUsedVouchers**: The total number of vouchers redeemed from active promotions.
    /// - **TotalActiveAmountRunningVouchers**: The total monetary value of vouchers still running for active promotions.
    /// - **TotalActiveAmountUsedPromotions**: The total monetary value of vouchers redeemed for active promotions.
    /// The underlying service layer utilizes the `GetPromotionStatisticsAsync` repository method to aggregate data.
    /// </remarks>
    /// <example>
    /// Example request:
    /// <code>
    /// GET /manager/Promotions
    /// </code>
    /// Example response (success):
    /// <code>
    /// {
    ///     "totalPromotions": 5,
    ///     "activePromotions": 3,
    ///     "totalAmountRunningVouchers": 1500.0,
    ///     "totalUsersTakingVouchers": 50,
    ///     "totalUsedVouchers": 40,
    ///     "totalAmountUsedPromotions": 1200.0,
    ///     "promotionDetails": [
    ///         {
    ///             "promotionId": 1,
    ///             "promotionName": "New Year Sale",
    ///             "totalUsersTakingVouchers": 25,
    ///             "totalUsedVouchers": 20,
    ///             "totalAmountUsedPromotions": 800.0,
    ///             "totalAmountRunningPromotion": 1000.0
    ///         }
    ///     ],
    ///     "promotionActiveDetails": [
    ///         {
    ///             "promotionId": 2,
    ///             "promotionName": "Holiday Discount",
    ///             "totalUsersTakingVouchers": 15,
    ///             "totalUsedVouchers": 10,
    ///             "totalAmountUsedPromotions": 400.0,
    ///             "totalAmountRunningPromotion": 500.0
    ///         }
    ///     ],
    ///     "totalActiveUsersTakingVouchers": 15,
    ///     "totalActiveUsedVouchers": 10,
    ///     "totalActiveAmountRunningVouchers": 500.0,
    ///     "totalActiveAmountUsedPromotions": 400.0
    /// }
    /// </code>
    /// </example>
    [HttpGet("manager/promotions")]
    public async Task<IActionResult> StatisticPromotions()
    {
        var response = await _statisticService.StatisticPromotion();

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    /// <summary>
    ///
    /// CHORE: STATISTIC FOR SERVICE
    /// 
    /// Retrieves service statistics, including:
    /// - Total number of parent services (Tier = 0).
    /// - Total number of child services (Tier = 1).
    /// - Total number of services.
    /// - Total number of active services.
    /// - Total number of inactive services.
    /// </summary>
    /// <returns>
    /// An object containing the requested statistics.
    /// </returns>
    [HttpGet("manager/services")]
    public async Task<IActionResult> StatisticServices()
    {
        var response = await _statisticService.StatisticSerivice();

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}