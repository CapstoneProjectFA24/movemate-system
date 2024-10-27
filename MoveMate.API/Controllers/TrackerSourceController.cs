using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class TrackerSourceController : BaseController
    {
        private readonly ITrackerSourceServices _trackerSourceServices;
        public TrackerSourceController(ITrackerSourceServices trackerSourceServices)
        {
            _trackerSourceServices = trackerSourceServices;
        }

        /// <summary>
        /// FEATURE: Set Tracker source's IsDeleted become true by trackerSourceId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTrackerSourceById(int id)
        {
            var response = await _trackerSourceServices.DeleteTrackerSource(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }



    }
}
