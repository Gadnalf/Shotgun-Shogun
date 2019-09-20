using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public float constantVelocity;
    public Vector2 direction;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = constantVelocity * direction;
        rb.isKinematic = true;
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageScript = collision.otherCollider.GetComponentInParent<IDamageable>();
        IHitable hitScript = collision.otherCollider.GetComponentInParent<IHitable>();
        if (damageScript != null)
        {
            damageScript.Damage(1);
            hitScript.Hit(direction, 1);
        }
    }
}
