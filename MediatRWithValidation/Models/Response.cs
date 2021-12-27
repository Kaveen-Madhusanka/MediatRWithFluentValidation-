using System.Net;

namespace MediatRWithValidation.Models
{
    public record Response
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string ErrorMessage { get; set; }
    }
}
