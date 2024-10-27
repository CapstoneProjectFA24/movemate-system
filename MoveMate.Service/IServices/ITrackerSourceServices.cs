using MoveMate.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface ITrackerSourceServices
    {
        Task<OperationResult<bool>> DeleteTrackerSource(int id);
    }
}
