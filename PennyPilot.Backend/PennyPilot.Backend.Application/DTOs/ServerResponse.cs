using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class ServerResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServerResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ServerResponse<T> { Success = true, Message = message, Data = data };
        }

        public static ServerResponse<T> FailResponse(string message)
        {
            return new ServerResponse<T> { Success = false, Message = message, Data = default };
        }
    }
}
