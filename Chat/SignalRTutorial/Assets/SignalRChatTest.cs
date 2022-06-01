using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SignalRChatTest
{
    #region Fields

    private HubConnection _connection;
    private readonly String _chatHubUrl = "http://localhost:5228/chat";

    #endregion

    public String AccessToken { get; set; }

    #region Actions

    public async Task Authenticate(String email, String password)
    {
        AccessToken = await this.PlayerAuth(email, password);

        Debug.Log($"Access token = {AccessToken}");
    }

    public async Task StartConnection()
    {
        _connection = new HubConnectionBuilder().WithUrl(_chatHubUrl, options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(AccessToken);
        })
            .AddJsonProtocol()
            .Build();

        _connection.On<String>("CreatePlayerConnectionAsyncCallback",
            (response) => Debug.Log($"Create connection callback result = {response}"));

        _connection.On<object>("SendMessageAsyncCallback",
            (response) => Debug.Log($"Send message callback result = {response}"));

        _connection.On<object>("SetMessagesAsReadAsyncCallback",
            (response) => Debug.Log($"Set as read callback result = {response}"));

        _connection.On<object>("GetChatsAsyncCallback",
            (response) => Debug.Log($"Get chats callback result = {response}"));

        _connection.On<object>("GetMessagesWithPaginationAsyncCallback",
            (response) => Debug.Log($"Get messages callback result = {response}"));

        await _connection.StartAsync();
    }

    public async Task SendMessageToOperator(String operatorId, String message) 
    {
        await _connection.InvokeAsync("SendMessageAsync", 
            JsonConvert.SerializeObject(new { Body = message, ReceiverId = operatorId }));
    }

    #endregion

    #region Private Metods

    private async Task<String> PlayerAuth(String email, String password)
    {
        using var httpClient = new HttpClient();

        var httpBody = new { email = email, password = password };

        var jsonBody = JsonConvert.SerializeObject(httpBody);
        StringContent data = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("http://localhost:5228/player/auth", data);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AuthenticationResponse>(responseString).Token;
        }
        return null;
    }

    #endregion

    private class AuthenticationResponse
    {
        public String Token { get; set; }

        public String Id { get; set; }
    }
}