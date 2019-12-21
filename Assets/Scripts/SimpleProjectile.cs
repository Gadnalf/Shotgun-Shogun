using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public float constantVelocity;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.right.normalized * constantVelocity;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageScript = collision.gameObject.GetComponentInParent<IDamageable>();
        IHitable hitScript = collision.gameObject.GetComponentInParent<IHitable>();
        if (damageScript != null)
        {
            damageScript.Damage(1);
        }
        if (hitScript != null)
        {
            Vector3 direction = Vector3.zero;
            hitScript.Hit(direction, 1);
        }
        Destroy(gameObject);
        Debug.Log("hut");
    }
}