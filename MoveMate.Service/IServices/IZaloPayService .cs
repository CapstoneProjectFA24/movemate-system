using MoveMate.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IZaloPayService
    {
        public Task<string> CreateOrderAsync(ZaloPayOrderRequest request);
    }
}
