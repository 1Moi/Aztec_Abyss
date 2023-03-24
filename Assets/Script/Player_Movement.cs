using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 8;

    private bool isGrounded = false;
    private CharacterController controller;
    private Vector3 moveDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get player input
        float moveX = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(moveX * speed, moveDirection.y, 0f);

        // Apply gravity
        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = 0f;
        }
        moveDirection.y += Physics.gravity.y * Time.deltaTime;

        // Move the player
        controller.Move(moveDirection * Time.deltaTime);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            moveDirection.y = jumpForce;
        }

        // Update grounded flag
        isGrounded = controller.isGrounded;
    }
}