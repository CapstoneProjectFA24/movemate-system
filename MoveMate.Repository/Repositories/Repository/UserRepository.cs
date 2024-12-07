using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using MoveMate.Repository.DBContext;
using MoveMate.Repository.Repositories.Dtos;

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

        public async Task<List<User>> GetWithTruckCategoryIdAsync(int truckCategoryId, int groupId,
            string includeProperties = "")
        {
            try
            {
                IQueryable<User> query = _dbSet;

                // Apply includes dynamically
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                             StringSplitOptions.RemoveEmptyEntries))
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

        public async Task<List<User>> GetUserWithRoleIdAndGroupIdAsync(int roleId, int groupId,
            string includeProperties = "")
        {
            try
            {
                IQueryable<User> query = _dbSet;

                // Apply includes dynamically
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                             StringSplitOptions.RemoveEmptyEntries))
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

        //
        /// <summary>
        /// Calculates user statistics for a specific shard prefix.
        /// </summary>
        /// <param name="shardPrefix">The shard prefix to filter users by.</param>
        /// <returns>
        /// An object of type <see cref="CalculateStatisticUserDto"/> containing statistics such as the total number of users,
        /// the number of users per role, and other relevant metrics.
        /// </returns>
        public async Task<CalculateStatisticUserDto> CalculateStatisticUsersAsync(string shardPrefix)
        {
            // Lấy dữ liệu người dùng từ shard theo tiền tố
            var users = await _dbSet
                .Where(u => u.Shard.StartsWith(shardPrefix) && u.IsDeleted == false)
                .Include(u => u.Role) // Bao gồm thông tin Role để tính phân vùng theo role
                .ToListAsync();

            // Tổng số người dùng
            var totalUsers = users.Count;

            // Số lượng người dùng bị cấm
            var totalBannedUsers = users
                .Where(u => u.IsBanned == true)
                .Count();

            // Số lượng người dùng không bị cấm
            var totalActiveUsers = users
                .Where(u => u.IsBanned == false)
                .Count();

            // Phân vùng người dùng theo role
            var usersByRole = users
                .GroupBy(u => u.Role?.Name)
                .Select(g => new
                {
                    RoleName = g.Key,
                    UserCount = g.Count()
                })
                .ToList();

            return new CalculateStatisticUserDto
            {
                Shard = shardPrefix,
                TotalUsers = totalUsers,
                TotalBannedUsers = totalBannedUsers,
                TotalActiveUsers = totalActiveUsers,
                UsersByRole = usersByRole.Select(r => new RoleUserCount
                {
                    RoleName = r.RoleName,
                    UserCount = r.UserCount
                }).ToList()
            };
        }

        /// <summary>
        /// Calculates user statistics for multiple shards concurrently.
        /// </summary>
        /// <param name="shardPrefixes">A list of shard prefixes to calculate statistics for each shard.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a list of <see cref="CalculateStatisticUserDto"/>
        /// objects containing the statistics for each shard.
        /// </returns>
        public async Task<List<CalculateStatisticUserDto>> CalculateStatisticsPerShardAsync(List<string> shardPrefixes)
        {
            var results = new List<CalculateStatisticUserDto>();

            foreach (var shardPrefix in shardPrefixes)
            {
                var shardStatistics = await CalculateStatisticUsersAsync(shardPrefix);
                results.Add(shardStatistics);
            }

            return results;
        }

        /// <summary>
        /// Calculates overall statistics for all users across multiple shards.
        /// </summary>
        /// <param name="shardPrefixes">A list of shard prefixes to aggregate data from multiple shards.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is an object of type 
        /// <see cref="CalculateStatisticUserDto"/> containing overall statistics for users across all the specified shards.
        /// </returns>
        public async Task<CalculateStatisticUserDto> CalculateOverallStatisticsAsync(List<string> shardPrefixes)
        {
            // Gom tất cả dữ liệu người dùng từ các shard
            var users = await GetAllUsersFromShardsAsync(shardPrefixes);

            var totalUsers = users.Count;

            // Số lượng người dùng bị cấm
            var totalBannedUsers = users
                .Where(u => u.IsBanned == true)
                .Count();

            // Số lượng người dùng không bị cấm
            var totalActiveUsers = users
                .Where(u => u.IsBanned == false)
                .Count();

            // Phân vùng người dùng theo role
            var usersByRole = users
                .GroupBy(u => u.Role?.Name)
                .Select(g => new
                {
                    RoleName = g.Key,
                    UserCount = g.Count()
                })
                .ToList();

            return new CalculateStatisticUserDto
            {
                TotalUsers = totalUsers,
                TotalBannedUsers = totalBannedUsers,
                TotalActiveUsers = totalActiveUsers,
                UsersByRole = usersByRole.Select(r => new RoleUserCount
                {
                    RoleName = r.RoleName,
                    UserCount = r.UserCount
                }).ToList()
            };
        }

        /// <summary>
        /// Retrieves all user data from multiple shards concurrently.
        /// </summary>
        /// <param name="shardPrefixes">A list of shard prefixes to filter users by.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a list of <see cref="User"/> objects 
        /// representing all users from the specified shards.
        /// </returns>
        private async Task<List<User>> GetAllUsersFromShardsAsync(List<string> shardPrefixes)
        {
            var allUsers = new List<User>();

            foreach (var shardPrefix in shardPrefixes)
            {
                var data = await _dbSet
                    .Where(u => u.Shard.StartsWith(shardPrefix) && u.IsDeleted == false)
                    .Include(u => u.Role) // Bao gồm Role trong truy vấn
                    .ToListAsync();
                allUsers.AddRange(data);
            }

            return allUsers;
        }
        
        /// <summary>
        /// Calculates user statistics without filtering by shard prefix.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="CalculateStatisticUserDto"/> containing statistics such as the total number of users,
        /// the number of users per role, and other relevant metrics.
        /// </returns>
        public async Task<CalculateStatisticUserDto> CalculateStatisticsWithoutShardAsync()
        {
            // Lấy tất cả dữ liệu người dùng
            var users = await _dbSet
                .Where(u => u.IsDeleted == false) // Lọc người dùng không bị xóa
                .Include(u => u.Role) // Bao gồm thông tin Role để tính phân vùng theo role
                .ToListAsync();

            var totalUsers = users.Count;

            // Số lượng người dùng bị cấm
            var totalBannedUsers = users
                .Where(u => u.IsBanned == true)
                .Count();

            // Số lượng người dùng không bị cấm
            var totalActiveUsers = users
                .Where(u => u.IsBanned == false)
                .Count();

            // Phân vùng người dùng theo role
            var usersByRole = users
                .GroupBy(u => u.Role?.Name)
                .Select(g => new
                {
                    RoleName = g.Key,
                    UserCount = g.Count()
                })
                .ToList();

            return new CalculateStatisticUserDto
            {
                TotalUsers = totalUsers,
                TotalBannedUsers = totalBannedUsers,
                TotalActiveUsers = totalActiveUsers,
                UsersByRole = usersByRole.Select(r => new RoleUserCount
                {
                    RoleName = r.RoleName,
                    UserCount = r.UserCount
                }).ToList()
            };
        }

    }
}