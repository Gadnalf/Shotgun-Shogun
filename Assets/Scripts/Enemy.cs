using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    int health;

    // Start is called before the first frame update
    void Start()
    {
        health = 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int val)
    {
        health -= val;
        Debug.Log("Health: " + health);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
