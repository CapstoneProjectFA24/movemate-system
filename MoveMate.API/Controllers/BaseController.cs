using MoveMate.Service.Commons;
using Microsoft.AspNetCore.Mvc;
using MoveMate.API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using MoveMate.Service.Commons;

namespace MoveMate.API.Controllers
{
    [Route("api/v1/[controller]s")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleErrorResponse(List<Error> errors)
        {
            var errorMessages = errors.Select(e => e.Message).ToList(); 

            if (errors.Any(e => e.Code == MoveMate.Service.Commons.StatusCode.UnAuthorize))
            {
                var error = errors.FirstOrDefault(e => e.Code == MoveMate.Service.Commons.StatusCode.UnAuthorize);
                return base.Unauthorized(new
                {
                    statusCode = 401,
                    message = "UnAuthorize",
                    isError = true,
                    errors = new List<string> { error!.Message }, 
                    timestamp = DateTime.Now
                });
            }

            if (errors.Any(e => e.Code == MoveMate.Service.Commons.StatusCode.NotFound))
            {
                var error = errors.FirstOrDefault(e => e.Code == MoveMate.Service.Commons.StatusCode.NotFound);
                return base.NotFound(new
                {
                    statusCode = 404,
                    message = "Not Found",
                    isError = true,
                    errors = new List<string> { error!.Message }, 
                    timestamp = DateTime.Now
                });
            }

            if (errors.Any(e => e.Code == MoveMate.Service.Commons.StatusCode.ServerError))
            {
                var error = errors.FirstOrDefault(e => e.Code == MoveMate.Service.Commons.StatusCode.ServerError);
                return base.StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Server Error",
                    isError = true,
                    errors = new List<string> { error!.Message },
                    timestamp = DateTime.Now
                });
            }

            return StatusCode(400, new
            {
                statusCode = 400,
                message = "Bad Request",
                isError = true,
                errors = errorMessages,
                timestamp = DateTime.UtcNow 
            });
        }

    }
}
