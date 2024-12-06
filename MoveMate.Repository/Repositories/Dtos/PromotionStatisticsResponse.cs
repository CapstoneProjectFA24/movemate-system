namespace MoveMate.Repository.Repositories.Dtos;

public class PromotionStatisticsResponse
{
    public int TotalPromotions { get; set; }
    public int ActivePromotions { get; set; }
    public double TotalAmountRunningVouchers { get; set; }
    public int TotalUsersTakingVouchers { get; set; }
    public int TotalUsedVouchers { get; set; }
    public double TotalAmountUsedPromotions { get; set; }
    public List<PromotionDetailStatistics> PromotionDetails { get; set; }
    public List<PromotionDetailStatistics> PromotionActiveDetails { get; set; } 
    public int TotalActiveUsersTakingVouchers { get; set; } 
    public int TotalActiveUsedVouchers { get; set; } 
    public double TotalActiveAmountRunningVouchers { get; set; } 
    public double TotalActiveAmountUsedPromotions { get; set; } 
}

