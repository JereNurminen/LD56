using NUnit.Framework.Constraints;
using UnityEngine;

public class CollisionDetector2D : MonoBehaviour
{
    public LayerMask groundLayer;
    public float groundCheckDistance = 1f;
    public float wallCheckDistance = 1f;

    private Collider2D col;
    private RaycastHit2D[] hits;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public RaycastHit2D[] CheckForGround()
    {
        hits = Physics2D.BoxCastAll(
            col.bounds.center,
            new Vector2(col.bounds.size.x, col.bounds.size.y + groundCheckDistance),
            0,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        return hits;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            col.bounds.center,
            new Vector3(col.bounds.size.x, col.bounds.size.y + groundCheckDistance, 1)
        );
    }

    public bool IsTouchingWall(float direction)
    {
        Vector2 leftRayOrigin = new Vector2(col.bounds.min.x, col.bounds.center.y);
        Vector2 rightRayOrigin = new Vector2(col.bounds.max.x, col.bounds.center.y);

        if (direction < 0)
        {
            return Physics2D.Raycast(leftRayOrigin, Vector2.left, wallCheckDistance, groundLayer);
        }
        else if (direction > 0)
        {
            return Physics2D.Raycast(rightRayOrigin, Vector2.right, wallCheckDistance, groundLayer);
        }

        return false;
    }
}
