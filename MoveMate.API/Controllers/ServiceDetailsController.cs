using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ServiceDetailsController : BaseController
    {
        private readonly IServiceDetails _serviceDetails;

        public ServiceDetailsController(IServiceDetails serviceDetails)
        {
            _serviceDetails = serviceDetails;
        }
    }
}
