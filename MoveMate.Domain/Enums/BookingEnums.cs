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
        REVIEWING,
        REVIEWED,
        COMING,
        WAITING,
        IN_PROGRESS,
        CONFIRMED,
        COMPLETED,
        CANCEL,
        REFUNDED
    }

    public enum AssignmentStatusEnums
    {
        WAITING,
        ASSIGNED,
        INCOMING,
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
        ONGOING,
        DELIVERED,
        UNLOAD
    }

public enum BookingDetailStatusEnums
{
    AVAILABLE,
    WAITING,
    COMPLETED
}

