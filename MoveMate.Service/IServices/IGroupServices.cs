using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IGroupServices
    {
        public Task<OperationResult<List<GroupResponse>>> GetAll(GetAllGroupRequest request);
        Task<OperationResult<GroupResponse>> GetGroupById(int id);
        Task<OperationResult<bool>> DeleteGroup(int id);
        public Task<OperationResult<GroupResponse>> UpdateGroup(int id, UpdateGroupRequest request);
        public Task<OperationResult<GroupResponse>> CreateGroup(CreateGroupRequest request);
        public Task<OperationResult<GroupResponse>> AddUserIntoGroup(AddUserIntoGroup request);
    }
}