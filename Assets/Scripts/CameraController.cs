using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 offset;
    public float followSpeed = 5f;
    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition =
            playerController.transform.position + new Vector3(offset.x, offset.y, -10);
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}
