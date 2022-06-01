using ChatWebService.Models.ChatDb;
using ChatWebService.Models.Requests;
using ChatWebService.Services.Interfaces;
using ChatWebService.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace ChatWebService.Controllers.Hubs
{
    [Authorize]
    public partial class ChatHub : Hub
    {
        #region Fields

        private readonly IAdminService _adminService;
        private readonly IPlayerService _playerService;

        #endregion

        #region Properties

        private HttpContext? HttpContext => this.Context.GetHttpContext();

        private String? UserId => HttpContext?.User.Claims.First(x => x.Type == "id").Value;

        private String? UserRole => HttpContext?.User.Claims.First(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;

        private JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        #endregion

        //TODO: both admin and player services have duplicate code 
        public ChatHub(IAdminService adminService, IPlayerService playerService)
        {
            _adminService = adminService;
            _playerService = playerService;
        }

        #region Actions

        /// <summary>
        /// Connects to hub, (!)you must be authenticated(jwt)
        /// </summary>
        /// <returns>body : {created}</returns>
        [Authorize]
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            if (UserRole == Roles.Player)
            {
                CreatePlayerConnectionAsync();
            }
            else if (UserRole == Roles.Admin)
            {
                CreateOperatorConnectionAsync();
            }
        }

        /// <summary>
        /// Disconnects from hub
        /// </summary>
        /// <param name="exception">error</param>
        /// <returns></returns>
        [Authorize]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        [Authorize(Roles = Roles.Player)]
        private async void CreatePlayerConnectionAsync()
        {
            if (HttpContext == null)
            {
                return;
            }

            var playerId = HttpContext.User.Claims.First(x => x.Type == "id").Value;

            var result = await _playerService.CreateConnection(playerId, this.Context.ConnectionId);

            await this.Clients.Caller.SendAsync(GetClientCallBackMethodName(), JsonConvert.SerializeObject(new { Created = result }));
        }

        [Authorize(Roles = Roles.Admin)]
        private async void CreateOperatorConnectionAsync()
        {
            if (HttpContext == null)
            {
                return;
            }

            var operatorId = HttpContext.User.Claims.First(x => x.Type == "id").Value;

            var result = await _adminService.CreateConnection(operatorId, this.Context.ConnectionId);

            await this.Clients.Caller.SendAsync(GetClientCallBackMethodName(), JsonConvert.SerializeObject(new { Created = result }));
        }

        /// <summary>
        /// Sends message to database, then notifies all connections of sender and receiver
        /// </summary>
        /// <param name="messageRequestString">body - {receiverId, body}</param>
        /// <returns>body - {id, operatorId, playerId, isRead, createdAt, body}</returns>
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Player}")]
        public async Task SendMessageAsync(String messageRequestString)
        {
            Boolean isSend;
            IList<ConnectionBase> receiverConnections;

            var messageRequest = JsonConvert.DeserializeObject<Models.Requests.Message>(messageRequestString);
            Models.ChatDb.Message message;

            if (UserRole == Roles.Admin)
            {
                message = this.FormMessageFromRequest(Owner.Operator, messageRequest);
                (isSend, receiverConnections) = await _adminService.SendMessage( 
                    UserId, message);
            }
            else if(UserRole == Roles.Player)
            {
                message = this.FormMessageFromRequest(Owner.Player, messageRequest);
                (isSend, receiverConnections) = await _playerService.SendMessage(
                    UserId, message);
            }
            else
            {
                throw new Exception();
            }

            if (isSend)
            {
                await this.Clients.Users(messageRequest.ReceiverId.ToString(), UserId)
                    .SendAsync(GetClientCallBackMethodName(), JsonConvert.SerializeObject(new { message }, this.JsonSerializerSettings));
            }
        }

        /// <summary>
        /// Sets sended message ids as read
        /// </summary>
        /// <param name="request">body {messageIds- string(Guid)[]}</param>
        /// <returns>body - {result}</returns>
        /// <exception cref="Exception"></exception>
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Player}")]
        public async Task SetMessagesAsReadAsync(String request)
        {
            Boolean result = false;

            var messageIds = JsonConvert.DeserializeObject<JsonBody_SetMessegesAsRead>(request).MessageIds;

            if (UserRole == Roles.Admin)
            {
                result = await _adminService.SetMessagesAsRead(UserId, messageIds);
            }
            else if (UserRole == Roles.Player)
            {
                result = await _playerService.SetMessagesAsRead(UserId, messageIds);
            }
            else
            {
                throw new Exception();
            }

            await this.Clients.User(Context.UserIdentifier)
                   .SendAsync(GetClientCallBackMethodName(), JsonConvert.SerializeObject(new { result }));
        }

        /// <summary>
        /// Get list of chats
        /// </summary>
        /// <returns>body - {chatIds[string(Guid)]}</returns>
        [Authorize(Roles = $"{Roles.Player}, {Roles.Admin}, {Roles.Observer}")]
        public async Task GetChatsAsync()
        {
            IEnumerable<Guid> chatIds;

            if (UserRole == Roles.Player)
            {
                chatIds = await _playerService.GetChats(UserId);
            }
            else
            {
                chatIds = await _adminService.GetChats(UserId);
            }

            await this.Clients.Caller
                   .SendAsync(GetClientCallBackMethodName(), JsonConvert.SerializeObject(new { chatIds }));
        }

        /// <summary>
        /// Gets all messages of a chat. 
        /// If want to accept with the pagination by 10,- send page parameter. 
        /// By default page = -1 what means all messages of the chat wil be sended 
        /// </summary>
        /// <param name="request">body - {operatorId, playerId, page}</param>
        /// <returns>body - {messages[]{id, operatorId, playerId, isRead, createdAt, body}}</returns>
        [Authorize(Roles = $"{Roles.Player}, {Roles.Admin}, {Roles.Observer}")]
        public async Task GetMessagesWithPaginationAsync(String request)
        {
            IEnumerable<Models.ChatDb.Message> messages;

            var getMessages = JsonConvert.DeserializeObject<JsonBody_GetMessagesWithPagination>(request);

            if (UserRole == Roles.Player)
            {
                messages = await _playerService.GetMessages(getMessages.OperatorId, UserId, getMessages.Page);
            }
            else
            {
                messages = await _playerService.GetMessages(getMessages.OperatorId, getMessages.PlayerId, getMessages.Page);
            }

            await this.Clients.Caller
                 .SendAsync(GetClientCallBackMethodName(), JsonConvert.SerializeObject(new { messages }, JsonSerializerSettings));
        }

        #endregion

        #region Private Methods

        private String GetClientCallBackMethodName([CallerMemberName] string member = null) => $"{member}Callback";

        private Models.ChatDb.Message FormMessageFromRequest(Owner owner ,Models.Requests.Message requestMessage)
        {
            var message = new Models.ChatDb.Message
            {
                BodyMessage = requestMessage.Body,
                CreatedAt = DateTime.Now,
                Owner = owner,
                IsRead = false
            };

            if(owner == Owner.Player)
            {
                message.PlayerId = new Guid(UserId);
                message.OperatorId = requestMessage.ReceiverId;
            }
            else
            {
                message.OperatorId = new Guid(UserId);
                message.PlayerId = requestMessage.ReceiverId;
            }

            return message;
        }

        #endregion

    }
}