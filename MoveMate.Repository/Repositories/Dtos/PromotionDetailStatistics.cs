namespace MoveMate.Repository.Repositories.Dtos;

public class PromotionDetailStatistics
{
    public int PromotionId { get; set; }
    public string PromotionName { get; set; }
    public int TotalUsersTakingVouchers { get; set; }
    public int? Quantity { get; set; }
    public int TotalUsedVouchers { get; set; }
    public double TotalAmountUsedPromotions { get; set; }
    public double TotalAmountRunningPromotion { get; set; }
}
