using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public ApiResponse(int statusCode,string? message = null )
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(StatusCode);
            
        }

        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            //500 => "Internal Server Error"
            //404 => "Not Found"
            //400 => "Bad Request"
            // 401 => "Unauthorized"
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
    }

