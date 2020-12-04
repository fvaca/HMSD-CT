using System;
using Microsoft.AspNetCore.Http;

namespace HMSD.APIGateway
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(string message) : base(message) { }
        public int Status { get; set; }

        public object Value { get; set; }

    }
}
