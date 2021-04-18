using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class holding lobby messages.
/// </summary>
public class GameMessaging : BaseMessaging
{
    public GameMessaging(ServerCommunication client) : base(client) { }


    public const string First = "first";


    public void OnFirstMessage(FirstMessageModel received, GameController gameController)
    {
        gameController.your_id = received.id_given;
    }


    public const string Init = "init";
    /*public UnityAction OnConnectedToServer;*/

    

    public void OnConnectedToServer(InitMessageModel received, GameController gameController)
    {

        // set gamecontroller

        int x = received.x;
        int z = received.y;
        int id = received.tankId;
        gameController.addPlayer(id, x, z);
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

    public void OnMovementMessage(MovementMessageModel message, GameController gameController)
    {
        int id = message.id;
        if(message.action == MovementMessageModel.Action.Forward)
        {
            gameController.tc[id].movingForward = message.pressed;
        }
        if (message.action == MovementMessageModel.Action.Backward)
        {
            gameController.tc[id].movingBackward = message.pressed;
        }
        if (message.action == MovementMessageModel.Action.Rot_right)
        {
            gameController.tc[id].turningRight = message.pressed;
        }
        if (message.action == MovementMessageModel.Action.Rot_left)
        {
            gameController.tc[id].turningLeft = message.pressed;
        }
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
