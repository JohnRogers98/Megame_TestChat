
using ChatWebService.Models.ChatDb;

namespace ChatWebService.Services.Interfaces
{
    public interface IAdminService
    {
        Int32 GetUnreadMessagesCountByOperatorId(String operatorId);

        Task<Boolean> CreateConnection(String operatorId, String connectionId);

        Task<(Boolean, IList<ConnectionBase>)> SendMessage(String operatorId, Message message);

        Task<Boolean> SetMessagesAsRead(String operatorId, IEnumerable<Int32> messageIds);

        Task<IEnumerable<Guid>> GetChats(String operatorId);

        Task<IEnumerable<Message>> GetMessages(String operatorId, String playerId, Int32 page = -1);
    }
}
