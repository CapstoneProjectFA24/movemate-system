using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class TruckController : BaseController
    {
        private readonly ITruckServices _truckServices;

        public TruckController(ITruckServices truckServices)
        {
            _truckServices = truckServices;
        }
        
       
        
        
    }
}
