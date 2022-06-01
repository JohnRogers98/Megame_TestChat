using ChatWebService.Models.ChatDb;
using ChatWebService.Services.Interfaces;

namespace ChatWebService.Services
{
    public class AdminService : IAdminService
    {
        #region Fields

        private readonly IChatService _chatService;

        #endregion

        public AdminService(IChatService chatService)
        {
            _chatService = chatService;
        }

        #region Public Methods

        public Int32 GetUnreadMessagesCountByOperatorId(String operatorId)
        {
            var unreadMessagesCount = _chatService.GetUnreadMessagesCountByOperatorId(operatorId);

            if (unreadMessagesCount == null)
            {
                throw new ArgumentException();
            }

            return unreadMessagesCount.Value;
        }

        public async Task<Boolean> CreateConnection(String operatorId, String connectionId)
        {
            try
            {
                Guid operatorIdGuid = new Guid(operatorId);

                return await _chatService.CreateOperatorConnection(operatorIdGuid, connectionId);
            }
            catch (FormatException e)
            {
                return false;
            }
        }

        public async Task<(Boolean, IList<ConnectionBase>)> SendMessage(String operatorId, Message message)
        {
            try
            {
                return await _chatService.SendMessage(Sender.Operator, new Guid(operatorId), message);
            }
            catch (FormatException e)
            {
                return (false, null);
            }
        }

        public async Task<Boolean> SetMessagesAsRead(String operatorId, IEnumerable<Int32> messageIds)
        {
            try
            {
                return await _chatService.SetMessagesAsRead(new Guid(operatorId), messageIds);
            }
            catch (FormatException e)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Guid>> GetChats(String operatorId)
        {
            try
            {
                return await _chatService.GetChats(Sender.Operator, new Guid(operatorId));
            }
            catch (FormatException e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Message>> GetMessages(String operatorId, String playerId, Int32 page = -1)
        {
            try
            {
                return await _chatService.GetMessages(new Guid(operatorId), new Guid(playerId), page);
            }
            catch (FormatException e)
            {
                return null;
            }
        }

        #endregion
    }
}
