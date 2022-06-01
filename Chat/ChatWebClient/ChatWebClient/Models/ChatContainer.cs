using ChatWebClient.Models.Responses;

namespace ChatWebClient.Models
{
    public static class ChatContainer
    {
        public static Int32 UnreadMessagesCount { get; set; }

        public static List<String> ChatIds { get; set; } = new List<String>();

        public static Dictionary<String, List<Message>> Messages { get; set; } 
            = new Dictionary<String, List<Message>>();
    }
}
