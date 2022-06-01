using ChatWebClient.Helpers;

namespace ChatWebClient.Models.Http
{
    public class HttpQueriesToServer
    {
        #region Queries

        public async Task<Response> Register(string email, string password, string name)
        {
            var httpBody = new { email, password, name };
            var httpResponseMessage = await HttpHelper.HttpPost(
                url: "http://localhost:5228/admin/register",
                httpBody: httpBody
                );

            return new Response
            {
                HttpStatusCode = httpResponseMessage.StatusCode
            };
        }

        public async Task<Response> Login(string email, string password)
        {
            var httpBody = new { email, password };
            var httpResponseMessage = await HttpHelper.HttpPost(
                url: "http://localhost:5228/admin/auth",
                httpBody: httpBody
                );

            return new Response
            {
                HttpStatusCode = httpResponseMessage.StatusCode,
                Content = httpResponseMessage.Content
            };
        }

        public async Task<Response> GetUnreadMessagesCount(string token)
        {
            var httpResponseMessage = await HttpHelper.HttpGet(
               url: "http://localhost:5228/admin/messages/unread-count",
               jwtBearer: token
               );

            return new Response
            {
                HttpStatusCode = httpResponseMessage.StatusCode,
                Content = httpResponseMessage.Content
            };
        }

        #endregion

    }
}