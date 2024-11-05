using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Firebase.Notification
{
    public record PushResult(
    int StatusCode,
    bool IsSuccessStatusCode,
    string Message,
    string Error);
}
