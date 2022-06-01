using System;
using UnityEngine;
using UnityEngine.UI;

public class TestConnection : MonoBehaviour
{
    [SerializeField]
    private InputField _inputMessage;

    private SignalRChatTest _connection;

    private static class TestData
    {
        public static String PlayerEmail => "john_rogers@gmail.com";
        public static String PlayerPassword => "_0cdSxf8N";

        public static String OperatorId => "80cec181-423a-47f8-a674-644466c56759";
    }

    async void Start()
    {
        _connection = new SignalRChatTest();
        await _connection.Authenticate(TestData.PlayerEmail, TestData.PlayerPassword);
        await _connection.StartConnection();
    }


    public async void OnButtonClick()
    {
        String message = _inputMessage.text;

        if(String.IsNullOrEmpty(message))
        {
            return;
        }
        else
        {
            await _connection.SendMessageToOperator(TestData.OperatorId, message);
        }
    }

}