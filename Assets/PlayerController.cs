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

    private Rigidbody2D rb;
    private Collider2D col;
    private CollisionDetector2D collisionDetector;
    private float coyoteTimeCounter;
    private Transform currentPlatform;
    private float verticalVelocity;
    private float horizontalVelocity;
    private bool isGrounded;
    private bool isJumping;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction goAction;
    private InputAction stopAction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        collisionDetector = GetComponent<CollisionDetector2D>();

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
            horizontalVelocity = movement.x * moveSpeed * Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        if (isGrounded && jumpAction.triggered && coyoteTimeCounter > 0f)
        {
            isJumping = true;
            verticalVelocity = jumpSpeed;
            coyoteTimeCounter = 0f;
        }
    }

    private void HandleGravity()
    {
        if (collisionDetector.IsGroundedBox())
        {
            coyoteTimeCounter = coyoteTime;
            if (isJumping && verticalVelocity <= 0)
            {
                isJumping = false;
                verticalVelocity = 0f;
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            verticalVelocity -= fallAcceleration * Time.deltaTime;
        }

        // Predict the character's new position
        Vector2 predictedPosition =
            transform.position + new Vector3(0, verticalVelocity * Time.deltaTime);

        Vector2 castOrig = new(col.bounds.center.x, col.bounds.min.y);
        Vector2 castTarget =
            new(predictedPosition.x, predictedPosition.y - col.bounds.size.y / 2 - 1);
        var hit = Physics2D.Linecast(castOrig, castTarget, collisionDetector.groundLayer);

        Debug.DrawLine(castOrig, castTarget, Color.green);
        if (hit.collider != null && verticalVelocity < 0)
        {
            Debug.DrawLine(castOrig, hit.point, Color.red, 5);
            // Snap character to the ground if a collision is detected
            predictedPosition.y = hit.point.y + (col.bounds.size.y / 2) - col.offset.y;
            verticalVelocity = 0f;
        }

        // Move the character on the y-axis
        transform.position = new Vector2(transform.position.x, predictedPosition.y);
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleGravity();
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
