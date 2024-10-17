using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.DBContext;
using System.Linq.Expressions;

namespace MoveMate.Repository.Repositories.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly MoveMateDbContext _dbContext;

        public UserRepository(MoveMateDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<User> GetUserAsync(int accountId)
        {
            try
            {
                return await this._dbContext.Users.Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Id == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<User> GetUserAsyncByEmail(string email)
        {
            try
            {
                return await this._dbContext.Users.Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<User> GetUserAsync(string email)
        {
            try
            {
                return await this._dbContext.Users.Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate)
        {
            return await _dbContext.Users.AnyAsync(predicate);
        }

        public async Task<User> GetUserByPhoneAsync(string phone)
        {
            try
            {
                return await this._dbContext.Users.Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Phone == phone);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await this._dbContext.Users
                .AsNoTracking() // No tracking is needed for read operations
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<int>> FindAllUserByRoleIdAsync(int roleId)
        {
            return await _dbContext.Users
                .Where(u => u.RoleId == roleId)
                .Select(u => u.Id)
                .ToListAsync();
        }
    }
}