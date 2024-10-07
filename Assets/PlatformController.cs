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
        animator.SetBool("is_open", startOpen ? false : true);
        col.enabled = startOpen ? true : false;
    }

    public void Close()
    {
        animator.SetBool("is_open", startOpen ? true : false);
    }

    public void OnCloseAnimationComplete()
    {
        col.enabled = startOpen ? false : true;
    }

    // Update is called once per frame
    void Update() { }
}
