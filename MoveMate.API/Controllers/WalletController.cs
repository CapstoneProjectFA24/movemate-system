using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class WalletController : BaseController
    {
        private readonly IWalletServices _walletServices;

        public WalletController(IWalletServices walletServices)
        {
            _walletServices = walletServices;
        }
    }
}
