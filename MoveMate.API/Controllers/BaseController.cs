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
            if (errors.Any(e => e.Code == MoveMate.Service.Commons.StatusCode.UnAuthorize))
            {
                var error = errors.FirstOrDefault(e => e.Code == MoveMate.Service.Commons.StatusCode.UnAuthorize);
                return base.Unauthorized(new ErrorResponse(401, "UnAuthorize", true, error!.Message, DateTime.Now));
            }
            if (errors.Any(e => e.Code == MoveMate.Service.Commons.StatusCode.NotFound))
            {
                var error = errors.FirstOrDefault(e => e.Code == MoveMate.Service.Commons.StatusCode.NotFound);
                return base.NotFound(new ErrorResponse(404, "Not Found", true, error!.Message, DateTime.Now));
            }
            if (errors.Any(e => e.Code == MoveMate.Service.Commons.StatusCode.ServerError))
            {
                return base.StatusCode(500, new ErrorResponse(500, "Server Error", true, "An internal server error occurred.", DateTime.Now));
            }
            return StatusCode(400, new ErrorResponse(400, errors.FirstOrDefault()?.Message == null ? "Bad Request" : errors.FirstOrDefault()!.Message, true, errors, DateTime.Now));
        }
    }
}
