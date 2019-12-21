using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
 * Manages player input and states
 */
public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private Collider2D col;
    public float jumpVelocity = 7;
    public float runVelocity = 4;
    public float walkVelocity = 2;
    public float fallMod = 1.5f;
    public float maxSlideTime = 1;
    public float brakeSpeed = 0.8f;
    public float accel = 0.2f;

    // Character state data, updated every frame
    private string playerState;
    private int stateTime;
    public bool FacingRight { get; private set; }

    // Context information
    public bool Grounded { get; private set; }
    float horizontalMovement;
    private bool touchingLeft;
    private bool touchingRight;

    // Input data
    private float horizontalInput;
    private bool jumpPressed;
    private bool jumpHeld;
    private bool duckPressed;
    private bool duckHeld;
    private bool shoot;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        CheckBounds();
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) { jumpPressed = true; }
        jumpHeld = Input.GetButton("Jump");
        if (duckHeld = Input.GetAxisRaw("Vertical") < 0) { duckHeld = true; };
        shoot = Input.GetButtonDown("Fire1");
    }

    private void FixedUpdate()
    {
        float horizontalMovement = rb.velocity.x;
        float verticalMovement = rb.velocity.y;
        
        // Set facing (for sprite purposes)
        if (horizontalInput < 0)
        {
            FacingRight = false;
        }
        else if (horizontalInput > 0)
        {
            FacingRight = true;
        }

        // Grounded jump
        if (jumpPressed)
        {
            if (Grounded)
                {
                    verticalMovement = jumpVelocity;
                }
            else if (touchingLeft)
                {
                    verticalMovement = (float) Math.Sqrt(jumpVelocity * jumpVelocity);
                    horizontalMovement = jumpVelocity;
                    playerState = "leaping";
                }
            else if (touchingRight)
                {
                    verticalMovement = (float)Math.Sqrt(jumpVelocity * jumpVelocity);
                    horizontalMovement = -jumpVelocity;
                    playerState = "leaping";
                }
        }
        jumpPressed = false;

        rb.velocity = new Vector2(horizontalMovement, verticalMovement);

        // Faster falling for more weightiness
        if (!jumpHeld || rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMod - 1) * Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Platform")
        {
            transform.parent = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Platform")
        {
            transform.parent = null;
        }
    }

    // Updates state data if player is touching anything
    private void CheckBounds()
    {
        float offset = 0.03f;
        // These all work by drawing a line slightly offset from the edges of the collider, then checking for objects hit.
        // Can be hypothetically modified to check for types of platforms and shit.

        Vector2 bottomLeft = new Vector2(transform.position.x - col.bounds.extents.x, transform.position.y - col.bounds.extents.y);
        Vector2 bottomRight = new Vector2(transform.position.x + col.bounds.extents.x, transform.position.y - col.bounds.extents.y);
        Grounded = false;
        // Debug.Log(bottom.x + ", " + bottom.y);
        RaycastHit2D[] results = Physics2D.LinecastAll(bottomLeft + new Vector2(0, -offset), bottomRight + new Vector2(0, -offset));
        foreach (RaycastHit2D result in results){
            if (result.collider != col && !result.collider.isTrigger) {
                Grounded = true;
                //Debug.Log("BEEP BOOP I'M TOUCHING THE GROUND");
            }
        }

        Vector2 leftTop = new Vector2(transform.position.x - col.bounds.extents.x, transform.position.y + col.bounds.extents.y);
        Vector2 leftBottom = new Vector2(transform.position.x - col.bounds.extents.x, transform.position.y - col.bounds.extents.y);
        touchingLeft = false;
        // Debug.Log(bottom.x + ", " + bottom.y);
        results = Physics2D.LinecastAll(leftTop + new Vector2(-offset, 0.1f), leftBottom + new Vector2(-offset, 0.1f));
        foreach (RaycastHit2D result in results)
        {
            if (result.collider != col && !result.collider.isTrigger)
            {
                touchingLeft = true;
                //Debug.Log("BEEP BOOP I'M TOUCHING THE LEFT WALL");
            }
        }

        Vector2 rightTop = new Vector2(transform.position.x + col.bounds.extents.x, transform.position.y + col.bounds.extents.y);
        Vector2 rightBottom = new Vector2(transform.position.x + col.bounds.extents.x, transform.position.y - col.bounds.extents.y);
        touchingRight = false;
        // Debug.Log(bottom.x + ", " + bottom.y);

        results = Physics2D.LinecastAll(rightTop + new Vector2(offset, 0.1f), rightBottom + new Vector2(offset, 0.1f));
        foreach (RaycastHit2D result in results)
        {
            if (result.collider != col && !result.collider.isTrigger)
            {
                touchingRight = true;
                //Debug.Log("BEEP BOOP I'M TOUCHING THE RIGHT WALL");
            }
        }
    }

    public float GetHorizontalSpeed()
    {
        return Math.Abs(rb.velocity.x);
    }

    public void Kill()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
