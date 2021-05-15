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
        gameController.healthStatusUI.playersNo++;

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
        if (!gameController.tc[message.tankId].isDead)
        {
            gameController.tc[message.tankId].setPosition(message.x, message.y, message.rotation);
        }
    }

    public const string Movement = "movement";

    public void OnMovementMessage(MovementMessageModel message, GameController gameController)
    {
        int id = message.id;
        if (message.action == MovementMessageModel.Action.Forward)
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

        if (message.action == MovementMessageModel.Action.Forward ||
            message.action == MovementMessageModel.Action.Backward)
        {
            if (message.pressed == false)
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
                id = GameController.your_id
            })
        };
        client.SendRequest(JsonUtility.ToJson(message));
    }

    public const string Fire = "fire";

    public void OnFireMessage(FireMessageModel message, GameController gameController)
    {
        int id = message.id;
        gameController.tc[id].barrelScript.Fire();
    }

    public const string Finish = "finish";

    public void OnStartGameMessage(FinishMessageModel message, GameController gameController)
    {
        Debug.Log("zaczynamy");
        gameController.StartGame();
        // EchoPositionMessage - chyba nie trzeba bo i tak sie odswiezy zaraz
    }

    public const string NewRound = "newRound";

    public void OnNewRoundMessage(NewRoundMessageModel message, GameController gameController)
    {
        Debug.Log("--------");
        Debug.Log(gameController.tc[0].transform.position +
             " _ " + gameController.tc[1].transform.position);
        Debug.Log("Dostalem nowa pozycje i zaczynam gre");
        Debug.Log("Podnosimy czolg o id: " + message.tankId);
        gameController.tc[message.tankId].setAlivePosition(message.x, message.y, message.rotation);
        // EchoPositionMessage - chyba nie trzeba bo i tak sie odswiezy zaraz
    }

    public void EchoFinishMessage()
    {
        var message = new MessageModel
        {
            type = "finish",
            message = JsonUtility.ToJson(new FinishMessageModel()
            {
                id = GameController.your_id,
                context = "finish"
            })
        };
        client.SendRequest(JsonUtility.ToJson(message));
    }

}
