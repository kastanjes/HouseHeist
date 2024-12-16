using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int playerID = 1; // 1 for Player 1, 2 for Player 2

    public Rigidbody2D rb;
    private Vector2 movement;

    public Animator animator;



    public void Start()
    {

    }

    void Update()
    {
        if (playerID == 1)
        {
            // Controls for Player 1 (e.g., WASD)
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");


        }
        else if (playerID == 2)
        {
            // Controls for Player 2 (e.g., Arrow Keys)
            movement.x = Input.GetAxisRaw("Horizontal2"); // Set up "Horizontal2" in Input Manager
            movement.y = Input.GetAxisRaw("Vertical2");   // Set up "Vertical2" in Input Manager

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }


    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}