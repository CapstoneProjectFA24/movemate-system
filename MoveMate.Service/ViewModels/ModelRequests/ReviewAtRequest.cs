using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class ReviewAtRequest
    {

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReviewAt { get; set; }
    }
}
