using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthStatusUI : MonoBehaviour
{
    TankController tank1;
    TankController tank2;
    TankController tank3;

    public Text tank1Health;
    public Text tank2Health;
    public Text tank3Health;

    private void Awake()
    {
        
    }

    void Start()
    {
        tank1 = GameObject.Find("Tank1").GetComponent<TankController>();
        tank2 = GameObject.Find("Tank2").GetComponent<TankController>();
        tank3 = GameObject.Find("Tank3").GetComponent<TankController>();
    }

    // Update is called once per frame
    void Update()
    {
        int points1 = tank1.points;
        tank1Health.text = "Red " + " " + points1;

        int points2 = tank2.points;
        tank2Health.text = "Green " + " " + points2;

        int points3 = tank3.points;
        tank3Health.text = "Blue " + " " + points3;
    }
}
