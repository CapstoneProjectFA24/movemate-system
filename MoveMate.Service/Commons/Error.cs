using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MoveMate.Service.Commons
{
    [JsonObject]
    public class Error
    {
        public StatusCode Code { get; set; }
        public string Message { get; set; }
    }
}

