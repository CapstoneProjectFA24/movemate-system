using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.Repository;
using MoveMate.Domain.DBContext;
namespace MoveMate.Repository.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private MoveMateDbContext _dbContext;
        private IDbFactory _dbFactory;

        private AchievementRepository _achievementRepository;
        private AchievementDetailRepository _achievementDetailRepository;
        private AchievementSettingRepository _achievementSettingRepository;
        private BookingRepository _bookingRepository;
        private BookingDetailRepository _bookingDetailRepository;      
        private BookingStaffDailyRepository _bookingStaffDailyRepository;
        private BookingTrackerRepository _bookingTrackerRepository;
        private FeeDetailRepository _feeDetailRepository;
        private FeeSettingRepository _feeSettingRepository;     
        private HouseTypeRepository _houseTypeRepository;    
        private NotificationRepository _notificationRepository;
        private PaymentRepository _paymentRepository;   
        private PromotionDetailRepository _promotionDetailRepository;
        private PromotionCategoryRepository _promotionCategoryRepository;
        private UserRepository _userRepository;
        private RoleRepository _roleRepository;
        private ScheduleRepository _scheduleRepository;
        private ScheduleDetailRepository _scheduleDetailRepository;
        private ServiceRepository _serviceRepository;
        private ServiceDetailsRepository _serviceDetailsRepository;      
        private TokenRepository _tokenRepository;
        private TrackerSourceRepository _trackerSourceRepository;
        private TransactionRepository _transactionRepository;
        private TruckRepository _truckRepository;
        private TruckCategoryRepository _truckCategoryRepository;
        private TruckImgRepository _truckImgRepository;
        private UserInfoRepository _userInfoRepository;
        private WalletRepository _walletRepository;


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


        public AchievementRepository AchievementRepository
        {
            get
            {
                if (_achievementRepository == null)
                {
                    _achievementRepository = new AchievementRepository(_dbContext);
                }
                return _achievementRepository;
            }
        }

        public AchievementDetailRepository AchievementDetailRepository
        {
            get
            {
                if (_achievementDetailRepository == null)
                {
                    _achievementDetailRepository = new AchievementDetailRepository(_dbContext);
                }
                return _achievementDetailRepository;
            }
        }

        public UserInfoRepository UserInfoRepository
        {
            get
            {
                if(_userInfoRepository == null)
                {
                    _userInfoRepository = new UserInfoRepository(_dbContext);
                }
                return _userInfoRepository;
            }
        }

        public AchievementSettingRepository AchievementSettingRepository
        {
            get
            {
                if(_achievementSettingRepository == null)
                {
                    _achievementSettingRepository = new AchievementSettingRepository(_dbContext);
                }
                return _achievementSettingRepository;
            }
        }

        public BookingRepository BookingRepository
        {
            get
            {
                if(_bookingRepository == null)
                {
                    _bookingRepository = new BookingRepository(_dbContext);
                }
                return _bookingRepository;
            }
        }


        public BookingDetailRepository BookingDetailRepository
        {
            get
            {
                if(_bookingDetailRepository == null)
                {
                    _bookingDetailRepository = new BookingDetailRepository(_dbContext);
                }
                return _bookingDetailRepository;
            }
        }

        public BookingStaffDailyRepository BookingStaffDailyRepository
        {
            get
            {
                if(_bookingStaffDailyRepository == null)
                {
                    _bookingStaffDailyRepository = new BookingStaffDailyRepository(_dbContext);
                }
                return _bookingStaffDailyRepository;
            }
        }

        public BookingTrackerRepository BookingTrackerRepository
        {
            get
            {
                if(_bookingTrackerRepository == null)
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
                if(_feeDetailRepository == null)
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
                if(_feeSettingRepository == null)
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
                if(_houseTypeRepository == null)
                {
                    _houseTypeRepository = new HouseTypeRepository(_dbContext);
                }
                return _houseTypeRepository;
            }
        }
      
       
        public NotificationRepository NotificationRepository
        {
            get
            {
                if(_notificationRepository == null)
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
                if(_paymentRepository == null)
                {
                    _paymentRepository = new PaymentRepository(_dbContext); 
                }
                return _paymentRepository;
            }
        }

       
        public PromotionDetailRepository PromotionDetailRepository
        {
            get
            {
                if(_promotionDetailRepository == null)
                {
                    _promotionDetailRepository = new PromotionDetailRepository(_dbContext);
                }
                return _promotionDetailRepository;
            }
        }

        public PromotionCategoryRepository PromotionCategoryRepository
        {
            get
            {
                if(_promotionCategoryRepository == null)
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
                if(_roleRepository == null)
                {
                    _roleRepository = new RoleRepository(_dbContext);
                }
                return _roleRepository;
            }
        }

        public ScheduleRepository ScheduleRepository
        {
            get
            {
                if(_scheduleRepository == null)
                {
                    _scheduleRepository = new ScheduleRepository(_dbContext);
                }
                return _scheduleRepository;
            }
        }

        public ScheduleDetailRepository ScheduleDetailRepository
        {
            get
            {
                if(_scheduleDetailRepository == null)
                {
                    _scheduleDetailRepository = new ScheduleDetailRepository(_dbContext);
                }
                return _scheduleDetailRepository;
            }
        }

        public ServiceRepository ServiceRepository
        {
            get
            {
                if(_serviceRepository == null)
                {
                    _serviceRepository = new ServiceRepository(_dbContext);
                }
                return _serviceRepository;
            }
        }

        public ServiceDetailsRepository ServiceDetailsRepository
        {
            get
            {
                if(_serviceDetailsRepository == null)
                {
                    _serviceDetailsRepository = new ServiceDetailsRepository(_dbContext);
                }
                return _serviceDetailsRepository;
            }
        }

       

        public TokenRepository TokenRepository
        {
            get
            {
                if(_tokenRepository == null)
                {
                    _tokenRepository = new TokenRepository(_dbContext);
                }
                return _tokenRepository;
            }
        }

        public TrackerSourceRepository TrackerSourceRepository
        {
            get
            {
                if(_trackerSourceRepository == null)
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
                if(_transactionRepository == null)
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
                if(_truckRepository == null)
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
                if(_truckCategoryRepository == null)
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
                if(_truckImgRepository == null)
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
                if(_userRepository == null)
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
                if(_walletRepository == null)
                {
                    _walletRepository = new WalletRepository(_dbContext);
                }
                return _walletRepository;
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
