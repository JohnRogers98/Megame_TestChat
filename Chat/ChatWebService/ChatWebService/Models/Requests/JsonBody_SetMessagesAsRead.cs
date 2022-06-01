namespace ChatWebService.Models.Requests
{
    public class JsonBody_SetMessegesAsRead
    {
        public IEnumerable<int> MessageIds { get; set; }
    }

}
