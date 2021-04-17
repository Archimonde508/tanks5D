using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float backwardSpeed = 2.5f;
    public float rotationSpeed = 135f;
    public float rotation = 0;
    public bool isDead = false;
    bool died = false;
    public Vector3 movingDirection = Vector3.zero;
    public CharacterController controller;
    public BarrelScript barrelScript;
    public int maxHealth = 1;
    public int points = 0;
    public int currentHealth;
    public KeyCode forwardKey;
    public KeyCode backwardKey;
    public KeyCode rotateLeftKey;
    public KeyCode rotateRightKey;
    public KeyCode fireKey;
    public bool zmienna;
    public GameObject explosionEffect;
   

    private void Update()
    {
        if (controller.isGrounded)
        {
            zmienna = true;
        }
        else
        {
            zmienna = false;
        }
        if (!isDead && currentHealth <= 0)
        {
            isDead = true;
        }
        if (isDead && !died)
        {
            died = true; //call this only once

            GetComponent<AudioSource>().time = 0.4f;
            GetComponent<AudioSource>().Play();
            GameObject explosionExample = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosionExample, 8);

            if (this.name == "Tank1")
            {
                Debug.Log("Czerwony wraca na spawn");
            }
            else if (this.name == "Tank2")
            {
                Debug.Log("Zielony wraca na spawn");
            }
            else if (this.name == "Tank3")
            {
                Debug.Log("Niebieski wraca na spawn");
            }
            
            transform.position = new Vector3(0.0f, -2.0f, 0.0f);

        }
        if (Input.GetKey(KeyCode.X))
        {
            Debug.Log(transform.position + this.name);
        }
    }
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        barrelScript = GetComponent<BarrelScript>();
        spawnTank();
    }

    public void spawnTank()
    {
        currentHealth = maxHealth;
        died = false;
        isDead = false;
    }

    public void setPosition(float x, float z, float rot)
    {
        controller.enabled = false;
        transform.position = new Vector3(x, 0.5f, z);
        transform.Rotate(0, rot, 0);
        controller.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bomb"))
        {
            Debug.Log("Col with bomb!");
            other.gameObject.GetComponent<BombScript>().DestroyBomb();
            ReduceHealth();
        }
    }

    void ReduceHealth()
    {
        if(!isDead && currentHealth > 0)
        {
            currentHealth--;
        }
    }




}
