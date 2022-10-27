using System;
using System.Collections.Generic;
using System.Net;
using AandAService.Bll.Models;

namespace AandAService.Bll.Helpers
{
    public static class BaseResponseCreator
    {
        public static BaseResponse<T> CreateResponse<T>(T data,HttpStatusCode statusCode,List<string> messages)
        {
            var response = new BaseResponse<T>(data, statusCode, messages);
            return response;
        }
    }
}
