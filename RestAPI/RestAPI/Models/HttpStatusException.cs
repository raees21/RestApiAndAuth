using System.Net;

namespace RestAPI.Models;

public class HttpStatusException : Exception
{
    public HttpStatusCode StatusCode { get; set; }

    public HttpStatusException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}