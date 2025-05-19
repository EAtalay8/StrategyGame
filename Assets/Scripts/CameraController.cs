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
    private bool isRotating = false;     // Sað týk ile rotasyon kontrolü

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera bileþeni bulunamadý! Lütfen bu scripti bir Kamera objesine ekleyin.");
        }

        // Baþlangýç rotasyonunu mevcut pozisyon olarak ayarla
        currentRotationX = transform.eulerAngles.x;
        targetRotationX = currentRotationX;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    // WASD veya Yön Tuþlarý ile Hareket
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A,D
        float vertical = Input.GetAxis("Vertical");     // W,S

        Vector3 move = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    // Fare Kaydýrma ile Zoom (FOV - Field of View)
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            // Zoom'u FOV ile kontrol et (Perspective)
            float newFOV = cam.fieldOfView - scroll * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);

            Debug.Log("Zoom (FOV): " + cam.fieldOfView); // Test için zoom deðerini gör
        }
    }

    // Sað Týk ile Smooth X Ekseninde Dönüþ (Rotation)
    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1)) // Sað týk basýlý tutulunca
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
            targetRotationX -= mouseY * rotationSpeed * Time.deltaTime; // X ekseninde yukarý-aþaðý
            targetRotationX = Mathf.Clamp(targetRotationX, minRotationX, maxRotationX);
        }

        // Smooth Lerp ile yumuþak geçiþ
        currentRotationX = Mathf.Lerp(currentRotationX, targetRotationX, rotationSmoothing * Time.deltaTime);
        transform.rotation = Quaternion.Euler(currentRotationX, transform.eulerAngles.y, 0);
    }
}
