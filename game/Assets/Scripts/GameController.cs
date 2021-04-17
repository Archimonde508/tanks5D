using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    const int maxTanks = 3;
    public GameObject[] tanks = new GameObject[maxTanks];
    public TankController[] tc = new TankController[maxTanks];
    float gravity = -1.0f;
    bool isRoundFinished = false;
    List<float> spawnPositionsX = new List<float>();
    List<float> spawnPositionsZ = new List<float>();
    int spawnPositionsNo = 5;
    public GameObject projectilePrefab;
    int currentPlayers = 0;

    [SerializeField]
    private ServerCommunication communication;

    void addPlayer(int id)
    {
        string idStr = (id + 1).ToString();
        Vector3 vec3 = new Vector3(2.0f, 0.5f, 3.0f);
        Vector3 rotVec = new Vector3(0.0f, 0.0f, 0.0f);
        Quaternion rotation = Quaternion.Euler(rotVec);
        tanks[id] = Instantiate(projectilePrefab, vec3, rotation);
        tanks[id].name = "Tank" + idStr;
        tc[id] = tanks[id].GetComponent<TankController>();

        if(id == 0)
        {
            tc[id].forwardKey = KeyCode.W;
            tc[id].backwardKey = KeyCode.S;
            tc[id].rotateLeftKey = KeyCode.A;
            tc[id].rotateRightKey = KeyCode.D;
            tc[id].fireKey = KeyCode.Q;
        }
        if(id == 1)
        {
            tc[id].forwardKey = KeyCode.UpArrow;
            tc[id].backwardKey = KeyCode.DownArrow;
            tc[id].rotateLeftKey = KeyCode.LeftArrow;
            tc[id].rotateRightKey = KeyCode.RightArrow;
            tc[id].fireKey = KeyCode.RightBracket;
        }
        if(id == 2)
        {
            tc[id].forwardKey = KeyCode.U;
            tc[id].backwardKey = KeyCode.J;
            tc[id].rotateLeftKey = KeyCode.H;
            tc[id].rotateRightKey = KeyCode.K;
            tc[id].fireKey = KeyCode.T;
        }


        var cubeRenderer = tanks[id].GetComponent<Renderer>();
        if (id == 0)
        {
            cubeRenderer.material.SetColor("_Color", Color.red);
        }
        if(id == 1)
        {
            cubeRenderer.material.SetColor("_Color", Color.green);
        }
        if (id == 2)
        {
            cubeRenderer.material.SetColor("_Color", Color.blue);
        }
        currentPlayers++;
    }

    private void Awake()
    {
        communication.ConnectToServer();
        addPlayer(currentPlayers);
        addPlayer(currentPlayers);
        addPlayer(currentPlayers);

        // z -34 do 18
        spawnPositionsZ.Add(-34f);
        spawnPositionsZ.Add(18f);
        spawnPositionsZ.Add(9f);
        spawnPositionsZ.Add(2f);
        spawnPositionsZ.Add(-15f);

        spawnPositionsX.Add(13);
        spawnPositionsX.Add(6);
        spawnPositionsX.Add(3);
        spawnPositionsX.Add(-3);
        spawnPositionsX.Add(-8);

        nextRound();
    }

    public void ManageKeyboard()
    {
        for (int i = 0; i < currentPlayers; i++)
        {
            TankController cur = tc[i];
            if (tanks[i] != null && !cur.isDead)
            {
                if (!cur.movingForward && Input.GetKey(cur.forwardKey))
                {
                    communication.SendRequest("forward");
                    cur.movingForward = true;
                }
                if (cur.movingForward && Input.GetKeyUp(cur.forwardKey))
                {
                    communication.SendRequest("stop forward");
                    cur.movingForward = false;
                }

                if (!cur.movingBackward && Input.GetKey(cur.backwardKey))
                {
                    communication.SendRequest("backward");
                    cur.movingBackward = true;
                }
                if (cur.movingBackward && Input.GetKeyUp(cur.backwardKey))
                {
                    communication.SendRequest("stop backward");
                    cur.movingBackward = false;
                }

                if (!cur.turningRight && Input.GetKey(cur.rotateRightKey))
                {
                    communication.SendRequest("right");
                    cur.turningRight = true;
                }
                if (cur.turningRight && Input.GetKeyUp(cur.rotateRightKey))
                {
                    communication.SendRequest("stop right");
                    cur.turningRight = false;
                }

                if (!cur.turningLeft && Input.GetKey(cur.rotateLeftKey))
                {
                    communication.SendRequest("left");
                    cur.turningLeft = true;
                }
                if (cur.turningLeft && Input.GetKeyUp(cur.rotateLeftKey))
                {
                    communication.SendRequest("stop left");
                    cur.turningLeft = false;
                }

                if (Input.GetKeyDown(cur.fireKey))
                {
                    communication.SendRequest("fire");
                    cur.barrelScript.Fire();
                }
            }
        }
    }

    void MoveTank()
    {
        for (int i = 0; i < currentPlayers; i++)
        {
            TankController cur = tc[i];
            if (tanks[i] != null && !cur.isDead)
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

                if (Input.GetKeyUp(cur.backwardKey) || Input.GetKeyUp(cur.forwardKey))
                {
                    cur.movingDirection = new Vector3(0, cur.movingDirection.y, 0);
                }


                if (cur.turningRight)
                {
                    cur.rotation += cur.rotationSpeed * Time.deltaTime;
                }

                if (cur.turningLeft)
                {
                    cur.rotation -= cur.rotationSpeed * Time.deltaTime;
                }

                cur.movingDirection = tanks[i].transform.TransformDirection(cur.movingDirection);
                cur.controller.Move(cur.movingDirection * Time.deltaTime);
                cur.transform.eulerAngles = new Vector3(0, cur.rotation, 0); // y, bo obrot do okola osi pionowej
                tc[i].rotation %= 360f;
            }
        }
    }

    private void Update()
    {
        ManageKeyboard();
        MoveTank();
        
        if (!isRoundFinished)
        {
            if (deadTanks() == maxTanks)
            {
                isRoundFinished = true;
                StartCoroutine(finishRound());
                nextRound();
            }
            if (deadTanks() == maxTanks - 1)
            {
                isRoundFinished = true;
                StartCoroutine(waiter());
            }
        }
    }

    private int deadTanks()
    {
        int counter = 0;
        for (int i = 0; i < maxTanks; i++)
            if (tc[i].isDead)
            {
                counter++;
            }
        return counter;
    }

    void nextRound()
    {
        System.Random r = new System.Random();

        List<float> alreadyUsedX = new List<float>();
        List<float> alreadyUsedZ = new List<float>();
        int licz = 0;
        foreach (TankController tank in tc)
        {
            bool correctSpawn = false;

            int rot = r.Next(0, 360); // TODO
            float x = 0;
            float z = 0;

            while (!correctSpawn)
            {
                if (licz++ > 10000)
                {
                    Debug.Log("enough");
                    return; // debug only
                }
                int rInt = r.Next(0, spawnPositionsNo);
                x = spawnPositionsX[rInt];
                rInt = r.Next(0, spawnPositionsNo);
                z = spawnPositionsZ[rInt];
               
                if (!alreadyUsedX.Any()) {
                    alreadyUsedX.Add(x);
                    alreadyUsedZ.Add(z);
                    correctSpawn = true;
                }
                else
                {
                    if (alreadyUsedX.Contains(x) || alreadyUsedZ.Contains(z))
                    {
                        continue;
                    }
                    else
                    {
                        alreadyUsedX.Add(x);
                        alreadyUsedZ.Add(z);
                        correctSpawn = true;
                    }
                }                
            }
    
            tank.spawnTank();
            tank.setPosition(x, z, rot);
        }
        isRoundFinished = false;
        // Destroy all bombs. Reset bombs limit.
        // block moving at the end
        // rotating doesnt work.
    }

    IEnumerator waiter()
    {
        // 3 second to assure that winner is still alive
        yield return new WaitForSeconds(3);
        Debug.Log(deadTanks() + " oraz " + maxTanks);
        if (deadTanks() == maxTanks)
        {
            StartCoroutine(finishRound());
        }
        else if (deadTanks() == maxTanks - 1)
        {
            for (int i = 0; i < maxTanks; i++)
            {
                if (!tc[i].isDead)
                {
                    tc[i].points++;
                }
            }
            StartCoroutine(finishRound());
        }
        else
        {
            Debug.Log("?????? Blad ??????");
            StartCoroutine(finishRound());
        }
    }

    IEnumerator finishRound()
    {
        Debug.Log("Round is finished.");
        yield return new WaitForSeconds(2);
        nextRound();
    }

    private void OnDestroy()
    {
        // TODO disconnect form server?
    }
}
