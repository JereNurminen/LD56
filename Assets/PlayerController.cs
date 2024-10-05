using Mono.Cecil;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

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

    void Update()
    {
        var groundCollisions = collisionDetector.CheckForGround();
        var wasGrounded = isGrounded;

        /*
        *  Update inputs
        */
        var moveValue = moveAction.ReadValue<Vector2>().x;
        var jumpTriggered = jumpAction.triggered;

        /*
        *  Ground check & Coyote time
        */
        if (groundCollisions.Length > 0)
        {
            coyoteTimeCounter = coyoteTime;
            isGrounded = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            verticalVelocity = Mathf.Max(
                verticalVelocity - fallAcceleration * Time.deltaTime,
                -maxFallSpeed
            );
        }

        /*
        *  Jump handling
        */
        if (jumpTriggered && coyoteTimeCounter > 0)
        {
            Debug.Log("Jump");
            verticalVelocity = jumpSpeed;
            coyoteTimeCounter = 0;
        }

        /*
        *  Horizontal movement
        */
        if (moveValue.CompareTo(0) != 0)
        {
            var movement = moveValue > 0 ? Vector2.right : Vector2.left;
            horizontalVelocity = movement.x * moveSpeed * Time.deltaTime;
        }

        /*
        * Apply gravity
        */
        if (!isGrounded)
        {
            verticalVelocity = Mathf.Max(
                verticalVelocity - fallAcceleration * Time.deltaTime,
                -maxFallSpeed
            );
        }

        /*
        *  Apply velocities
        */
        var predictedPosition = rb.position + new Vector2(horizontalVelocity, verticalVelocity);

        transform.position = predictedPosition;

        /*
        * Check for ground hits afte applying vertical velocity
        */
        var groundHits = Physics2D.BoxCastAll(
            col.bounds.center,
            col.bounds.size,
            0,
            Vector2.down,
            verticalVelocity,
            collisionDetector.groundLayer
        );
        if (groundHits.Length > 0)
        {
            var closestHit = groundHits[0];
            foreach (var hit in groundHits)
            {
                if (hit.distance < closestHit.distance)
                {
                    closestHit = hit;
                }
            }

            verticalVelocity = 0;
            transform.position = new Vector2(
                transform.position.x,
                closestHit.point.y + col.bounds.size.y / 2
            );
        }
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
