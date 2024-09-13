using System.Collections.Generic;
using MoveMate.Repository.Repositories.UnitOfWork;

namespace MoveMate.Service.Commons
{
    public class OperationResult<T>
    {
        public StatusCode StatusCode { get; set; } = StatusCode.Ok;
        public string? Message { get; set; }
        public bool IsError { get; set; }

        public T? Payload { get; set; }
        
        public object? MetaData { get; set; }
        //public Pagination? Pagination { get; set; }
        public List<Error> Errors { get; set; } = new List<Error>();
        public void AddError(StatusCode code, string message)
        {
            HandleError(code, message);
        }

        public void AddResponseStatusCode(
            StatusCode code,
            string message,
            T? payload,
            object? metaData = null
            )
        {
            HandleResponse(code, message, payload, metaData);
        }

        private void HandleResponse(StatusCode code, string message,
            T? payload, object? metaData)
        {
            StatusCode = code;
            IsError = false;
            Message = message;
            Payload = payload;
            MetaData ??= metaData;


        }

        public void AddUnknownError(string message)
        {
            HandleError(StatusCode.UnknownError, message);
        }
        
        public void ResetIsErrorFlag()
        {
            IsError = false;
        }

        private void HandleResponse(StatusCode code, string message, T? payload, Pagination? pagination = null, object? metaData = null )
        {
            StatusCode = code;
            IsError = false;
            Message = message;
            Payload = payload;
           // Pagination = pagination ?? new Pagination(0,1,1);
            MetaData ??= metaData;

        }

        private void HandleError(StatusCode code, string message)
        {
            Errors.Add(new Error { Code = code, Message = message });
            IsError = true;
        }

        public void AddValidationError(string foodIdAndSupplierIdCannotBeTheSame)
        {
            HandleError(StatusCode.UnknownError, foodIdAndSupplierIdCannotBeTheSame);
        }
    }

}
