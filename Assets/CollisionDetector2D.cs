using NUnit.Framework.Constraints;
using UnityEngine;

public class CollisionDetector2D : MonoBehaviour
{
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public float wallCheckDistance = 0.1f;

    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public bool IsGroundedBox()
    {
        Vector2 boxCenter = new Vector2(
            col.bounds.center.x,
            col.bounds.min.y - groundCheckDistance / 2
        );
        Vector2 boxSize = new Vector2(col.bounds.size.x, groundCheckDistance);
        return Physics2D.OverlapBox(boxCenter, boxSize, 0, groundLayer) != null;
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
