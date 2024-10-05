using System.Collections.Generic;
using System.Security.Cryptography;
using Mono.Cecil;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;
    public float coyoteTime = 0.2f; // Coyote time duration
    public float fallAcceleration = 4f; // Units (pixels) per second
    public float maxFallSpeed = 8f; // Units (pixels) per second
    public float commandRange = 128f;

    private Rigidbody2D rb;
    private Collider2D col;
    private CollisionDetector2D collisionDetector;
    private float coyoteTimeCounter;
    private Transform currentPlatform;
    private float verticalVelocity;
    private float horizontalVelocity;
    private bool isGrounded;
    private bool jumpThisFrame;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction goAction;
    private InputAction stopAction;

    private SheepController[] sheepControllers;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        collisionDetector = GetComponent<CollisionDetector2D>();
        sheepControllers = FindObjectsByType<SheepController>(FindObjectsSortMode.None);
        animator = GetComponent<Animator>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        goAction = InputSystem.actions.FindAction("Command: Go");
        stopAction = InputSystem.actions.FindAction("Command: Stop");
    }

    void HandleMovement()
    {
        var moveValue = moveAction.ReadValue<Vector2>().x;
        if (moveValue.CompareTo(0) != 0)
        {
            var movement = moveValue > 0 ? Vector2.right : Vector2.left;

            if (movement.x > 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
            else
            {
                transform.localScale = new Vector2(-1, 1);
            }

            horizontalVelocity = movement.x * moveSpeed * Time.deltaTime;

            if (collisionDetector.IsTouchingWall(movement.x))
            {
                horizontalVelocity = 0;
            }
            else
            {
                transform.Translate(horizontalVelocity, 0, 0);
            }

            animator.SetBool("is_running", true);
        }
        else
        {
            animator.SetBool("is_running", false);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && jumpAction.triggered && coyoteTimeCounter > 0f)
        {
            jumpThisFrame = true;
            verticalVelocity = jumpSpeed;
            coyoteTimeCounter = 0f;

            animator.SetTrigger("jump");
        }
        else
        {
            jumpThisFrame = false;
        }
    }

    private void HandleGravity()
    {
        if (collisionDetector.IsGroundedBox() && verticalVelocity.CompareTo(0) <= 0)
        {
            coyoteTimeCounter = coyoteTime;
            verticalVelocity = 0f;
            isGrounded = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            verticalVelocity -= fallAcceleration * Time.deltaTime;
            isGrounded = false;
        }

        Vector2 predictedPosition =
            transform.position + new Vector3(0, verticalVelocity * Time.deltaTime);

        Vector2 castOrig = new(col.bounds.center.x, col.bounds.min.y);
        Vector2 castTarget =
            new(predictedPosition.x, predictedPosition.y - col.bounds.size.y / 2 - 1);
        var hit = Physics2D.Linecast(castOrig, castTarget, collisionDetector.groundLayer);

        if (hit.collider != null && verticalVelocity < 0)
        {
            // Snap character to the ground if a collision is detected
            predictedPosition.y = hit.point.y + (col.bounds.size.y / 2) - col.offset.y;
            verticalVelocity = 0f;
        }

        // Move the character on the y-axis
        transform.position = new Vector2(transform.position.x, predictedPosition.y);
    }

    void HandleCommands()
    {
        if (goAction.triggered)
        {
            foreach (var sheepController in FindSheepInRange())
            {
                sheepController.ReceiveCommand(SheepCommand.Go, transform.position);
            }
        }
        if (stopAction.triggered)
        {
            foreach (var sheepController in FindSheepInRange())
            {
                sheepController.ReceiveCommand(SheepCommand.Stop, transform.position);
            }
        }
    }

    SheepController[] FindSheepInRange()
    {
        var sheepInRange = new List<SheepController>();
        foreach (var sheepController in sheepControllers)
        {
            if (
                Vector2.Distance(sheepController.transform.position, transform.position)
                < commandRange
            )
            {
                sheepInRange.Add(sheepController);
            }
        }
        return sheepInRange.ToArray();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleGravity();
        HandleCommands();

        animator.SetFloat("vertical_speed", verticalVelocity);
        animator.SetBool("is_grounded", isGrounded);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = collision.transform;
            transform.SetParent(currentPlatform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }
}
