using System;
using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerController playerController;
    private GunControl gunControl;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.flipX = gunControl.facingLeft;
        //animator.SetBool("isWalking", (playerController.GetHorizontalSpeed() > 0.3f) && playerController.Grounded);
        //animator.SetFloat("horizontalSpeed", playerController.GetHorizontalSpeed());
        //animator.SetBool("isJumping", playerController.jumpPressed);
    }
}
