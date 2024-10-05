using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float parallaxEffectMultiplier = 0.5f; // Controls the parallax effect

    private Transform cam; // Reference to the camera transform
    private Vector3 lastCameraPosition; // Stores the camera's position from the previous frame

    void Start()
    {
        cam = Camera.main.transform; // Get the main camera's transform
        lastCameraPosition = cam.position; // Set the initial camera position
    }

    void Update()
    {
        Vector3 deltaMovement = cam.position - lastCameraPosition; // Calculate the camera movement
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, 0, 0); // Adjust the background position

        lastCameraPosition = cam.position; // Update the last camera position
    }
}
