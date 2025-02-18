using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 100.0f;
    public float zoomSpeed = 5.0f;
    public float minZoom = 1.0f;
    public float maxZoom = 10.0f;

    private Camera cam;
    private float initialZoom;
    private Quaternion initialRotation;

    void Start()
    {
        cam = GetComponent<Camera>(); // Ensure the camera component is attached
        initialZoom = cam.orthographicSize; // Save the initial zoom level
        initialRotation = transform.rotation; // Save the initial rotation
    }

    void Update()
    {
        // Movement
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(moveX, moveY, 0);

        // Rotate around the Y-axis using Q and E keys
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime, Space.World);
        }

        // Zooming with the mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
        }

        // Reset rotation and zoom with the 'R' key
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.rotation = initialRotation;
            cam.orthographicSize = initialZoom;
        }
    }

    // Method to change zoom level within the min and max range
    public void ChangeZoomLevel(int zoomLevel)
    {
        // Ensure the zoom level is within the given min and max values
        if (zoomLevel >= minZoom && zoomLevel <= maxZoom)
        {
            cam.orthographicSize = zoomLevel;
        }
        else
        {
            // Set to the middle value if the given zoom level is out of range
            cam.orthographicSize = (minZoom + maxZoom) / 2;
        }
    }
}
