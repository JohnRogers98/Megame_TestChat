using ChatWebService.Models.ChatDb;

namespace ChatWebService.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<Boolean> CreateConnection(String playerId, String connectionId);

        Task<(Boolean, IList<ConnectionBase>)> SendMessage(String playerId, Message message);

        Task<Boolean> SetMessagesAsRead(String playerId, IEnumerable<Int32> messageIds);

        Task<IEnumerable<Guid>> GetChats(String playerId);

        Task<IEnumerable<Message>> GetMessages(String operatorId, String playerId, Int32 page = -1);
    }
}
