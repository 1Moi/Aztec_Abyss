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
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject flameEffectPrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float flameEffectDuration = 2f;
    [SerializeField] private float projectileDuration = 2f;
    [SerializeField] private float flameOffset = 0.5f;

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

        if (isFacingRight && moveInput < 0)
            Flip();
        else if (!isFacingRight && moveInput > 0)
            Flip();

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.AddForce(Vector3.up * jumpForce);

        if (Input.GetButtonDown("Action"))
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
        if (this.gameObject.name == "Child")
        {
            Vector3 flamePosition = transform.position + transform.right * (isFacingRight ? flameOffset : -flameOffset);
            flamePosition.y += 0.5f; // Adjust this value to change the height of the flame AOE
            Quaternion flameRotation = Quaternion.Euler(-90, 0, 0);
            GameObject flameEffect = Instantiate(flameEffectPrefab, flamePosition, flameRotation);
            Destroy(flameEffect, flameEffectDuration);
        }
        else if (this.gameObject.name == "European")
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.velocity = transform.right * (isFacingRight ? projectileSpeed : -projectileSpeed);
            Destroy(projectile, projectileDuration);
        }
        Debug.Log("Action performed");
    }
}