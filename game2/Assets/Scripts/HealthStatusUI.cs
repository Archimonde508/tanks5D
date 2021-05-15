using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthStatusUI : MonoBehaviour
{

    public int playersNo = 0;
    TankController[] tanks;

    public Text tank1Health;
    public Text tank2Health;
    public Text tank3Health;

    public Text lobbyPlayersNo;
    bool gameStarted = false;

    public void StartGame(int players, TankController[] tanks)
    {
        this.tanks = tanks;
        playersNo = players;
        gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        lobbyPlayersNo.text = "Players: " + playersNo;
        if (playersNo != 0 && gameStarted)
        {
            int points1 = tanks[0].points;
            tank1Health.text = "Red " + " " + points1;

            if (playersNo > 1)
            {
                int points2 = tanks[1].points;
                tank2Health.text = "Green " + " " + points2;

                if (playersNo > 2)
                {
                    int points3 = tanks[2].points;
                    tank3Health.text = "Blue " + " " + points3;
                }
            }

        }

    }
}
