namespace MoveMate.Service.ViewModels.ModelResponses;

public class TruckCateDetailResponse : TruckCateResponse
{
    public virtual ICollection<TruckImgResponse> TruckImgs { get; set; } = new List<TruckImgResponse>();
}

public class TruckImgResponse
{
    public int Id { get; set; }

    public int? TruckId { get; set; }

    public string? ImageUrl { get; set; }

    public string? ImageCode { get; set; }
}