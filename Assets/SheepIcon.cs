using UnityEngine;
using UnityEngine.UI;

public enum SheepIconState
{
    Default,
    Dead,
    Success
}

public class SheepIcon : MonoBehaviour
{
    public Image defaultSheepIcon;
    public Image deadSheepIcon;
    public Image successSheepIcon;
    public int id = 0;

    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSheepIcon.sprite;
    }

    public void SetState(SheepIconState state)
    {
        switch (state)
        {
            case SheepIconState.Default:
                spriteRenderer.sprite = defaultSheepIcon.sprite;
                break;
            case SheepIconState.Dead:
                spriteRenderer.sprite = deadSheepIcon.sprite;
                break;
            case SheepIconState.Success:
                spriteRenderer.sprite = successSheepIcon.sprite;
                break;
        }
    }

    // Update is called once per frame
    void Update() { }
}
