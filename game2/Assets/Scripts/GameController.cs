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
    static public int your_id;

    [SerializeField]
    private ServerCommunication communication;

    public void addPlayer(int id, int x, int z)
    {
        string idStr = (id + 1).ToString();
        Vector3 vec3 = new Vector3(2.0f, 0.5f, 3.0f);
        Vector3 rotVec = new Vector3(0.0f, 0.0f, 0.0f);
        Quaternion rotation = Quaternion.Euler(rotVec);
        tanks[id] = Instantiate(projectilePrefab, vec3, rotation);
        tanks[id].name = "Tank" + idStr;
        tc[id] = tanks[id].GetComponent<TankController>();

        if(id == your_id)
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
        if(id == 1)
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
        //nextRound();
    }

    IEnumerator sendPosition()
    {
        while(true)
        {
            TankController cur = tc[your_id];
            if (cur != null)
            {
                communication.GameMsg.EchoPositionMessage(cur);
            }
            yield return new WaitForSeconds(1f);
        }
        
    }

    public void ManageKeyboard()
    {

        TankController cur = tc[your_id];

        if (tanks[your_id] != null && !cur.isDead)
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

                if (cur.stopTank)
                {
                    cur.movingDirection = new Vector3(0, cur.movingDirection.y, 0);
                    cur.stopTank = false;
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

        //if (!isRoundFinished)
        //{
        //    if (deadTanks() == maxTanks)
        //    {
        //        isRoundFinished = true;
        //        StartCoroutine(finishRound());
        //        nextRound();
        //    }
        //    if (deadTanks() == maxTanks - 1)
        //    {
        //        isRoundFinished = true;
        //        StartCoroutine(waiter());
        //    }
        //}
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
        //Debug.Log(deadTanks() + " oraz " + maxTanks);
        //if (deadTanks() == maxTanks)
        //{
        //    StartCoroutine(finishRound());
        //}
        //else if (deadTanks() == maxTanks - 1)
        //{
        //    for (int i = 0; i < maxTanks; i++)
        //    {
        //        if (!tc[i].isDead)
        //        {
        //            tc[i].points++;
        //        }
        //    }
        //    StartCoroutine(finishRound());
        //}
        //else
        //{
        //    Debug.Log("?????? Blad ??????");
        //    StartCoroutine(finishRound());
        //}
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
