using System;
using System.Collections.Generic;
using System.Net;

namespace AandAService.Bll.Models
{
    public class BaseResponse<T>
    {
        public BaseResponse(T data, HttpStatusCode statusCode, List<string> messages = null)
        {
            messages = messages ?? new List<string>();
            Data = data;
            StatusCode = statusCode;
            Messages = messages;

            if ((int)statusCode >= 200 && (int)statusCode < 300)
                IsSuccess = true;
        }

        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Messages { get; set; }
    }
}
