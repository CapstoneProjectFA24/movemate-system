using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateHouseTypeSetting
    {
        public int HouseTypeId { get; set; }

        public int TruckCategoryId { get; set; }

        public int NumberOfFloors { get; set; }

        public int NumberOfRooms { get; set; }

        public int NumberOfTrucks { get; set; }
    }
}