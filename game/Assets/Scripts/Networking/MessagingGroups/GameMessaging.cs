using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class holding lobby messages.
/// </summary>
public class GameMessaging : BaseMessaging
{
    public GameMessaging(ServerCommunication client) : base(client) { }

    public const string Init = "init";
    /*public UnityAction OnConnectedToServer;*/

    public void OnConnectedToServer()
    {
        var message = new MessageModel
        {
            type = "position",
            message = JsonUtility.ToJson(new PositionMessageModel()
            {
                x = 0,
                y = 0,
                tankId = 0
            })
        };
        client.SendRequest(JsonUtility.ToJson(message));
    }

    public const string Position = "position";
    /*public UnityAction<PositionMessagModel> OnEchoMessage;*/

    public void OnPositionMessage(PositionMessageModel message)
    {
        Debug.Log("Spawn new opponent");
    }

    public const string Movement = "movement";

    public void OnMovementMessage(MovementMessageModel message)
    {
        Debug.Log("Opponent moved");
    }
    
    public void EchoMovementMessage(MovementMessageModel.Action action, bool pressed)
    {
        var message = new MessageModel
        {
            type = "movement",
            message = JsonUtility.ToJson(new MovementMessageModel()
            {
                action = action,
                pressed = pressed
            })
        };
        client.SendRequest(JsonUtility.ToJson(message));
    }

    public void EchoFireMessage()
    {
        var message = new MessageModel
        {
            type = "fire",
            message = JsonUtility.ToJson(new FireMessageModel()
            {
            })
        };
        client.SendRequest(JsonUtility.ToJson(message));
    }

}
