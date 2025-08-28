using System;
using System.Net;

namespace InventoryService.Services.HttpClients
{
    public class CylinderApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ResponseBody { get; }

        public CylinderApiException(HttpStatusCode statusCode, string responseBody)
            : base($"Cylinder API returned {(int)statusCode} - {statusCode}")
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }
}
