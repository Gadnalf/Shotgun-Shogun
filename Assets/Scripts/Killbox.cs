using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<IKillable>();
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().Kill();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
