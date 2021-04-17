using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    const int maxTanks = 3;
    public TankController[] tanks = new TankController[maxTanks];
    float gravity = -1.0f;
    bool isRoundFinished = false;
    List<float> spawnPositionsX = new List<float>();
    List<float> spawnPositionsZ = new List<float>();
    int spawnPositionsNo = 5;

    private void Awake()
    {
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

        tanks[0] = GameObject.Find("Tank1").GetComponent<TankController>();
        tanks[0].forwardKey = KeyCode.W;
        tanks[0].backwardKey = KeyCode.S;
        tanks[0].rotateLeftKey = KeyCode.A;
        tanks[0].rotateRightKey = KeyCode.D;
        tanks[0].fireKey = KeyCode.Q;

        tanks[1] = GameObject.Find("Tank2").GetComponent<TankController>();
        tanks[1].forwardKey = KeyCode.UpArrow;
        tanks[1].backwardKey = KeyCode.DownArrow;
        tanks[1].rotateLeftKey = KeyCode.LeftArrow;
        tanks[1].rotateRightKey = KeyCode.RightArrow;
        tanks[1].fireKey = KeyCode.RightBracket;

        tanks[2] = GameObject.Find("Tank3").GetComponent<TankController>();
        tanks[2].forwardKey = KeyCode.U;
        tanks[2].backwardKey = KeyCode.J;
        tanks[2].rotateLeftKey = KeyCode.H;
        tanks[2].rotateRightKey = KeyCode.K;
        tanks[2].fireKey = KeyCode.T;
    }

    private void Update()
    {
        for (int i = 0; i < maxTanks; i++)
        {
            if (tanks[i] != null && !tanks[i].isDead)
            {
                if (tanks[i].controller.isGrounded)
                {
                    tanks[i].movingDirection.y = gravity;
                    // Jesli postac nie spada (isGrounded) to moveDirection.y nie powinno rosnac. 
                    // Dlatego "zerujemy" te wartosc. (Inaczej bedzie ciagle malec na skutek -= gravity)
                    // Nie piszemy moveDirection.y = 0, lecz moveDirection.y = -1, bo w innym wypadku controller.isGrounded nie dziala poprawnie. 
                }
                if (Input.GetKey(tanks[i].forwardKey))
                {
                    tanks[i].movingDirection = new Vector3(0, tanks[i].movingDirection.y, 1);
                    tanks[i].movingDirection *= tanks[i].forwardSpeed;
                }
                else if (Input.GetKey(tanks[i].backwardKey))
                {
                    tanks[i].movingDirection = new Vector3(0, tanks[i].movingDirection.y, -1);
                    tanks[i].movingDirection *= tanks[i].backwardSpeed;
                }
                if (Input.GetKeyUp(tanks[i].backwardKey) || Input.GetKeyUp(tanks[i].forwardKey))
                {
                    tanks[i].movingDirection = new Vector3(0, tanks[i].movingDirection.y, 0);
                }
                if (Input.GetKey(tanks[i].rotateRightKey))
                {
                    tanks[i].rotation += tanks[i].rotationSpeed * Time.deltaTime;
                }
                else if (Input.GetKey(tanks[i].rotateLeftKey))
                {
                    tanks[i].rotation -= tanks[i].rotationSpeed * Time.deltaTime;
                }
                if (Input.GetKeyDown(tanks[i].fireKey))
                {
                    tanks[i].barrelScript.Fire();
                }


                tanks[i].movingDirection = tanks[i].transform.TransformDirection(tanks[i].movingDirection);
                tanks[i].controller.Move(tanks[i].movingDirection * Time.deltaTime);
                tanks[i].transform.eulerAngles = new Vector3(0, tanks[i].rotation, 0); // y, bo obrot do okola osi pionowej
                tanks[i].rotation %= 360f;
            }
        }

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
            if (tanks[i].isDead)
            {
                counter++;
            }
        return counter;
    }

    void nextRound()
    {
        System.Random r = new System.Random();

        foreach (TankController tank in tanks)
        {
            int rInt = r.Next(0, spawnPositionsNo);
            float x = spawnPositionsX[rInt];
            rInt = r.Next(0, spawnPositionsNo);
            float z = spawnPositionsZ[rInt];
            int rot = r.Next(0, 360);
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
                if (!tanks[i].isDead)
                {
                    tanks[i].points++;
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
}
