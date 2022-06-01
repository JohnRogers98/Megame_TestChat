using ChatWebClient.Models.Requests;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace ChatWebClient.Models.SignalR
{
    public class SignalRChat
    {
        private HubConnection _connection;

        public String Token { get; set; }

        #region Callback Events

        public event Action<String> CreateOperatorCallbackEvent;
        public event Action<String> SendMessageCallbackEvent;
        public event Action<String> SetMessagesAsReadCallbackEvent;
        public event Action<String> GetChatsCallbackEvent;
        public event Action<String> GetMessagesCallbackEvent;

        #endregion

        public SignalRChat(String token)
        {
            Token = token;
        }

        #region Actions

        public async Task Connect()
        {
            _connection = new HubConnectionBuilder().WithUrl("http://localhost:5228/chat", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(Token);
            })
                .AddJsonProtocol()
                .Build();

            _connection.On<String>("CreateOperatorConnectionAsyncCallback",
                (response) => CreateOperatorCallbackEvent?.Invoke(response));

            _connection.On<String>("SendMessageAsyncCallback",
                (response) => SendMessageCallbackEvent?.Invoke(response));

            _connection.On<String>("SetMessagesAsReadAsyncCallback",
                (response) => SetMessagesAsReadCallbackEvent?.Invoke(response));

            _connection.On<String>("GetChatsAsyncCallback",
                (response) => GetChatsCallbackEvent?.Invoke(response));

            _connection.On<String>("GetMessagesWithPaginationAsyncCallback",
                (response) => GetMessagesCallbackEvent?.Invoke(response));

            await _connection.StartAsync();
        }

        public async Task GetChats()
        {
            await _connection.InvokeAsync("GetChatsAsync");
        }

        public async Task GetMessagesWithPagination(String playerId, Int32 page = -1)
        {
            await _connection.InvokeAsync("GetMessagesWithPaginationAsync", 
                JsonConvert.SerializeObject(new { OperatorId = Authentication.Id, playerId, page}));
        }

        public async Task SendMessage(SendMessageRequest request)
        {
            await _connection.SendAsync("SendMessageAsync",
                JsonConvert.SerializeObject(request));
        }

        #endregion

    }
}