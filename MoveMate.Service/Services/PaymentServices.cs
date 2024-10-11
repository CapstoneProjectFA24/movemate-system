using MoveMate.Service.IServices;

using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Repository.Repositories.UnitOfWork;
using Net.payOS.Types;
using MoveMate.Service.Commons;

namespace MoveMate.Service.Services
{
    public class PaymentService : IPaymentServices
    {
        private UnitOfWork _unitOfWork;
        private readonly PayOS _payOS;

        public PaymentService(IUnitOfWork unitOfWork, PayOS payOS)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _payOS = payOS;
        }

       


    }

}
