using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// 
        /// FEATURE: Get all transaction
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTransactionRequest request)
        {

            var response = await _transactionService.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

    }
}
