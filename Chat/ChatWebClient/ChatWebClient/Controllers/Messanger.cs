using ChatWebClient.Models;
using ChatWebClient.Models.Http;
using ChatWebClient.Models.Requests;
using ChatWebClient.Models.Responses;
using ChatWebClient.Models.SignalR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatWebClient.Controllers
{
    public class Messanger : Controller
    {
        #region Fields

        private HttpQueriesToServer _httpQueries;
        private SignalRChat _chat;

        #endregion

        public Messanger()
        {
            _httpQueries = new HttpQueriesToServer();

            _chat = new SignalRChat(Authentication.Token);

            InitializeSignalRChat();

            _chat.Connect().Wait();
        }

        public async Task<IActionResult> Index()
        {
            await GetUnreadMessagesCount();

            await _chat.GetChats();

            this.ViewBag.UnreadMessagesCount = ChatContainer.UnreadMessagesCount;

            return View(ChatContainer.ChatIds);
        }

        #region Actions

        [HttpGet]
        public async Task<IActionResult> Chat([FromQuery]String chatId)
        {
            //get messages
            if (chatId != null)
            {
                await _chat.GetMessagesWithPagination(chatId);

                if (ChatContainer.Messages.ContainsKey(chatId))
                {
                    return View(
                        (chatId, ChatContainer.Messages[chatId])
                        );
                }
                return View();
            }

            return View(nameof(Index));
        }

        [HttpPost]
        public async void SendMessage(SendMessageRequest sendMessageRequest)
        {
            var a = JsonConvert.SerializeObject(sendMessageRequest);
            await _chat.SendMessage(sendMessageRequest);
        }

        #endregion

        #region Private Methods

        private async Task GetUnreadMessagesCount()
        {
            var result = await _httpQueries.GetUnreadMessagesCount(Authentication.Token);

            if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await result.Content.ReadAsStringAsync();
                ChatContainer.UnreadMessagesCount = JsonConvert.DeserializeObject<Int32>(responseString);
               
            }
        } 

        private void InitializeSignalRChat()
        {
            _chat.GetChatsCallbackEvent += GetChatsCallback;
            _chat.GetMessagesCallbackEvent += GetMessagesCallback;
        }

        private void GetChatsCallback(String response)
        {
            var chatIdsResponse = JsonConvert.DeserializeObject<GetChatsResponse>(response);

            ChatContainer.ChatIds = chatIdsResponse.ChatIds;
        }

        private void GetMessagesCallback(String response)
        {
            var chatIdsResponse = JsonConvert.DeserializeObject<GetMessagesResponse>(response);

            var playerId = chatIdsResponse.Messages.First().PlayerId;

            if (ChatContainer.Messages.ContainsKey(playerId))
            {
                ChatContainer.Messages[playerId].AddRange(chatIdsResponse.Messages);
            }
            else
            {
                ChatContainer.Messages.Add(playerId, chatIdsResponse.Messages);
            }
        }

        #endregion

    }
}