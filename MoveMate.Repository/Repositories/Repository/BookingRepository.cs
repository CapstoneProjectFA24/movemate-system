using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using MoveMate.Domain.Enums;
using MoveMate.Repository.DBContext;
using MoveMate.Repository.Repositories.Dtos;

namespace MoveMate.Repository.Repositories.Repository
{
    /// <summary>
    /// Repository for managing bookings in the MoveMate application.
    /// </summary>
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookingRepository"/> class with the specified DbContext.
        /// </summary>
        /// <param name="context">The MoveMateDbContext instance for database operations.</param>
        public BookingRepository(MoveMateDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a booking by its ID, including specified related entities.
        /// </summary>
        /// <param name="id">The ID of the booking to retrieve.</param>
        /// <param name="includeProperties">Comma-separated list of related entities to include.</param>
        /// <returns>A <see cref="Booking"/> object, or null if no booking matches the specified ID.</returns>
        public virtual async Task<Booking?> GetByIdAsyncV1(int id, string includeProperties = "")
        {
            IQueryable<Booking> query = _dbSet;
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Include BookingTrackers and their related TrackerResources (fix typo here)
            //query = query
            //    .Include(b => b.BookingDetails)
            //    .Include(b => b.Assignments)
            //    .Include(b => b.FeeDetails)
            //    .Include(b => b.BookingTrackers)              
            //    .ThenInclude(bt => bt.TrackerSources); // Use 'TrackerResources' instead of 'TrackerSources'

            query = query.Where(a => a.Id == id);

            var result = await query.FirstOrDefaultAsync();
            return result;
        }

        /// <summary>
        /// Retrieves a booking by its ID synchronously, including specified related entities.
        /// </summary>
        /// <param name="id">The ID of the booking to retrieve.</param>
        /// <param name="includeProperties">Comma-separated list of related entities to include.</param>
        /// <returns>A <see cref="Booking"/> object, or null if no booking matches the specified ID.</returns>
        public Booking GetByIdV1(int id, string includeProperties = "")
        {
            IQueryable<Booking> query = _dbSet;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                             StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault(e => e.Id == id);
        }
        
        /// <summary>
        /// Retrieves a booking by its ID asynchronously, including specified related entities.
        /// </summary>
        /// <param name="id">The ID of the booking to retrieve.</param>
        /// <param name="includeProperties">Comma-separated list of related entities to include.</param>
        /// <returns>A <see cref="Booking"/> object, or null if no booking matches the specified ID.</returns>
        public virtual async Task<Booking?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<Booking> query = _dbSet;

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
        
        /// <summary>
        /// Retrieves a booking by its ID and associated user ID asynchronously.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <param name="userId">The ID of the user associated with the booking.</param>
        /// <returns>A <see cref="Booking"/> object, or null if no matching booking is found.</returns>
        public virtual async Task<Booking?> GetByBookingIdAndUserIdAsync(int bookingId, int userId)
        {
            IQueryable<Booking> query = _dbSet;
            query = query.Where(b => b.Id == bookingId && b.UserId == userId)
                .Include(b => b.Assignments);
            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a booking asynchronously based on a filter and optional include properties.
        /// </summary>
        /// <param name="filter">An expression defining the filtering criteria.</param>
        /// <param name="include">Optional include properties for related entities.</param>
        /// <returns>A <see cref="Booking"/> object, or null if no matches are found.</returns>
        public virtual async Task<Booking?> GetAsync(
            Expression<Func<Booking, bool>> filter,
            Func<IQueryable<Booking>, IIncludableQueryable<Booking, object>>? include = null)
        {
            IQueryable<Booking> query = _dbSet;

            // Apply include if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply the filter expression to the query
            query = query.Where(filter);

            // Execute the query and return the first result or null if no matches
            return
                await query.AsNoTracking()
                    .FirstOrDefaultAsync(); // Note: Using AsNoTracking to avoid unintended tracking
        }

        /// <summary>
        /// Calculates booking statistics for a specific shard.
        /// </summary>
        /// <param name="shardPrefix">The prefix of the shard to filter bookings by.</param>
        /// <returns>
        /// A <see cref="CalculateStatisticBookingDto"/> object containing statistics such as total bookings,
        /// most booked house type, most booked truck, and other metrics.
        /// </returns>
        public async Task<CalculateStatisticBookingDto> CalculateStatisticBookingsAsync(string shardPrefix)
        {
            var datas = await _dbSet
                .Where(t => t.Shard.StartsWith(shardPrefix) && t.IsDeleted == false)
                .ToListAsync();

            var totalBookings = datas.Count;

            var totalInProcessBookings = datas
                .Where(b => b.IsCancel == false && b.Status != BookingEnums.COMPLETED.ToString())
                .Count();

            var totalCancelBookings = datas
                .Where(b => b.IsCancel == true)
                .Count();

            // Loại nhà được đặt nhiều nhất
            var mostBookedHouseType = datas
                .Where(b => b.HouseTypeId.HasValue)
                .GroupBy(b => b.HouseTypeId)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    HouseTypeId = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            // Loại xe được đặt nhiều nhất
            var mostBookedTruck = datas
                .Where(b => b.TruckNumber.HasValue)
                .GroupBy(b => b.TruckNumber)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    TruckNumber = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            // Thời gian đặt nhiều nhất
            var mostBookedTime = datas
                .Where(b => b.BookingAt.HasValue)
                .GroupBy(b => b.BookingAt.Value.Hour)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    Hour = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            // Ngày trong tuần được đặt nhiều nhất
            var mostBookedDayOfWeek = datas
                .Where(b => b.BookingAt.HasValue)
                .GroupBy(b => b.BookingAt.Value.DayOfWeek)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    DayOfWeek = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            // Ngày cụ thể được đặt nhiều nhất
            var mostBookedDate = datas
                .Where(b => b.BookingAt.HasValue)
                .GroupBy(b => b.BookingAt.Value.Date)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            return new CalculateStatisticBookingDto
            {
                Shard = shardPrefix,
                TotalBookings = totalBookings,
                TotalInProcessBookings = totalInProcessBookings,
                TotalCancelBookings = totalCancelBookings,
                MostBookedHouseType = mostBookedHouseType?.HouseTypeId,
                MostBookedTruck = mostBookedTruck?.TruckNumber,
                MostBookedTime = mostBookedTime?.Hour,
                MostBookedDayOfWeek = mostBookedDayOfWeek?.DayOfWeek.ToString(),
                MostBookedDate = mostBookedDate?.Date
            };
        }
        /// <summary>
        /// Calculates statistics for multiple shards asynchronously.
        /// </summary>
        /// <param name="shardPrefixes">A list of shard prefixes to calculate statistics for.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of 
        /// <see cref="CalculateStatisticBookingDto"/> objects, each representing statistics for a specific shard.
        /// </returns>
        public async Task<List<CalculateStatisticBookingDto>> CalculateStatisticsPerShardAsync(List<string> shardPrefixes)
        {
            var results = new List<CalculateStatisticBookingDto>();

            foreach (var shardPrefix in shardPrefixes)
            {
                var shardStatistics = await CalculateStatisticBookingsAsync(shardPrefix);
                results.Add(shardStatistics);
            }

            return results;
        }

        /// <summary>
        /// Calculates overall booking statistics across all provided shards.
        /// </summary>
        /// <param name="shardPrefixes">A list of shard prefixes to aggregate data from.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a 
        /// <see cref="CalculateStatisticBookingDto"/> object with overall booking statistics such as total bookings,
        /// most booked house type, most booked truck, and other aggregated metrics.
        /// </returns>
        public async Task<CalculateStatisticBookingDto> CalculateOverallStatisticsAsync(List<string> shardPrefixes)
        {
            // Gom tất cả dữ liệu từ các shard
            var datas = await GetAllBookingsFromShardsAsync(shardPrefixes);

            var totalBookings = datas.Count;

            var totalInProcessBookings = datas
                .Where(b => b.IsCancel == false && b.Status != BookingEnums.COMPLETED.ToString())
                .Count();

            var totalCancelBookings = datas
                .Where(b => b.IsCancel == true)
                .Count();

            // Loại nhà được đặt nhiều nhất
            var mostBookedHouseType = datas
                .Where(b => b.HouseTypeId.HasValue)
                .GroupBy(b => b.HouseTypeId)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    HouseTypeId = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            // Loại xe được đặt nhiều nhất
            var mostBookedTruck = datas
                .Where(b => b.TruckNumber.HasValue)
                .GroupBy(b => b.TruckNumber)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    TruckNumber = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            // Thời gian đặt nhiều nhất
            var mostBookedTime = datas
                .Where(b => b.BookingAt.HasValue)
                .GroupBy(b => b.BookingAt.Value.Hour)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    Hour = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            // Thứ trong tuần được đặt nhiều nhất
            var mostBookedDayOfWeek = datas
                .Where(b => b.BookingAt.HasValue)
                .GroupBy(b => b.BookingAt.Value.DayOfWeek)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    DayOfWeek = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            // Ngày được đặt nhiều nhất
            var mostBookedDate = datas
                .Where(b => b.BookingAt.HasValue)
                .GroupBy(b => b.BookingAt.Value.Date)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .FirstOrDefault();

            return new CalculateStatisticBookingDto
            {
                TotalBookings = totalBookings,
                TotalInProcessBookings = totalInProcessBookings,
                TotalCancelBookings = totalCancelBookings,
                MostBookedHouseType = mostBookedHouseType?.HouseTypeId,
                MostBookedTruck = mostBookedTruck?.TruckNumber,
                MostBookedTime = mostBookedTime?.Hour,
                MostBookedDayOfWeek = mostBookedDayOfWeek?.DayOfWeek.ToString(),
                MostBookedDate = mostBookedDate?.Date
            };
        }

        /// <summary>
        /// Retrieves all booking data from multiple shards asynchronously.
        /// </summary>
        /// <param name="shardPrefixes">A list of shard prefixes to filter bookings by.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of 
        /// <see cref="Booking"/> objects representing all bookings from the specified shards.
        /// </returns>
        private async Task<List<Booking>> GetAllBookingsFromShardsAsync(List<string> shardPrefixes)
        {
            var allData = new List<Booking>();

            foreach (var shardPrefix in shardPrefixes)
            {
                var data = await _dbSet
                    .Where(t => t.Shard.StartsWith(shardPrefix) && t.IsDeleted == false)
                    .ToListAsync();
                allData.AddRange(data);
            }

            return allData;
        }
    }
}