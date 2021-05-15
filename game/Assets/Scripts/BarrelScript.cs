using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelScript : MonoBehaviour
{
    TankController tank;
    public int bombsFired = 0;
    public const int maxBombsTotal = 20;
    public GameObject projectilePrefab;
    public GameObject[] bombs = new GameObject[maxBombsTotal];
    public int maxBombsCurrent = 6;
    float bombSpawnDistance = 1.2f;

    private void Awake()
    {
        tank = GetComponent<TankController>();
    }

    public void Fire()
    {
        if(bombsFired < maxBombsCurrent)
        {  
            Vector3 vec = tank.transform.position + tank.transform.forward * bombSpawnDistance; // distance between a tank and a bomb
            vec.y = 0.8f; // height at which a bomb will be released
            for (int i = 0; i < maxBombsCurrent; i++)
            { 
                if(bombs[i] == null)
                {
                    GameObject bomb = Instantiate(projectilePrefab, vec, transform.rotation);
                    BombScript bombScript = bomb.GetComponent<BombScript>();
                    bombScript.barrelScript = this;
                    bombScript.id = i;
                    bombScript.StartCoroutine(bombScript.BombCoroutine());
                    bombs[i] = bomb;
                    bombsFired++;
                    Debug.Log("Fire. Remaning shots: " + (maxBombsCurrent - bombsFired));
                    break;
                }
            }
        }   
    }
    public void removeBomb(int id)
    {
        bombs[id] = null;
        bombsFired--;
    }
}
