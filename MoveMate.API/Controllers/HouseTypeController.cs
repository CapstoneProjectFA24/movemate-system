using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class HouseTypeController : BaseController
    {
        private readonly IHouseTypeServices _houseTypeServices;

        public HouseTypeController(IHouseTypeServices houseTypeServices)
        {
            _houseTypeServices = houseTypeServices;
        }
    }
}
