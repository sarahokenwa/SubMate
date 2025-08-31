using SubMate.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubMate.Core.Exceptions
{
    public class BaseException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; }
        public string ExceptionMessage { get; }

        public BaseException(ExceptionType exceptionType, string? customMessage = null)
            : base(customMessage ?? ResolveMessage(exceptionType))
        {
            HttpStatusCode = ResolveHttpStatus(exceptionType);
            ExceptionMessage = customMessage ?? ResolveMessage(exceptionType);
        }

        private static HttpStatusCode ResolveHttpStatus(ExceptionType type)
        {
            return type switch
            {
                ExceptionType.BAD_REQUEST => HttpStatusCode.BadRequest,
                ExceptionType.ALREADY_EXIST => HttpStatusCode.Conflict,
                ExceptionType.NO_RECORD_FOUND => HttpStatusCode.NotFound, //No record found is a 404 error.
                ExceptionType.NOTFOUND => HttpStatusCode.NotFound,
                ExceptionType.UNAUTHORIZED => HttpStatusCode.Unauthorized,
                ExceptionType.OPERATION_FAILED => HttpStatusCode.ExpectationFailed,
                _ => HttpStatusCode.InternalServerError,
            };
        }

        private static string ResolveMessage(ExceptionType type)
        {
            return type switch
            {
                ExceptionType.BAD_REQUEST => "INVALID_INPUT",
                ExceptionType.ALREADY_EXIST => "ALREADY_EXIST",
                ExceptionType.NO_RECORD_FOUND => "NO_RECORD_FOUND",
                ExceptionType.NOTFOUND => "NOT_FOUND",
                ExceptionType.UNAUTHORIZED => "UNAUTHORIZED",
                ExceptionType.OPERATION_FAILED => "FAILED",
                _ => "INTERNAL_SERVER_ERROR",
            };
        }
    }

}
