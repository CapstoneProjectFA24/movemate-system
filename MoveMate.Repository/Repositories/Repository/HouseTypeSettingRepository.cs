using MoveMate.Domain.DBContext;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MoveMate.Repository.Repositories.Repository;

public class HouseTypeSettingRepository : GenericRepository<HouseTypeSetting>, IHouseTypeSettingRepository
{
    private readonly MoveMateDbContext _context;

    public HouseTypeSettingRepository(MoveMateDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ValidateTruckSelection(int houseTypeId, int? truckCategoryId, string? floorsNumber,
        string? roomNumber, int truckNumber)
    {
        // Lấy các cấu hình loại nhà (house type settings)
        var houseTypeSettings = await _context.HouseTypeSettings.Include(h => h.TruckCategory)
            .Where(h => h.HouseTypeId == houseTypeId)
            .ToListAsync();

        if (houseTypeSettings == null || !houseTypeSettings.Any())
            // Không có cấu hình loại nhà
            return false;

        // Phân tích các trường có thể null
        int? parsedFloorsNumber = string.IsNullOrEmpty(floorsNumber) ? null : int.Parse(floorsNumber);
        int? parsedRoomNumber = string.IsNullOrEmpty(roomNumber) ? null : int.Parse(roomNumber);

        // Tìm cấu hình phù hợp cho loại xe tải đã chọn
        var matchingSetting = houseTypeSettings.FirstOrDefault(setting =>
            setting.TruckCategoryId == truckCategoryId &&
            (parsedFloorsNumber == null || setting.NumberOfFloors == parsedFloorsNumber) &&
            (parsedRoomNumber == null || setting.NumberOfRooms == parsedRoomNumber) &&
            setting.NumberOfTrucks == truckNumber);

        // Nếu tìm thấy cấu hình phù hợp, xác thực loại xe tải đã chọn dựa trên MaxLoad
        if (matchingSetting != null)
        {
            var selectedTruckCategory =
                await _context.TruckCategories.FirstOrDefaultAsync(tc => tc.Id == truckCategoryId);
            if (selectedTruckCategory != null)
            {
                // So sánh MaxLoad của loại xe tải đã chọn với loại xe tải tối thiểu yêu cầu trong HouseTypeSetting
                var minRequiredTruckCategory = houseTypeSettings.Where(setting =>
                        (parsedFloorsNumber == null || setting.NumberOfFloors >= parsedFloorsNumber) &&
                        (parsedRoomNumber == null || setting.NumberOfRooms >= parsedRoomNumber) &&
                        setting.NumberOfTrucks == truckNumber)
                    .OrderBy(tc => tc.TruckCategory.MaxLoad)
                    .FirstOrDefault();

                if (minRequiredTruckCategory != null && selectedTruckCategory.MaxLoad >=
                    minRequiredTruckCategory.TruckCategory.MaxLoad)
                    return true; // Lựa chọn xe tải hợp lý
            }
        }

        // Lựa chọn xe tải không hợp lý
        return false;
    }

    public bool IsValidTruckSelection(
        int houseTypeId,
        int truckCategoryId,
        int truckNumber,
        int? floorsNumber,
        int? roomNumber)
    {
        // Lấy các house type settings từ database
        var houseTypeSettings = _context.HouseTypeSettings
            .Where(setting => setting.HouseTypeId == houseTypeId)
            .ToList();

        // Tìm setting phù hợp dựa trên floorsNumber, roomNumber và truckCategoryId
        var matchingSetting = houseTypeSettings
            .FirstOrDefault(setting =>
                setting.TruckCategoryId == truckCategoryId &&
                (setting.NumberOfFloors == floorsNumber || setting.NumberOfFloors == null) &&
                setting.NumberOfRooms == roomNumber &&
                setting.NumberOfTrucks <= truckNumber
            );

        // Nếu tìm thấy setting phù hợp, kiểm tra xem xe tải đã chọn có hợp lệ dựa trên MaxLoad
        if (matchingSetting != null)
        {
            return true;
        }

        // Nếu không tìm thấy matchingSetting, tìm loại xe tải tối thiểu gần nhất
        var closestSetting = houseTypeSettings
            .OrderBy(setting =>
                Math.Abs((setting.NumberOfFloors ?? floorsNumber ?? 0) - (floorsNumber ?? 0)) +
                Math.Abs((setting.NumberOfRooms ?? roomNumber ?? 0) - (roomNumber ?? 0)) +
                Math.Abs((setting.NumberOfTrucks ?? 0) - truckNumber)
            )
            .FirstOrDefault();

        // Nếu không tìm thấy setting gần nhất, trả về false
        if (closestSetting == null) return false;

        // Kiểm tra MaxLoad của xe tải đã chọn so với loại xe tối thiểu trong closestSetting
        var selectedTruck = _context.TruckCategories.FirstOrDefault(truck => truck.Id == truckCategoryId);
        var minimumTruckInClosestSetting =
            _context.TruckCategories.FirstOrDefault(truck => truck.Id == closestSetting.TruckCategoryId);

        if (selectedTruck != null && minimumTruckInClosestSetting != null)
            return selectedTruck.MaxLoad >= minimumTruckInClosestSetting.MaxLoad;

        return false;
    }
}