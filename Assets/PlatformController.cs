using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public bool startOpen = false;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("is_open", startOpen);
        col = GetComponent<Collider2D>();
    }

    public void Open()
    {
        animator.SetBool("is_open", true);
        col.enabled = false;
    }

    public void Close()
    {
        animator.SetBool("is_open", false);
    }

    public void OnCloseAnimationComplete()
    {
        col.enabled = true;
    }

    // Update is called once per frame
    void Update() { }
}
