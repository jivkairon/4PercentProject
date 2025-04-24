using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float moveSpeed = 5f;
    public float targetZoom = 5f; 

    private Vector3 targetPosition;
    private float originalZoom;
    private bool isZooming = false;

    void Start()
    {
        originalZoom = Camera.main.orthographicSize;
    }

    void Update()
    {
        if (isZooming)
        {
            // Преместване на камерата
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Зуум на камерата
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);

            // Спиране на зуум
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f &&
                Mathf.Abs(Camera.main.orthographicSize - targetZoom) < 0.1f)
            {
                isZooming = false;
            }
        }
    }

    public void ZoomToRegion(Vector3 regionPosition)
    {
        targetPosition = new Vector3(regionPosition.x, regionPosition.y, transform.position.z);
        isZooming = true;
    }

    public void ResetCamera()
    {
        targetPosition = new Vector3(0, 0, transform.position.z); // Връщане към начална позиция
        targetZoom = originalZoom;
        isZooming = true;
    }
}