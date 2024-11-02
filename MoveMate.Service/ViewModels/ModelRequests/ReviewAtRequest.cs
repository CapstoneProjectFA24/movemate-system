using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class ReviewAtRequest
    {
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ReviewAt { get; set; }
        
        public bool IsReviewAtValid()
        {
            return ReviewAt.HasValue && ReviewAt.Value >= DateTime.Now;
        }
    }
}
