using Microsoft.AspNetCore.Http;
using MoveMate.Service.Commons;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoveMate.Service.Commons.Errors;
using FluentValidation;
using System.Net;
using System.Linq;
using MoveMate.Service.Exceptions;

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
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (BadRequestException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (KeyNotFoundException ex) // Example for handling 404 errors
            {
                await HandleNotFoundExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
        {
            // Prepare the errors dictionary with the first validation error for each property
            var errorsDictionary = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => new[] { g.First().ErrorMessage }
                );

            // Prepare the result with the desired structure
            var result = new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                message = "Bad Request",
                isError = true,
                errors = errorsDictionary,
                timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format for timestamp
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private Task HandleValidationExceptionAsync(HttpContext context, BadRequestException ex)
        {
            // Prepare the errors dictionary with the first validation error for each property
            var errorsDictionary = ex.Message;

            // Prepare the result with the desired structure
            var result = new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                message = "Bad Request",
                isError = true,
                errors = errorsDictionary,
                timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format for timestamp
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private Task HandleNotFoundExceptionAsync(HttpContext context, KeyNotFoundException ex)
        {
            var result = new
            {
                statusCode = (int)HttpStatusCode.NotFound,
                message = "Not Found",
                isError = true,
                errors = new List<string> { ex.Message },
                timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            var result = new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                message = "An unexpected error occurred.",
                isError = true,
                errors = new List<string> { ex.Message },
                timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
