using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Domain.Enums
{
    public enum BookingEnums
    {
        PENDING,
        RECOMMEND,
        APPROVED,
        WAITING, // Thanh toán tiền đặt cọc
        ASSIGNED,
        ACCEPT,
        ARRIVED,
        PACKING,
        LOADING,
        IN_TRANSIT,
        DELIVERED,
        UNLOADED,
        ROUND_TRIP,
        CONFIRM,
        COMPLETED, // Thanh toán phần tiền còn lại
        FINISHED,
        CANCEL

    }
}
