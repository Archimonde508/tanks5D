using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Forefront class for the server communication.
/// </summary>
public class ServerCommunication : MonoBehaviour
{
    // Server IP address
    [SerializeField]
    private string hostIP = "tanks-2d-online.herokuapp.com";

    // Server port
    [SerializeField]
    private int port = 3000;
    
    // Flag to use localhost
    [SerializeField]
    private bool useLocalhost = true;

    // Address used in code
    private string host => useLocalhost ? "localhost" : hostIP;
    // Final server address
    private string server;

    // WebSocket Client
    private WsClient client = new WsClient("ws://tanks-2d-online.herokuapp.com");

    // Class with messages for "game"
    public GameMessaging GameMsg { private set; get; }

    public GameController gameController;
    /// <summary>
    /// Unity method called on initialization
    /// </summary>
    private void Awake()
    {
        //gameController = GetComponent<GameController>();
        server = "ws://" + host + ":" + port;
        Debug.Log(server);
        //client = new WsClient(server);

        // Messaging
        GameMsg = new GameMessaging(this);
    }

    /// <summary>
    /// Unity method called every frame
    /// </summary>
    private void Update()
    {
        // Check if server send new messages
        var cqueue = client.receiveQueue;
        string msg;
        while (cqueue.TryPeek(out msg))
        {
            // Parse newly received messages
            cqueue.TryDequeue(out msg);
            HandleMessage(msg);
        }
    }

    /// <summary>
    /// Method responsible for handling server messages
    /// </summary>
    /// <param name="msg">Message.</param>
    private void HandleMessage(string msg)
    {
        //Debug.Log("Server: " + msg);

        // Deserializing message from the server
        var message = JsonUtility.FromJson<MessageModel>(msg);

        // Picking correct method for message handling
        //Debug.Log(message);
        //Debug.Log(message.type);

        switch (message.type)
        {
            case GameMessaging.First:
                GameMsg.OnFirstMessage(
                     JsonUtility.FromJson<FirstMessageModel>(message.message),
                     gameController
                    );
                break;
            case GameMessaging.Init:
                GameMsg.OnConnectedToServer(
                    JsonUtility.FromJson<InitMessageModel>(message.message),
                    gameController
                    );
                break;
            case GameMessaging.Position:
                GameMsg.OnPositionMessage(JsonUtility.FromJson<PositionMessageModel>(message.message),
                    gameController);
                break;
            case GameMessaging.Movement:
                GameMsg.OnMovementMessage(
                    JsonUtility.FromJson<MovementMessageModel>(message.message),
                    gameController
                    );
                break;
            case GameMessaging.Fire:
                GameMsg.OnFireMessage(
                    JsonUtility.FromJson<FireMessageModel>(message.message),
                    gameController
                    );
                break;
            case GameMessaging.NewRound:
                GameMsg.OnNewRoundMessage(
                   JsonUtility.FromJson<NewRoundMessageModel>(message.message),
                   gameController
                   );
                break;
            case GameMessaging.Finish: //start game
                GameMsg.OnStartGameMessage(
                   JsonUtility.FromJson<FinishMessageModel>(message.message),
                   gameController
                   );
                break;
            default:
                Debug.LogError("Unknown type of method: " + message.type);
                break;
        }
    }

    /// <summary>
    /// Call this method to connect to the server
    /// </summary>
    public async void ConnectToServer()
    {
        await client.Connect();
    }

    /// <summary>
    /// Method which sends data through websocket
    /// </summary>
    /// <param name="message">Message.</param>
    public void SendRequest(string message)
    {
        client.Send(message);
    }

    private void OnDestroy()
    {
        Thread disconnectThread = new Thread(client.onDestroy);
        disconnectThread.Start();
        disconnectThread.Join();
    }
}
