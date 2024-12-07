using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.Models;

using MoveMate.Repository.Repositories.Repository;
using MoveMate.Repository.DBContext;

namespace MoveMate.Repository.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private MoveMateDbContext _dbContext;
        private IDbFactory _dbFactory;

        //private LoyalUserRepository _loyalUserRepository;
        //private LoyalUserDetailRepository _loyalUserDetailRepository;
        //private LoyalUserSettingRepository _loyalUserSettingRepository;
        private BookingRepository _bookingRepository;
        private BookingDetailRepository _bookingDetailRepository;
        private GroupRepository _GroupRepository;
        private BookingTrackerRepository _bookingTrackerRepository;
        private FeeDetailRepository _feeDetailRepository;
        private FeeSettingRepository _feeSettingRepository;
        private HouseTypeRepository _houseTypeRepository;
       // private HouseTypeSettingRepository _houseTypeSettingRepository;
        private NotificationRepository _notificationRepository;
        private PaymentRepository _paymentRepository;
        private VoucherRepository _voucherRepository;
        private PromotionCategoryRepository _promotionCategoryRepository;
        private UserRepository _userRepository;
        private RoleRepository _roleRepository;
        private ScheduleBookingRepository _scheduleBookingRepository;
        private ScheduleRepository _scheduleRepository;
       // private ScheduleBookingDetailRepository _scheduleBookingDetailRepository;
        private ServiceRepository _serviceRepository;
        private AssignmentsRepository _assignmentsRepository;
        //private TokenRepository _tokenRepository;
        private TrackerSourceRepository _trackerSourceRepository;
        private TransactionRepository _transactionRepository;
        private TruckRepository _truckRepository;
        private TruckCategoryRepository _truckCategoryRepository;
        private TruckImgRepository _truckImgRepository;
        private UserInfoRepository _userInfoRepository;
        private WalletRepository _walletRepository;
        private WithdrawalRepository _withdrawalRepository;
        private HolidaySettingRepository _holidaySettingRepository;
        private ScheduleWorkingRepository _scheduleWorkingRepository;



        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public UnitOfWork(IDbFactory dbFactory)
        {
            this._dbFactory = dbFactory;
            if (this._dbContext == null)
            {
                this._dbContext = dbFactory.InitDbContext();
            }
        }


        //public LoyalUserRepository LoyalUserRepository
        //{
        //    get
        //    {
        //        if (_loyalUserRepository == null)
        //        {
        //            _loyalUserRepository = new LoyalUserRepository(_dbContext);
        //        }

        //        return _loyalUserRepository;
        //    }
        //}

        //public LoyalUserDetailRepository LoyalUserDetailRepository
        //{
        //    get
        //    {
        //        if (_loyalUserDetailRepository == null)
        //        {
        //            _loyalUserDetailRepository = new LoyalUserDetailRepository(_dbContext);
        //        }

        //        return _loyalUserDetailRepository;
        //    }
        //}

        public UserInfoRepository UserInfoRepository
        {
            get
            {
                if (_userInfoRepository == null)
                {
                    _userInfoRepository = new UserInfoRepository(_dbContext);
                }

                return _userInfoRepository;
            }
        }

        //public LoyalUserSettingRepository LoyalUserSettingRepository
        //{
        //    get
        //    {
        //        if (_loyalUserSettingRepository == null)
        //        {
        //            _loyalUserSettingRepository = new LoyalUserSettingRepository(_dbContext);
        //        }

        //        return _loyalUserSettingRepository;
        //    }
        //}

        public BookingRepository BookingRepository
        {
            get
            {
                if (_bookingRepository == null)
                {
                    _bookingRepository = new BookingRepository(_dbContext);
                }

                return _bookingRepository;
            }
        }

        public ScheduleRepository ScheduleRepository
        {
            get
            {
                if (_scheduleRepository == null)
                {
                    _scheduleRepository = new ScheduleRepository(_dbContext);
                }

                return _scheduleRepository;
            }
        }

        public ScheduleWorkingRepository ScheduleWorkingRepository
        {
            get
            {
                if (_scheduleWorkingRepository == null)
                {
                    _scheduleWorkingRepository = new ScheduleWorkingRepository(_dbContext);
                }

                return _scheduleWorkingRepository;
            }
        }

        public HolidaySettingRepository HolidaySettingRepository
        {
            get
            {
                if (_holidaySettingRepository == null)
                {
                    _holidaySettingRepository = new HolidaySettingRepository(_dbContext);
                }

                return _holidaySettingRepository;
            }
        }

        public BookingDetailRepository BookingDetailRepository
        {
            get
            {
                if (_bookingDetailRepository == null)
                {
                    _bookingDetailRepository = new BookingDetailRepository(_dbContext);
                }

                return _bookingDetailRepository;
            }
        }

        public GroupRepository GroupRepository
        {
            get
            {
                if (_GroupRepository == null)
                {
                    _GroupRepository = new GroupRepository(_dbContext);
                }

                return _GroupRepository;
            }
        }

        public BookingTrackerRepository BookingTrackerRepository
        {
            get
            {
                if (_bookingTrackerRepository == null)
                {
                    _bookingTrackerRepository = new BookingTrackerRepository(_dbContext);
                }

                return _bookingTrackerRepository;
            }
        }

        public FeeDetailRepository FeeDetailRepository
        {
            get
            {
                if (_feeDetailRepository == null)
                {
                    _feeDetailRepository = new FeeDetailRepository(_dbContext);
                }

                return _feeDetailRepository;
            }
        }

        public FeeSettingRepository FeeSettingRepository
        {
            get
            {
                if (_feeSettingRepository == null)
                {
                    _feeSettingRepository = new FeeSettingRepository(_dbContext);
                }

                return _feeSettingRepository;
            }
        }


        public HouseTypeRepository HouseTypeRepository
        {
            get
            {
                if (_houseTypeRepository == null)
                {
                    _houseTypeRepository = new HouseTypeRepository(_dbContext);
                }

                return _houseTypeRepository;
            }
        }

        //public HouseTypeSettingRepository HouseTypeSettingRepository
        //{
        //    get
        //    {
        //        if (_houseTypeSettingRepository == null)
        //        {
        //            _houseTypeSettingRepository = new HouseTypeSettingRepository(_dbContext);
        //        }

        //        return _houseTypeSettingRepository;
        //    }
        //}


        public NotificationRepository NotificationRepository
        {
            get
            {
                if (_notificationRepository == null)
                {
                    _notificationRepository = new NotificationRepository(_dbContext);
                }

                return _notificationRepository;
            }
        }

        public PaymentRepository PaymentRepository
        {
            get
            {
                if (_paymentRepository == null)
                {
                    _paymentRepository = new PaymentRepository(_dbContext);
                }

                return _paymentRepository;
            }
        }


        public VoucherRepository VoucherRepository
        {
            get
            {
                if (_voucherRepository == null)
                {
                    _voucherRepository = new VoucherRepository(_dbContext);
                }

                return _voucherRepository;
            }
        }

        public PromotionCategoryRepository PromotionCategoryRepository
        {
            get
            {
                if (_promotionCategoryRepository == null)
                {
                    _promotionCategoryRepository = new PromotionCategoryRepository(_dbContext);
                }

                return _promotionCategoryRepository;
            }
        }

        public RoleRepository RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                {
                    _roleRepository = new RoleRepository(_dbContext);
                }

                return _roleRepository;
            }
        }

        public ScheduleBookingRepository ScheduleBookingRepository
        {
            get
            {
                if (_scheduleBookingRepository == null)
                {
                    _scheduleBookingRepository = new ScheduleBookingRepository(_dbContext);
                }

                return _scheduleBookingRepository;
            }
        }

        //public ScheduleBookingDetailRepository ScheduleBookingDetailRepository
        //{
        //    get
        //    {
        //        if (_scheduleBookingDetailRepository == null)
        //        {
        //            _scheduleBookingDetailRepository = new ScheduleBookingDetailRepository(_dbContext);
        //        }

        //        return _scheduleBookingDetailRepository;
        //    }
        //}

        public ServiceRepository ServiceRepository
        {
            get
            {
                if (_serviceRepository == null)
                {
                    _serviceRepository = new ServiceRepository(_dbContext);
                }

                return _serviceRepository;
            }
        }

        public AssignmentsRepository AssignmentsRepository
        {
            get
            {
                if (_assignmentsRepository == null)
                {
                    _assignmentsRepository = new AssignmentsRepository(_dbContext);
                }

                return _assignmentsRepository;
            }
        }


        //public TokenRepository TokenRepository
        //{
        //    get
        //    {
        //        if (_tokenRepository == null)
        //        {
        //            _tokenRepository = new TokenRepository(_dbContext);
        //        }

        //        return _tokenRepository;
        //    }
        //}

        public TrackerSourceRepository TrackerSourceRepository
        {
            get
            {
                if (_trackerSourceRepository == null)
                {
                    _trackerSourceRepository = new TrackerSourceRepository(_dbContext);
                }

                return _trackerSourceRepository;
            }
        }

        public TransactionRepository TransactionRepository
        {
            get
            {
                if (_transactionRepository == null)
                {
                    _transactionRepository = new TransactionRepository(_dbContext);
                }

                return _transactionRepository;
            }
        }

        public TruckRepository TruckRepository
        {
            get
            {
                if (_truckRepository == null)
                {
                    _truckRepository = new TruckRepository(_dbContext);
                }

                return _truckRepository;
            }
        }

        public TruckCategoryRepository TruckCategoryRepository
        {
            get
            {
                if (_truckCategoryRepository == null)
                {
                    _truckCategoryRepository = new TruckCategoryRepository(_dbContext);
                }

                return _truckCategoryRepository;
            }
        }

        public TruckImgRepository TruckImgRepository
        {
            get
            {
                if (_truckImgRepository == null)
                {
                    _truckImgRepository = new TruckImgRepository(_dbContext);
                }

                return _truckImgRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_dbContext);
                }

                return _userRepository;
            }
        }

        public WalletRepository WalletRepository
        {
            get
            {
                if (_walletRepository == null)
                {
                    _walletRepository = new WalletRepository(_dbContext);
                }

                return _walletRepository;
            }
        }

        public WithdrawalRepository WithdrawalRepository
        {
            get
            {
                if (_withdrawalRepository == null)
                {
                    _withdrawalRepository = new WithdrawalRepository(_dbContext);
                }

                return _withdrawalRepository;
            }
        }


        public void Commit()
        {
            this._dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await this._dbContext.SaveChangesAsync();
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}