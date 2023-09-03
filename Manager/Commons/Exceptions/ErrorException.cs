using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Exceptions;

[Serializable]
public sealed class ErrorException : Exception
{
    public ErrorException()
    {
    }

    public ErrorException(string message)
    {
        StatusCode = (int)HttpStatusCode.BadRequest;
        Message = message;
    }

    public ErrorException(HttpStatusCode httpStatusCode, string message)
    {
        StatusCode = (int)httpStatusCode;
        Message = message;
    }

    public ErrorException(HttpStatusCode httpStatusCode, string message, object errors)
    {
        StatusCode = (int)httpStatusCode;
        Message = message;
        Errors = errors;
    }


    public int StatusCode { get; set; }

    public bool Succeeded { get; set; }

    public new string Message { get; set; }

    public object Errors { get; }

    public new object Data { get; set; }
}
