using Microsoft.AspNetCore.Http;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MoveMate.Service.Commons;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoveMate.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                NotFoundException _ => Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound,
                BadRequestException _ => Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest,
                ConflictException _ => Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict,
                JsonReaderException _ => Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, // Consider BadRequest for JSON errors
                _ => Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError
            };

            var errorDetails = new List<ErrorDetail>
            {
                new ErrorDetail
                {
                    FieldNameError = null,
                    DescriptionError = new List<string> { ex.Message }
                }
            };

            var error = new Error
            {
                Code = (Service.Commons.StatusCode)context.Response.StatusCode,
                Message = ex.Message // Use the exception message as the error message
            };

            _logger.LogError(ex, "An error occurred while processing the request.");

            await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
        }
    }
}