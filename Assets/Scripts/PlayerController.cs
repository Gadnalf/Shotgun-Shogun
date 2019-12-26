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
    private GunControl gunCtrl;
    public float jumpVelocity = 7;
    public float walkVelocity = 2;
    public float airVelocity = 0.25f;
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
    private Vector2 recoil;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        gunCtrl = GetComponent<GunControl>();
    }

    private void Update()
    {
        CheckBounds();
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) { jumpPressed = true; }
        jumpHeld = Input.GetButton("Jump");
        if (duckHeld = Input.GetAxisRaw("Vertical") < 0) { duckHeld = true; };
        if (Input.GetButtonDown("Fire1"))
        {
            shoot = true;
        }
    }

    private void FixedUpdate()
    {
        float horizontalMovement = rb.velocity.x - rb.velocity.x * Time.deltaTime;
        float verticalMovement = rb.velocity.y;

        // Walking and air control
        if (Grounded)
        {
            horizontalMovement = horizontalInput * walkVelocity;
        }
        else
        {
            if (horizontalMovement > 0 && horizontalMovement + horizontalInput * airVelocity < horizontalInput * airVelocity ||
                horizontalMovement < 0 && horizontalMovement + horizontalInput * airVelocity > horizontalInput * airVelocity)
            {
                horizontalMovement = horizontalInput * airVelocity;
            }
            else
            {
                horizontalMovement = GetMaxOfAbs(horizontalMovement + horizontalInput * airVelocity, horizontalInput * airVelocity);
            }
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
                verticalMovement = jumpVelocity;
                horizontalMovement = jumpVelocity * 0.75f;
                playerState = "leaping";
            }
            else if (touchingRight)
            {
                verticalMovement = jumpVelocity;
                horizontalMovement = -jumpVelocity * 0.75f;
                playerState = "leaping";
            }
        }
        jumpPressed = false;

        rb.velocity = new Vector2(horizontalMovement, verticalMovement);

        // Gun stuff
        if (shoot)
        {
            recoil = gunCtrl.Shoot();
            rb.velocity = rb.velocity + recoil;

            shoot = false;
        }

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
        // Can be modified to check for types of platforms and shit. Should maybe expand detection.

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

    public float GetMaxOfAbs(float a, float b)
    {
        if (Math.Abs(a) > Math.Abs(b)) {
            return a;
        }
        return b;
    }

    public void Kill()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
