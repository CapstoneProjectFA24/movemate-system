namespace MoveMate.Service.ViewModels.ModelResponses;

public class TruckCateDetailResponse : TruckCateResponse
{
    public virtual ICollection<TruckImgResponse> TruckImgs { get; set; } = new List<TruckImgResponse>();
}


public class TruckImgResponse
{
    public string? ImageUrl { get; set; }

}
