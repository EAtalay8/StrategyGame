using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Header("Zoom Settings (Perspective)")]
    public float zoomSpeed = 10f;
    public float minZoom = 20f; // Minimum Field of View (FOV)
    public float maxZoom = 60f; // Maximum Field of View (FOV)

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public float rotationSmoothing = 5f;
    public float minRotationX = -45f;
    public float maxRotationX = 45f;

    private Camera cam;
    private float currentRotationX = 0f; // Mevcut X ekseni rotasyonu
    private float targetRotationX = 0f;  // Hedef X ekseni rotasyonu
    private bool isRotating = false;     // Sa� t�k ile rotasyon kontrol�

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera bile�eni bulunamad�! L�tfen bu scripti bir Kamera objesine ekleyin.");
        }

        // Ba�lang�� rotasyonunu mevcut pozisyon olarak ayarla
        currentRotationX = transform.eulerAngles.x;
        targetRotationX = currentRotationX;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    // WASD veya Y�n Tu�lar� ile Hareket
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A,D
        float vertical = Input.GetAxis("Vertical");     // W,S

        Vector3 move = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    // Fare Kayd�rma ile Zoom (FOV - Field of View)
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) return;

        Ray rayBefore = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 preZoomPoint;

        // 1. Zoom �ncesi mouse'un bakt��� nokta
        if (Physics.Raycast(rayBefore, out RaycastHit preHit))
            preZoomPoint = preHit.point;
        else
            preZoomPoint = rayBefore.origin + rayBefore.direction * 100f; // Bo�lu�a bak�yorsa ileri bir nokta

        // 2. FOV'u de�i�tir
        float currentFOV = cam.fieldOfView;
        float newFOV = Mathf.Clamp(currentFOV - scroll * zoomSpeed, minZoom, maxZoom);
        cam.fieldOfView = newFOV;

        // 3. Zoom sonras� mouse'un bakt��� nokta
        Ray rayAfter = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 postZoomPoint;

        if (Physics.Raycast(rayAfter, out RaycastHit postHit))
            postZoomPoint = postHit.point;
        else
            postZoomPoint = rayAfter.origin + rayAfter.direction * 100f;

        // 4. Kameray� fark kadar telafi et
        Vector3 offset = preZoomPoint - postZoomPoint;
        cam.transform.position += offset;
    }

    // Sa� T�k ile Smooth X Ekseninde D�n�� (Rotation)
    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1)) // Sa� t�k bas�l� tutulunca
        {
            isRotating = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            float mouseY = Input.GetAxis("Mouse Y");
            targetRotationX -= mouseY * rotationSpeed * Time.deltaTime; // X ekseninde yukar�-a�a��
            targetRotationX = Mathf.Clamp(targetRotationX, minRotationX, maxRotationX);
        }

        // Smooth Lerp ile yumu�ak ge�i�
        currentRotationX = Mathf.Lerp(currentRotationX, targetRotationX, rotationSmoothing * Time.deltaTime);
        transform.rotation = Quaternion.Euler(currentRotationX, transform.eulerAngles.y, 0);
    }
}
