using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Domain.Enums;

    public enum BookingEnums
    {
        PENDING,
        DEPOSITING,
        ASSIGNED,
        APPROVED,
        REVIEWED,
        COMMING,
        WAITING,
        IN_PROGRESS,
        COMPLETED,
        CANCEL,
        REFUNDED
    }

    public enum BookingDetailStatus
    {
        WAITING,
        ASSIGNED,
        ENROUTE,
        ARRIVED,
        IN_PROGRESS,
        COMPLETED,
        FAILED,
        ROUND_TRIP,
        CONFIRM,
        SUGGESTED,
        CANCELLED,
        REFUNDED,
        REVIEWED,
        IN_TRANSIT,
        DELIVERED,
        UNLOAD
    }

