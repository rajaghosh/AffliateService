using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telemedicine.Models
{
    public class ResponseModel<T>
    {
        public T Data { get; set; }
        public ErrorModel Error { get; set; }
    }
    public class ErrorModel
    {
        public ErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum ErrorCode
    {
        NotFound = 404,
        InternalServerError = 500,
        ExternalApiError = 501,
        UnAuthorize = 401,
        PaymentFailed = 410
    }
}
