namespace MoveMate.Service.ViewModels.ModelResponses;

public class TruckCateResponse
{
    public int Id { get; set; }
    public string? CategoryName { get; set; }

    public double? MaxLoad { get; set; }

    public string? Description { get; set; }

    public string? ImgUrl { get; set; }

    public string? EstimatedLength { get; set; }

    public string? EstimatedWidth { get; set; }

    public string? EstimatedHeight { get; set; }

    public string? Summarize { get; set; }
}