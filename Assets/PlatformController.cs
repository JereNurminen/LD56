using System.Data.Common;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public bool startOpen = false;
    public LayerMask launchLayers;
    public Vector2 launchBoxOffset;
    public Vector2 launchBoxSize;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + launchBoxOffset, launchBoxSize);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("is_open", startOpen);
        col = GetComponent<Collider2D>();
    }

    public void Open()
    {
        animator.SetBool("is_open", startOpen ? false : true);
        col.enabled = startOpen ? true : false;
    }

    public void Close()
    {
        animator.SetBool("is_open", startOpen ? true : false);

        var hits = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + launchBoxOffset,
            launchBoxSize,
            0,
            launchLayers
        );

        foreach (Collider2D hit in hits)
        {
            var other = hit.gameObject;
            if (other.GetComponent<PlayerController>() != null)
            {
                other.GetComponent<PlayerController>().Jump();
            }
            else if (other.GetComponent<SheepController>() != null)
            {
                other.GetComponent<SheepController>().Jump(SheepJumpStrength.ExtraLong);
            }
        }
    }

    public void OnCloseAnimationComplete()
    {
        col.enabled = startOpen ? false : true;
    }

    // Update is called once per frame
    void Update() { }
}
