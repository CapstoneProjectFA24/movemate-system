using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class TruckController : BaseController
    {
        private readonly ITruckServices _truckServices;
        private readonly IRedisService _cache;
        public TruckController(ITruckServices truckServices, IRedisService cache)
        {
            _cache = cache;
            _truckServices = truckServices;
        }
        
        /// <summary>
        /// 
        /// get truck
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllTruckRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;
            var cec = _cache.GetData<String>("now");
            if (cec is not null)
            {
                return Ok(cec);
            }
           
            DateTime localtime = DateTime.Now;
            DateTime now = TimeZoneInfo.ConvertTime(localtime, TimeZoneInfo.Local, DateUtil.GetSEATimeZone()); 
                //await _truckServices.GetAll(request);
            var response = now;
            
            _cache.SetData("now",now);

            return  Ok(response);
        }
        
        
    }
}
