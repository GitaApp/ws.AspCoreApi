using System.Net;

namespace gitaAPI.Controllers
{
    internal class ResponseModel
    {
        
        public HttpStatusCode errorCode { get; set; }
        public string errorMessage { get; set; }
        public object responseData { get; set; }
       
    }
}