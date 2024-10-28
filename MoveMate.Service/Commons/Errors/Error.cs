using Newtonsoft.Json;

namespace MoveMate.Service.Commons.Errors
{
    [JsonObject]
    public class Error
    {
        public StatusCode Code { get; set; }
        public string Message { get; set; }
    }
}