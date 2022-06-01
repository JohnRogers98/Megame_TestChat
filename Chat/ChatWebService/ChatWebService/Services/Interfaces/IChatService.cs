using ChatWebService.Models.ChatDb;

namespace ChatWebService.Services.Interfaces
{
    public interface IChatService
    {
        IEnumerable<Message> GetAllMessagesByOperatorId(String operatorId);
        Int32? GetUnreadMessagesCountByOperatorId(String operatorId);

        Task<Boolean> CreatePlayerConnection(Guid playerId, String connectionId);
        Task<Boolean> CreateOperatorConnection(Guid operatorId, String connectionId);

        Task<(Boolean, IList<ConnectionBase>)> SendMessage(Sender sender, Guid senderId, Message message);

        Task<Boolean> SetMessagesAsRead(Guid readerId, IEnumerable<Int32> messageIds);

        Task<IEnumerable<Guid>> GetChats(Sender sender, Guid senderId);

        Task<IEnumerable<Message>> GetMessages(Guid operatorId, Guid playerId, Int32 page = -1);
    }
}
