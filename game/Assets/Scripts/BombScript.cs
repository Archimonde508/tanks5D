using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public float force = 500;
    public float bombTime = 10.0f;
    public BarrelScript barrelScript;
    public int id;
    public bool isDestroyed = false;

    private void Awake()
    {
        GetComponent<AudioSource>().time = 0.5f;
    }
    private void Start()
    {

    }
    public IEnumerator BombCoroutine()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * force);
        yield return new WaitForSeconds(bombTime);
        DestroyBomb();
    }

    public void DestroyBomb()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            barrelScript.removeBomb(id);
            Destroy(this.gameObject);
        }
    }
}
