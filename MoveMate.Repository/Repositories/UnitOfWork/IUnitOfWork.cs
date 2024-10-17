using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;


namespace MoveMate.Repository.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        public void Commit();
        public Task CommitAsync();
    }
}