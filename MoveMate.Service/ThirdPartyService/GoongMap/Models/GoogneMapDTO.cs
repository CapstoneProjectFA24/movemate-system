namespace MoveMate.Service.ThirdPartyService.GoongMap.Models;

public class GoogleMapDTO
{
    public DistanceInfo Distance { get; set; }
    public DurationInfo Duration { get; set; }
}

public class DistanceInfo
{
    public string Text { get; set; } // "125.73 km"
    public int Value { get; set; } // 125731
}

public class DurationInfo
{
    public string Text { get; set; } // "2 giờ 53 phút"
    public int Value { get; set; } // 10393
}
