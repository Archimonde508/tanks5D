using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class GameController : MonoBehaviour
{
    const int maxTanks = 3;
    public GameObject[] tanks = new GameObject[maxTanks];
    public TankController[] tc = new TankController[maxTanks];
    readonly float gravity = -1.0f;
    bool isRoundFinished = false;
    public GameObject projectilePrefab;
    int currentPlayers = 0;
    static public int your_id;
    public bool gameStarted = false;
    public HealthStatusUI healthStatusUI;

    [SerializeField]
    private ServerCommunication communication;

    public void addPlayer(int id, int x, int z)
    {
        string idStr = (id + 1).ToString();
        Vector3 vec3 = new Vector3(x, 0.5f, z);
        Vector3 rotVec = new Vector3(0.0f, 0.0f, 0.0f);
        Quaternion rotation = Quaternion.Euler(rotVec);
        tanks[id] = Instantiate(projectilePrefab, vec3, rotation);
        tanks[id].name = "Tank" + idStr;
        tc[id] = tanks[id].GetComponent<TankController>();

        if (id == your_id)
        {
            tc[id].forwardKey = KeyCode.W;
            tc[id].backwardKey = KeyCode.S;
            tc[id].rotateLeftKey = KeyCode.A;
            tc[id].rotateRightKey = KeyCode.D;
            tc[id].fireKey = KeyCode.Q;
        }


        var cubeRenderer = tanks[id].GetComponent<Renderer>();
        if (id == 0)
        {
            cubeRenderer.material.SetColor("_Color", Color.red);
        }
        if (id == 1)
        {
            cubeRenderer.material.SetColor("_Color", Color.green);
        }
        if (id == 2)
        {
            cubeRenderer.material.SetColor("_Color", Color.blue);
        }
        currentPlayers++;
        tc[id].setPosition(x, z, 0);
    }

    private void Awake()
    {
        communication.ConnectToServer();
        StartCoroutine("sendPosition");
    }

    IEnumerator sendPosition()
    {
        while (true)
        {
            TankController cur = tc[your_id];
            if (cur != null)
            {
                if (!cur.isDead)
                {
                    communication.GameMsg.EchoPositionMessage(cur);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ManageKeyboard()
    {
        TankController cur = tc[your_id];

        if (tanks[your_id] != null && !cur.isDead && cur.movingEnabled)
        {
            GameMessaging gm = communication.GameMsg;
            if (!cur.movingForward && Input.GetKey(cur.forwardKey))
            {
                gm.EchoMovementMessage(MovementMessageModel.Action.Forward, true, this);
                cur.movingForward = true;
            }
            if (cur.movingForward && Input.GetKeyUp(cur.forwardKey))
            {
                gm.EchoMovementMessage(MovementMessageModel.Action.Forward, false, this);
                cur.movingForward = false;
                cur.stopTank = true;
            }

            if (!cur.movingBackward && Input.GetKey(cur.backwardKey))
            {
                gm.EchoMovementMessage(MovementMessageModel.Action.Backward, true, this);
                cur.movingBackward = true;
            }
            if (cur.movingBackward && Input.GetKeyUp(cur.backwardKey))
            {
                gm.EchoMovementMessage(MovementMessageModel.Action.Backward, false, this);
                cur.movingBackward = false;
                cur.stopTank = true;
            }

            if (!cur.turningRight && Input.GetKey(cur.rotateRightKey))
            {
                gm.EchoMovementMessage(MovementMessageModel.Action.Rot_right, true, this);
                cur.turningRight = true;
            }
            if (cur.turningRight && Input.GetKeyUp(cur.rotateRightKey))
            {
                gm.EchoMovementMessage(MovementMessageModel.Action.Rot_right, false, this);
                cur.turningRight = false;
            }

            if (!cur.turningLeft && Input.GetKey(cur.rotateLeftKey))
            {
                gm.EchoMovementMessage(MovementMessageModel.Action.Rot_left, true, this);
                cur.turningLeft = true;
            }
            if (cur.turningLeft && Input.GetKeyUp(cur.rotateLeftKey))
            {
                gm.EchoMovementMessage(MovementMessageModel.Action.Rot_left, false, this);
                cur.turningLeft = false;
            }

            if (Input.GetKeyDown(cur.fireKey))
            {
                gm.EchoFireMessage();
                cur.barrelScript.Fire();
            }
            if (Input.GetKeyUp(KeyCode.Z))
            {
                for (int i = 0; i < currentPlayers; i++)
                {
                    Debug.Log(
                        "(" + currentPlayers + ")" + "[" + i + "]" +
                        " " + tanks[i].transform.rotation +
                        " | " + "|" +
                        tc[i].movingBackward + tc[i].movingForward +
                        tc[i].turningLeft + tc[i].turningRight
                        );
                }
            }
        }
    }

    void MoveTank()
    {
        for (int i = 0; i < currentPlayers; i++)
        {
            TankController cur = tc[i];
            if (tanks[i] != null && !cur.isDead && cur.movingEnabled)
            {
                if (cur.controller.isGrounded)
                {
                    cur.movingDirection.y = gravity;
                    // Jesli postac nie spada (isGrounded) to moveDirection.y nie powinno rosnac. 
                    // Dlatego "zerujemy" te wartosc. (Inaczej bedzie ciagle malec na skutek -= gravity)
                    // Nie piszemy moveDirection.y = 0, lecz moveDirection.y = -1, bo w innym wypadku controller.isGrounded nie dziala poprawnie. 
                }

                if (cur.movingForward)
                {
                    cur.movingDirection = new Vector3(0, cur.movingDirection.y, 1);
                    cur.movingDirection *= cur.forwardSpeed;
                }

                if (cur.movingBackward)
                {
                    cur.movingDirection = new Vector3(0, cur.movingDirection.y, -1);
                    cur.movingDirection *= cur.backwardSpeed;
                }

                if (cur.stopTank)
                {
                    cur.movingDirection = new Vector3(0, cur.movingDirection.y, 0);
                    cur.stopTank = false;
                }

                cur.movingDirection = tanks[i].transform.TransformDirection(cur.movingDirection);

                cur.controller.Move(cur.movingDirection * Time.deltaTime);

                if (i == your_id) //  rotate our tank only
                {
                    if (cur.turningRight)
                    {
                        cur.rotation += cur.rotationSpeed * Time.deltaTime;
                    }

                    if (cur.turningLeft)
                    {
                        cur.rotation -= cur.rotationSpeed * Time.deltaTime;
                    }

                    cur.transform.eulerAngles = new Vector3(0, cur.rotation, 0);
                    cur.rotation %= 360f;
                }
            }
        }
    }

    private void Update()
    {
        if (gameStarted)
        {
            ManageKeyboard();
            MoveTank();

            // checking if round is finished
            if (!isRoundFinished)
            {
                if (deadTanks() == currentPlayers)
                {
                    isRoundFinished = true;
                    finishRound();
                }
                else if (deadTanks() == currentPlayers - 1)
                {
                    isRoundFinished = true;
                    StartCoroutine(waiter());
                }
            }
        }
    }

    private int deadTanks()
    {
        int counter = 0;
        for (int i = 0; i < currentPlayers; i++)
            if (tc[i].isDead)
            {
                counter++;
            }
        return counter;
    }

    IEnumerator waiter()
    {
        // 3 seconds to check if winner is still alive
        yield return new WaitForSeconds(3);
        if (deadTanks() == currentPlayers)
        {
            finishRound();
        }
        else if (deadTanks() == currentPlayers - 1) // if one tank is still alive
        {
            Debug.Log("Jest git - jeden przezyl");
            for (int i = 0; i < currentPlayers; i++)
            {
                if (!tc[i].isDead)
                {
                    tc[i].points++;
                }
            }
            finishRound();
        }
        else
        {
            Debug.Log("Blad??? Niepoprawna liczba graczy pod koniec rundy.");
        }
    }

    private void finishRound()
    {
        Debug.Log("Round is finished.");

        GameMessaging gm = communication.GameMsg;
        gm.EchoFinishMessage();
        for (int i = 0; i < currentPlayers; i++)
        {
            for (int j = 0; j < tc[i].barrelScript.maxBombsCurrent; j++)
            {
                if (tc[i].barrelScript.bombs[j] != null)
                {
                    tc[i].barrelScript.bombs[j].GetComponent<BombScript>().DestroyBomb(); // remove all bombs
                }
            }
            tc[i].movingEnabled = false;

            tc[i].turningLeft = false;
            tc[i].turningRight = false;
            tc[i].movingBackward = false;
            tc[i].movingForward = false;

            tc[i].isDead = false;
            tc[i].died = false;
            tc[i].currentHealth = tc[i].maxHealth;

            Debug.Log("i: " + tc[i].transform.position);
        }
        isRoundFinished = false;
    }

    private void OnDestroy()
    {
        // TODO disconnect form server?
    }

    public void StartGame()
    {
        gameStarted = true;
        healthStatusUI.StartGame(currentPlayers, tc);
    }
}
