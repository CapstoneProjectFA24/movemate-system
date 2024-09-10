using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Repository.Repositories.IRepository
{
    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email);
    }
}
