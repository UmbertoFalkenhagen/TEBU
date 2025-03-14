using UnityEngine;

public class CameraManager : MonoBehaviour
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
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("No main camera found in the scene.");
            enabled = false;
            return;
        }

        initialZoom = cam.orthographicSize;
        initialRotation = cam.transform.rotation;
    }

    void Update()
    {
        if (cam == null) return;

        // Movement
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        cam.transform.Translate(moveX, moveY, 0);

        // Rotate around the Y-axis using Q and E keys
        if (Input.GetKey(KeyCode.Q))
        {
            cam.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.E))
        {
            cam.transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime, Space.World);
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
            cam.transform.rotation = initialRotation;
            cam.orthographicSize = initialZoom;
        }
    }

    public void ChangeZoomLevel(int zoomLevel)
    {
        if (cam == null) return;
        cam.orthographicSize = Mathf.Clamp(zoomLevel, minZoom, maxZoom);
    }
}