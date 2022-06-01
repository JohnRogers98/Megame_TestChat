using System.Net;

namespace ChatWebClient.Models.Http
{
    public class Response
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public HttpContent? Content { get; set; }
    }
}
