using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.ViewModels.ModelRequests.Statistics;

namespace MoveMate.Service.IServices
{
    public interface ITransactionService
    {
        Task<OperationResult<List<TransactionResponse>>> GetAll(GetAllTransactionRequest request);
        Task<OperationResult<object>> StatisticTransaction(StatisticRequest request);
    }
}
