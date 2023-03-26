using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 700f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;

    private Rigidbody rb;
    private bool isGrounded;
    private float moveInput;
    private bool isFacingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);

        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector3(moveInput * moveSpeed, rb.velocity.y, 0);
        rb.position = new Vector3(rb.position.x, rb.position.y, 0); // Lock the Z-axis position

        if (isFacingRight && moveInput < 0)
            Flip();
        else if (!isFacingRight && moveInput > 0)
            Flip();

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.AddForce(Vector3.up * jumpForce);

        if (Input.GetButtonDown("Fire1"))
            PerformAction();
    }


    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void PerformAction()
    {
        Debug.Log("Action performed");
    }
}