using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Class holding lobby messages.
/// </summary>
public class GameMessaging : BaseMessaging
{
    public GameMessaging(ServerCommunication client) : base(client) { }


    public const string First = "first";


    public void OnFirstMessage(FirstMessageModel received, GameController gameController)
    {
        GameController.your_id = received.id_given;
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
    }


    public const string Position = "position";
    /*public UnityAction<PositionMessagModel> OnEchoMessage;*/

    public void EchoPositionMessage(TankController tank)
    {
        var message = new MessageModel
        {
            type = "position",
            message = JsonUtility.ToJson(new PositionMessageModel()
            {
                x = tank.transform.position.x,
                y = tank.transform.position.z,
                rotation = tank.transform.eulerAngles.y,
                tankId = GameController.your_id
            })
        };
        client.SendRequest(JsonUtility.ToJson(message));
    }

    public void OnPositionMessage(PositionMessageModel message, GameController gameController)
    {
        gameController.tc[message.tankId].setPosition(message.x, message.y, message.rotation);
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

        if(message.action == MovementMessageModel.Action.Forward ||
            message.action == MovementMessageModel.Action.Backward)
        {
            if(message.pressed == false)
            {
                gameController.tc[id].stopTank = true;
            }
        }
    }
    
    public void EchoMovementMessage(MovementMessageModel.Action action, bool pressed, GameController gameController)
    {
        var message = new MessageModel
        {
            type = "movement",
            message = JsonUtility.ToJson(new MovementMessageModel()
            {
                action = action,
                pressed = pressed,
                id = GameController.your_id
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
