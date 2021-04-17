using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class holding lobby messages.
/// </summary>
public class GameMessaging : BaseMessaging
{
    public GameMessaging(ServerCommunication client) : base(client) { }

    // Register messages
    public const string Init = "init";
    /*public UnityAction OnConnectedToServer;*/

    public void OnConnectedToServer()
    {
        EchoMessage(new PositionMessageModel()
        {
            x = 0,
            y = 0,
            tankId = 0
        });
    }

    // Echo messages
    public const string Position = "position";
    /*public UnityAction<PositionMessagModel> OnEchoMessage;*/

    public void OnPositionMessage(PositionMessageModel message)
    {
        Debug.Log("Received position from server! x:" + message.x + " y:" + message.y + " tankId:" + message.tankId);
    }

    // Sends echo message to the server.
    public void EchoMessage(PositionMessageModel request)
    {
        var message = new MessageModel
        {
            method = "position",
            message = JsonUtility.ToJson(request)
        };
        client.SendRequest(JsonUtility.ToJson(message));
    }
}
