using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR;

public enum SheepCommand
{
    Go,
    Stop
}

public class SheepController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float memoryDuration = 5f;
    public float jumpSpeed = 10f;
    public float fallAcceleration = 4f;
    public LayerMask obstacleLayers;
    public LayerMask hazardLayers;

    private float horizontalVelocity = 0;
    private float verticalVelocity = 0;

    private int direction = 1;

    private float timeSinceCommand = 0;
    private bool isGrounded = true;
    private bool isJumping = false;
    public bool isAlive = true;
    public bool isSuccess = false;

    private CollisionDetector2D collisionDetector;
    private Collider2D col;
    private SheepCommand? currentCommand = null;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed += Random.Range(-1f, 1f);
        memoryDuration += Random.Range(-1f, 1f);

        col = GetComponent<Collider2D>();
        collisionDetector = GetComponent<CollisionDetector2D>();
        animator = GetComponent<Animator>();
    }

    private void HandleGravity()
    {
        if (collisionDetector.IsGroundedBox() && verticalVelocity.CompareTo(0) <= 0)
        {
            verticalVelocity = 0f;
            isGrounded = true;
            isJumping = false;
        }
        else
        {
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

        animator.SetFloat("vertical_speed", verticalVelocity);
    }

    public void ReceiveCommand(SheepCommand command, Vector2 commandPos)
    {
        currentCommand = command;
        timeSinceCommand = 0;
        switch (command)
        {
            case SheepCommand.Go:
                Go(commandPos);
                break;
            case SheepCommand.Stop:
                Stop();
                break;
        }
    }

    void Go(Vector2 moveAwayFrom)
    {
        direction = transform.position.x > moveAwayFrom.x ? 1 : -1;
    }

    void Stop() { }

    void DetectLedge()
    {
        var leftGroundHit = Physics2D.Raycast(
            new Vector2(col.bounds.min.x, col.bounds.min.y),
            Vector2.down,
            2f,
            collisionDetector.groundLayer
        );
        var rightGroundHit = Physics2D.Raycast(
            new Vector2(col.bounds.max.x, col.bounds.min.y),
            Vector2.down,
            2f,
            collisionDetector.groundLayer
        );

        if (
            (leftGroundHit.collider == null && rightGroundHit.collider != null)
            || (leftGroundHit.collider != null && rightGroundHit.collider == null)
        )
        {
            verticalVelocity = jumpSpeed;
            isJumping = true;
            animator.SetTrigger("jump");
        }
    }

    bool IsBlocked()
    {
        var dir = direction == 1 ? Vector2.right : Vector2.left;
        var origX = direction == 1 ? col.bounds.max.x + 1 : col.bounds.min.x - 1;
        var rayLength = 2f;

        var bottomHit = Physics2D.Raycast(
            new Vector2(origX, col.bounds.min.y + 1),
            dir,
            rayLength,
            obstacleLayers
        );
        var topHit = Physics2D.Raycast(
            new Vector2(origX, col.bounds.max.y - 1),
            dir,
            rayLength,
            obstacleLayers
        );

        var wouldHit = bottomHit.collider != null || topHit.collider != null;

        return wouldHit;
    }

    void Move()
    {
        if (!IsBlocked())
        {
            horizontalVelocity = direction * moveSpeed * Time.deltaTime;
            transform.Translate(horizontalVelocity, 0, 0);
        }
        else if (!isGrounded)
        {
            //direction *= -1;
        }
        transform.localScale = new Vector2(direction, 1);
        DetectLedge();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (hazardLayers == (hazardLayers | (1 << collision.gameObject.layer)))
        {
            OnHazardHit();
        }
    }

    public void OnHazardHit()
    {
        animator.SetTrigger("die");
        isAlive = false;
    }

    public void Kill()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            return;
        }

        HandleGravity();

        timeSinceCommand += Time.deltaTime;
        if (timeSinceCommand > memoryDuration)
        {
            currentCommand = null;
        }

        if (currentCommand == SheepCommand.Go || isJumping)
        {
            Move();
            animator.SetBool("is_running", true);
        }
        else
        {
            animator.SetBool("is_running", false);
        }

        animator.SetBool("is_grounded", isGrounded);
    }
}
