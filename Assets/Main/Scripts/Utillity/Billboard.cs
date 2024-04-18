using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera mainCamera;

    // Hovering parameters
    public float hoverAmplitude = 0.5f; // The height range of the hover effect
    public float hoverFrequency = 1f; // How fast it hovers up and down

    private float originalY; // Original y-position of the GameObject

    // Start is called before the first frame update
    void Start()
    {
        // Find and store a reference to the main camera in the scene
        mainCamera = Camera.main;

        // Store the original y-position
        originalY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the billboard to face towards the camera
        transform.LookAt(mainCamera.transform);

        // Hover effect
        Hover();
    }

    void Hover()
    {
        // Calculate the new Y position using sine to create smooth oscillation
        float newY = originalY + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

        // Apply the new Y position while keeping X and Z positions the same
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
