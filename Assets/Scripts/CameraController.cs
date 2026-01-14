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
    public float minRotationX = 10f; // Yere carpmasin diye min aci
    public float maxRotationX = 85f; // Tam tepeye dikilmeyelim

    private Camera cam;
    private bool isRotating = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Component 'Camera' not found! Please attach this script to a Camera object.");
        }
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    // WASD veya Yön Tuşları ile Hareket
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A,D
        float vertical = Input.GetAxis("Vertical");     // W,S

        // Kameranin baktigi yone gore hareket etmek icin transform.forward kullaniyoruz ama Y eksenini sifirliyoruz
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 move = (right * horizontal + forward * vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    // Fare Kaydırma ile Zoom (FOV - Field of View)
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) return;

        Vector3 preZoomPoint = GetMouseGroundPoint();

        // FOV'u değiştir
        float currentFOV = cam.fieldOfView;
        float newFOV = Mathf.Clamp(currentFOV - scroll * zoomSpeed, minZoom, maxZoom);
        cam.fieldOfView = newFOV;

        Vector3 postZoomPoint = GetMouseGroundPoint();

        // Kamerayı fark kadar telafi et (Zoom to cursor)
        Vector3 offset = preZoomPoint - postZoomPoint;
        cam.transform.position += offset;
    }

    // Mouse imlecinin altindaki yer noktasi
    Vector3 GetMouseGroundPoint()
    {
        return GetGroundPoint(Input.mousePosition);
    }

    // Ekranin tam ortasindaki yer noktasi (Pivot)
    Vector3 GetCenterGroundPoint()
    {
        return GetGroundPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
    }

    // Parametrik yer bulucu (Raycast veya Plane)
    Vector3 GetGroundPoint(Vector3 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            return ray.origin + ray.direction * 100f;
        }
    }

    // Mouse Orta Tuş ile Pivot Etrafında Dönüş (Orbit)
    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(2)) 
        {
            isRotating = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            float mouseY = Input.GetAxis("Mouse Y"); // Yukari asagi mouse hareketi
            float rotationAmount = -mouseY * rotationSpeed * Time.deltaTime;

            // Su anki aciyi kontrol et
            float currentX = transform.eulerAngles.x;
            
            // Hedef aci limitleri asiyor mu?
            // Eger 90'i gecerse ters doner, 0in altina duserse yere girer.
            // Basit bir Euler tahmini yapiyoruz:
            float nextX = currentX + rotationAmount;
            
            // Euler acilari 0-360 arasi oldugu icin negatif degerleri duzeltelim
            if (nextX > 180) nextX -= 360; 

            // Limit kontrolu
            if (nextX >= minRotationX && nextX <= maxRotationX)
            {
                // ISLEM: Pivot noktasini bul ve etrafinda cevir
                Vector3 pivotPoint = GetCenterGroundPoint();
                transform.RotateAround(pivotPoint, transform.right, rotationAmount);
            }
        }
    }
}
