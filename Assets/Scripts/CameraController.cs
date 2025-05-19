using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 50f;

    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A,D
        float vertical = Input.GetAxis("Vertical"); // W,S

        Vector3 move = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(mousePos);

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPoint = hit.point; // Çarptýðý noktaya zoom yap
            }
            else
            {
                targetPoint = ray.origin + ray.direction * 10f; // Boþluða bakýyorsa 10 birim uzaða bak
            }

            Vector3 direction = targetPoint - cam.transform.position;
            float newZoom = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
            cam.orthographicSize = newZoom;

            // Kamera hedef noktaya yaklaþsýn
            transform.position += direction.normalized * scroll * zoomSpeed * 0.5f;
        }
    }
}
