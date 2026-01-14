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
    
    [Header("Build Mode Settings")]
    public float buildModeDistance = 40f; // Insaat modunda kameranin sehirden ne kadar uzakta olacagi (Sabit)

    private Camera cam;
    private bool isRotating = false;
    
    // Smooth gecis ve limit takibi icin rotation degiskenleri
    private float currentRotationX = 0f;
    private float targetRotationX = 0f;

    // Kamera durumunu kaydetmek icin degiskenler
    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private float savedFOV;
    private float savedRotationXValue;

    private bool isLocked = false; // Kamera kontrolu kilitli mi?

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Component 'Camera' not found! Please attach this script to a Camera object.");
        }

        // Baslangic rotasyonunu al
        currentRotationX = transform.eulerAngles.x;
        targetRotationX = currentRotationX;
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
        if (isLocked) return;

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
        if (isLocked) return;

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
        if (isLocked) return;

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

    public void FocusOnCity(Vector3 cityPosition)
    {
        // Mevcut durumu kaydet (Geri donus icin)
        savedPosition = transform.position;
        savedRotation = transform.rotation;
        savedFOV = cam.fieldOfView;
        savedRotationXValue = currentRotationX;

        // Hedefler:
        // 1. Zoom: En yakin (minZoom)
        // 2. Rotasyon: En dik (maxRotationX)
        // 3. Pozisyon: Sehri ortalayacak sekilde

        float targetFOV = minZoom;
        float targetRotX = maxRotationX;

        // Hedef pozisyonu hesapla:
        // Kamerayi dik aciya getirdigimizde (maxRotationX), sehri gorebilmesi icin
        // yukarida ve biraz geride (Z ekseninde) olmasi lazim.
        // Basitce: Kamerayi sehrin X,Z koordinatina tasiyalim, Y'sini koruyalim (veya ayarlayalim)
        // Ve rotasyona gore geri cekelim.
        
        // Mevcut yuksekligi koruyalim simdilik
        float currentHeight = transform.position.y;
        
        // Yeni rotasyon Quaternion'u
        Quaternion targetRotation = Quaternion.Euler(targetRotX, transform.eulerAngles.y, 0);
        
        // Kameranin bakacagi yonun tersine (Back) dogru yukseklik kadar gidelim
        // Bu kisim biraz deneme yanilma isteyebilir ama basic mantik sudur:
        // Pos = CityPos - Forward * Distance
        
        // Raycast ile mesafeyi bulabiliriz ama simdilik basit bir offset yapalim
        // Kamerayi sehrin tam ustune (X, Z ayni) getirelim, sonra aciya gore geriye kaydiralim
        
        Vector3 finalPos = cityPosition;
        finalPos.y = currentHeight; 

        // Hafif duzeltme: Eger tam 90 derece degilse (ki 85 yapiyoruz), biraz Z offset lazim
        // Ama Orbit mantigimiz zaten Raycast kullaniyor, o yuzden sadece X ve Z'yi ortalamak yetebilir.
        // Biz direkt Coroutine icinde Lerp yapalim.
        
        StartCoroutine(MoveToTargetCoroutine(cityPosition, targetFOV, targetRotX));
    }

    public void SetLock(bool state)
    {
        isLocked = state;
    }

    System.Collections.IEnumerator MoveToTargetCoroutine(Vector3 targetCenter, float targetFOV, float targetRotX)
    {
        float duration = 1.0f; 
        float time = 0;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float startFOV = cam.fieldOfView;
        float startRotXValue = currentRotationX;

        // Harekete baslarken kilitle
        SetLock(true);

        // Hedef Rotasyon
        Quaternion endRot = Quaternion.Euler(targetRotX, startRot.eulerAngles.y, 0);

        // Hedef Pozisyon Hesabi (Biraz trick lazim)
        // Kameranin su anki yerden yuksekligini bulalim
        float height = transform.position.y;
        
        // Hedefte kameranin konumu ne olmali?
        // Sehri tam ortalamak istiyoruz.
        // Eger kamera yere paralel (90 derece) bakiyorsa tam ustunde olmali (X,Z ayni).
        // Eger egik bakiyorsa, biraz geride olmali.
        // Basit Trigonometri: Z_offset = Height / Tan(Angle)
        // Ama 'Forward' vektoru ile geri gitmek daha kolay.
        
        Vector3 forwardDir = endRot * Vector3.forward;
        // Ray'in yere degmesi gerektigi nokta targetCenter.
        // CameraPos = TargetCenter - (Forward * Distance)
        
        // ESKI YONTEM: Su anki mesafeyi aliyordu (tekrar basinca yaklasiyordu)
        // float distance = Vector3.Distance(transform.position, GetCenterGroundPoint());
        // if (distance < 5f) distance = 20f;
        
        // YENI YONTEM: Sabit bir mesafe kullaniyoruz
        float distance = buildModeDistance;

        Vector3 endPos = targetCenter - (forwardDir * distance);
        // Yere cakilmamak icin Y kontrolu
        if (endPos.y < 5f) endPos.y = 10f; 


        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = t * t * (3f - 2f * t); // Smoothstep

            cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            
            // Rotasyon
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            currentRotationX = Mathf.Lerp(startRotXValue, targetRotX, t);

            // Pozisyon
            // transform.position = Vector3.Lerp(startPos, endPos, t);
            // Kayan bir etki icin Spherip Lerp veya direkt Lerp
            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        // Garanti bitis degerleri
        cam.fieldOfView = targetFOV;
        transform.rotation = endRot;
        currentRotationX = targetRotX;
        targetRotationX = targetRotX; 
    }

    public void RestoreCameraState()
    {
        // Kaydedilen duruma geri don
        // İsteğe bağlı: Burada da Smooth gecis yapilabilir ama simdilik direkt donelim veya ayni Coroutine'i kullanalim
        
        // Coroutine ile donelim ki smooth olsun
        StopAllCoroutines(); // Onceki hareketi durdur
        StartCoroutine(RestoreStateCoroutine());
    }

    System.Collections.IEnumerator RestoreStateCoroutine()
    {
        float duration = 1.0f;
        float time = 0;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float startFOV = cam.fieldOfView;
        float startRotXValue = currentRotationX;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = t * t * (3f - 2f * t); // Smoothstep

            cam.fieldOfView = Mathf.Lerp(startFOV, savedFOV, t);
            transform.rotation = Quaternion.Lerp(startRot, savedRotation, t);
            transform.position = Vector3.Lerp(startPos, savedPosition, t);
            currentRotationX = Mathf.Lerp(startRotXValue, savedRotationXValue, t);

            yield return null;
        }

        // Tam degerlere oturt
        cam.fieldOfView = savedFOV;
        transform.rotation = savedRotation;
        transform.position = savedPosition;
        currentRotationX = savedRotationXValue;
        targetRotationX = savedRotationXValue;

        // Hareketi bitirince kilidi ac
        SetLock(false);
    }
}
