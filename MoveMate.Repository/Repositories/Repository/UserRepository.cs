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
using System.Text.RegularExpressions;

namespace MoveMate.Repository.Repositories.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly MoveMateDbContext _dbContext;

        public UserRepository(MoveMateDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public virtual async Task<User?> GetByIdAsyncV1(int id, string includeProperties = "")
        {
            IQueryable<User> query = _dbSet;
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }
            query = query.Where(a => a.Id == id);

            var result = await query.FirstOrDefaultAsync();
            return result;
        }
        
        public async Task<User> GetUserAsync(int accountId)
        {
            try
            {
                IQueryable<User> query = _dbSet;
                return await query.Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Id == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<User>> GetUsersByTruckCategoryIdAsync(int truckCategoryId, int groupId)
        {

            IQueryable<User> query = _dbSet;

            var users = await query
            .Include(u => u.Truck) 
                .Where(u => u.Truck != null && u.Truck.TruckCategoryId == truckCategoryId && u.GroupId == groupId)
                .ToListAsync();

            return users;
        }
        public async Task<List<User>> GetUsersByGroupIdAsync(int groupId, int roleId)
        {

            IQueryable<User> query = _dbSet;

            var users = await query
                .Where(u => u.GroupId == groupId && u.RoleId == roleId)
                .ToListAsync();

            return users;
        }

        public virtual async Task<User?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<User> query = _dbSet;

            // Apply includes
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Filter by ID
            query = query.Where(a => a.Id == id);

            // Execute the query and get the result
            var result = await query.FirstOrDefaultAsync();

            return result;
        }

        public virtual async Task<List<User>> GetByListIdsAsync(IEnumerable<int> ids, string includeProperties = "")
        {
            IQueryable<User> query = _dbSet;

            // Apply includes
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Filter by IDs
            query = query.Where(a => ids.Contains(a.Id));

            // Execute the query and get the result
            var result = await query.ToListAsync();

            return result;
        }

        public async Task<User> GetUserAsyncByEmail(string email)
        {
            try
            {
                IQueryable<User> query = _dbSet;
                return await query.Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<User>> GetByIdAsync(List<int> userIds)
        {
            IQueryable<User> query = _dbSet;
            return await query.Where(u => userIds.Contains(u.Id)) // Get users where ID is in the list
                                 .ToListAsync();
        }

        public async Task<User> GetUserAsync(string email)
        {
            try
            {
                IQueryable<User> query = _dbSet;
                return await query.Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate)
        {
            IQueryable<User> query = _dbSet;
            return await query.AnyAsync(predicate);
        }

        public async Task<User> GetUserByPhoneAsync(string phone)
        {
            try
            {
                IQueryable<User> query = _dbSet;
                return await query.Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Phone == phone);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            IQueryable<User> query = _dbSet;
            return await query
            .AsNoTracking() // No tracking is needed for read operations
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<int>> FindAllUserByRoleIdAsync(int roleId)
        {
            IQueryable<User> query = _dbSet;
            return await query
            .Where(u => u.RoleId == roleId)
                .Select(u => u.Id)
                .ToListAsync();
        }

        public async Task<List<int>> FindAllUserByRoleIdAsync(int roleId, int groupId)
        {
            IQueryable<User> query = _dbSet;
            return await query
            .Where(u => u.RoleId == roleId && u.GroupId == groupId)
                .Select(u => u.Id)
                .ToListAsync();
        }

        public async Task<List<int>> FindAllUserByRoleIdAndGroupIdAsync(int roleId, int groupId)
        {
            IQueryable<User> query = _dbSet;
            return await query
                .Where(u => u.RoleId == roleId)
                .Where(u => u.GroupId == groupId)
                .Select(u => u.Id)
                .ToListAsync();
        }
        
        public async Task<User> GetDriverAsync(int accountId)
        {
            try
            {
                IQueryable<User> query = _dbSet;
                return await query.Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Id == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<List<int>> GetUsersWithTruckCategoryIdAsync(int truckCategoryId)
        {
            try
            {
                IQueryable<User> query = _dbSet;
                return await query
                    .Include(u => u.Truck) 
                    .Where(u => u.Truck!.TruckCategoryId == truckCategoryId)
                    .Select(u => u.Id)
                    .ToListAsync(); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<List<int>> GetUsersWithTruckCategoryIdAsync(int truckCategoryId, int groupId)
        {
            try
            {
                IQueryable<User> query = _dbSet;
                return await query
                    .Where(u => u.GroupId == groupId)
                    .Include(u => u.Truck) 
                    .Where(u => u.Truck!.TruckCategoryId == truckCategoryId)
                    .Select(u => u.Id)
                    .ToListAsync(); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<User>> GetWithTruckCategoryIdAsync(int truckCategoryId, int groupId, string includeProperties = "")
        {
            try
            {
                IQueryable<User> query = _dbSet;

                // Apply includes dynamically
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }

                // Apply filters
                return await query
                    .Where(u => u.GroupId == groupId && u.Truck!.TruckCategoryId == truckCategoryId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<List<User>> GetUserWithRoleIdAndGroupIdAsync(int roleId, int groupId, string includeProperties = "")
        {
            try
            {
                IQueryable<User> query = _dbSet;

                // Apply includes dynamically
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }

                // Apply filters
                return await query
                    .Where(u => u.GroupId == groupId && u.RoleId == roleId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<User> GetUserByRoleIdAsync()
        {
            IQueryable<User> query = _dbSet;
            return await query
            .Where(u => u.RoleId == 6)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetManagerAsync()
        {
            try
            {
                IQueryable<User> query = _dbSet;
                return await query
                    .Where(u => u.RoleId == 6)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}