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
    [SerializeField] private Vector3 spawnOffset = new Vector3(1f, 0, 0); // Default value

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
        Interactable interactable = FindClosestInteractable();
        if (interactable != null && interactable.canInteract)
        {
            if (interactable.requiresKey)
            {
                // Check if the player has the required key to open the door or chest
                if (SharedInventory.Instance.HasCollectible(interactable.requiredKeyId))
                {
                    // Open the door or chest
                    interactable.Interact();
                    Debug.Log("Opened door or chest");
                }
            }
            else
            {
                // Open the door or chest
                interactable.Interact();
                Debug.Log("Opened door or chest");
            }
        }
        else if (this.gameObject.name == "Child")
        {
            Vector3 flamePosition = transform.position + transform.right * (isFacingRight ? flameOffset : -flameOffset);
            flamePosition.y += 0.5f; // Adjust this value to change the height of the flame AOE
            Quaternion flameRotation = Quaternion.Euler(-90, 0, 0);
            GameObject flameEffect = Instantiate(flameEffectPrefab, flamePosition, flameRotation);
            Destroy(flameEffect, flameEffectDuration);
        }
        else if (this.gameObject.name == "European")
        {
            Vector3 adjustedSpawnOffset = new Vector3(isFacingRight ? spawnOffset.x : -spawnOffset.x, spawnOffset.y, spawnOffset.z);
            Vector3 spawnPosition = transform.position + adjustedSpawnOffset;

            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.velocity = transform.right * (isFacingRight ? projectileSpeed : -projectileSpeed);
            Destroy(projectile, projectileDuration);
        }
        Debug.Log("Action performed");
    }
    private Interactable FindClosestInteractable()
    {
        Interactable[] interactables = FindObjectsOfType<Interactable>();
        Interactable closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(currentPosition, interactable.transform.position);
            if (distance < minDistance && interactable.canInteract)
            {
                closest = interactable;
                minDistance = distance;
            }
        }

        return closest;
    }

    private bool HasKey(string keyName)
    {
        // Implement your inventory logic here to check if the player has the required key
        return true; // For testing purposes, return true to assume the player always has the key
    }
}