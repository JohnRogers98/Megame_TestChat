using ChatWebService.Models.ChatDb;
using ChatWebService.Services.Interfaces;

namespace ChatWebService.Services
{
    public class PlayerService : IPlayerService
    {
        #region Fields

        readonly IChatService _chatService;

        #endregion

        public PlayerService(IChatService chatService)
        {
            _chatService = chatService;
        }

        #region Public Methods

        public async Task<Boolean> CreateConnection(String playerId, String connectionId)
        {
            try
            {
                Guid playerIdGuid = new Guid(playerId);

                return await _chatService.CreatePlayerConnection(playerIdGuid, connectionId);
            }
            catch(FormatException e)
            {
                return false;
            }
        }

        public async Task<(Boolean, IList<ConnectionBase>)> SendMessage(String playerId, Message message)
        {
            try
            {
                return await _chatService.SendMessage(Sender.Player, new Guid(playerId), message);
            }
            catch (FormatException e)
            {
                return (false, null);
            }
        }

        public async Task<Boolean> SetMessagesAsRead(String playerId, IEnumerable<Int32> messageIds)
        {
            try
            {
                return await _chatService.SetMessagesAsRead(new Guid(playerId), messageIds);
            }
            catch (FormatException e)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Guid>> GetChats(String playerId)
        {
            try
            {
                return await _chatService.GetChats(Sender.Player, new Guid(playerId));
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
