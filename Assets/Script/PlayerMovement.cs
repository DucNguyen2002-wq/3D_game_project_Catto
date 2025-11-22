using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController controller;
    public float walkSpeed = 3f; // Giảm tốc độ cho mèo nhỏ
    public float runSpeed = 6f; // Giảm tốc độ chạy
    public float gravity = -9.81f * 2;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.15f; // Giảm từ 0.4f để phù hợp với mèo nhỏ
    public LayerMask groundMask;

    [Header("Animation")]
    public Animator animator;

    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;

    void Update()
    {
        // Checking if we hit the ground to reset our falling velocity
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction relative to player's facing direction (góc nhìn thứ nhất)
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Check if player is moving
        bool isMoving = move.magnitude > 0.1f;

        // Determine if running (Shift key pressed while moving)
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving && isGrounded;
        
        // Determine if walking (moving but not running)
        bool isWalking = isMoving && !isRunning && isGrounded;

        // Set current speed based on running state
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Move the character
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Update animator parameters
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isRunning", isRunning);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply vertical movement (gravity)
        controller.Move(velocity * Time.deltaTime);
    }
}