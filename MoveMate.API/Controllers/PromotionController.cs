using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class PromotionController : BaseController
    {
        private readonly IPromotionServices _promotionServices;

        public PromotionController(IPromotionServices promotionServices)
        {
            _promotionServices = promotionServices;
        }
    }
}
