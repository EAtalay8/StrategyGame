using UnityEngine;
using UnityEngine.EventSystems;

public class CitySelector : MonoBehaviour
{
    public static CitySelector activeCity; // Seçili şehri tutmak için static referans
    public GameObject particlePrefab;       // Şehir seçildiğinde çıkacak Particle System Prefab
    private GameObject activeParticle;      // Sahnedeki aktif particle objesi
    
    private bool isBuildModeActive = false; // Insaat modu aktif mi?

    //public GameObject outlineObj;         // Shader Graph outline objesi (Artık ObjectOutline.cs kullanılıyor)

    void Start()
    {
        // Particle prefabı oluştur ama başta kapalı olsun
        if (particlePrefab != null)
        {
            activeParticle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            activeParticle.transform.SetParent(transform); // Şehrin çocuğu yap
            activeParticle.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        // Eğer UI üzerindeysek (tıklama UI'ye geldiyse) şehir seçmeyi engelle
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        SelectCity();
    }

    public void SelectCity()
    {
        // Eğer zaten başka bir şehir seçiliyse, önce onu kapat
        if (activeCity != null && activeCity != this)
        {
            activeCity.DeselectCity();
        }

        // Particle Circle'ı aç
        if (activeParticle != null)
        {
            // Şehrin tam ortasını bulmak için Renderer kullanabiliriz (opsiyonel)
            // MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            // if (renderers.Length > 0) ... center hesapla ...
            // Şimdilik direkt transform.position kullanıyoruz.
            
            // Eğer objenin merkezi aşağıdaysa, particle'ı biraz yukarı kaldırabiliriz
            // activeParticle.transform.position = transform.position + Vector3.up * 0.1f;

            // Eğer AutoCityCollider kullanıyorsak, collider center'ı daha doğru olabilir
            // Merkez noktasini hesaplayalim
            Vector3 finalCenter = transform.position;

            // 1. Varsa Collider merkezini al (En garantisi bu)
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Debug.Log($"CitySelector: Found Collider on {name}. Bounds Center: {col.bounds.center}");
                finalCenter = col.bounds.center;
            }
            // 2. Yoksa Renderer'larin ortalamasini al
            else
            {
                Debug.LogWarning($"CitySelector: No Collider found on {name} (Root). Checking Renderers...");
                MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
                if (renderers.Length > 0)
                {
                    Bounds combinedBounds = renderers[0].bounds;
                    for (int i = 1; i < renderers.Length; i++)
                    {
                        combinedBounds.Encapsulate(renderers[i].bounds);
                    }
                    finalCenter = combinedBounds.center;
                }
            }

            // Yuksekligi (Y) bozma, yer seviyesinde kalsin
            finalCenter.y = transform.position.y;
            
            // Particle'i oraya tasi
            activeParticle.transform.position = finalCenter;
            activeParticle.SetActive(true);
        }

        // UI paneli ve şehir bilgisi göster
        // ?? CityReference ile ID al, sonra GameManager'dan şehri bul
        CityReference cityRef = GetComponent<CityReference>();
        if (cityRef != null)
        {
            CityInformationPanel.Instance.ShowCityInfo(cityRef.cityID);

            // Building Slots Goster
            // City city = GameManager.Instance.GetCityByID(cityRef.cityID);
            // if (city != null && BuildingManager.Instance != null)
            // {
            //     // ARTIK OTOMATIK GOSTERMIYORUZ - BUTONA TIKLAYINCA GELECEK
            // }
        }

        UIManager.Instance.ShowPanel();
        activeCity = this;
    }

    /// <summary>
    /// Şehir seçimini kaldırır, Particle Circle ve UI panelini kapatır.
    /// </summary>
    public void DeselectCity()
    {
        // Particle Circle'ı kapat
        if (activeParticle != null)
        {
            activeParticle.SetActive(false);
        }

        // Secim kalkinca build modunu da kapat ve temizle
        if (isBuildModeActive)
        {
             BuildingManager.Instance.HideSlots();

             // Kamerayi eski haline dondur
             Camera cam = Camera.main;
             if (cam != null)
             {
                 CameraController camCtrl = cam.GetComponent<CameraController>();
                 if (camCtrl != null)
                 {
                     camCtrl.RestoreCameraState();
                 }
             }

             isBuildModeActive = false;
        }

        // UI panelini kapat
        UIManager.Instance.HidePanel();
        
        // Building Slots Kapat
        if (BuildingManager.Instance != null)
        {
            BuildingManager.Instance.HideSlots();
        }

        activeCity = null;
    }

    /// <summary>
    /// UI panelinden şehir kapatıldığında çağrılır.
    /// </summary>
    public void ClosePanel()
    {
        DeselectCity();
    }

    // UI Butonundan cagrilacak fonksiyon
    public void ToggleBuildMode()
    {
        if (activeCity != this) return;

        CityReference cityRef = GetComponent<CityReference>();
        if (cityRef == null) return;
        
        City city = GameManager.Instance.GetCityByID(cityRef.cityID);
        if (city == null) return;

        Camera cam = Camera.main;
        CameraController camCtrl = (cam != null) ? cam.GetComponent<CameraController>() : null;

        if (!isBuildModeActive)
        {
            // AKTIF ET
            // 1. Slotlari Goster
            Vector3 centerPos = activeParticle != null ? activeParticle.transform.position : transform.position;
            BuildingManager.Instance.ShowSlots(city, centerPos, transform.rotation);

            // 2. Kamerayi Odakla
            if (camCtrl != null)
            {
                camCtrl.FocusOnCity(centerPos);
            }
            
            isBuildModeActive = true;
        }
        else
        {
            // KAPAT
            // 1. Slotlari Gizle
            BuildingManager.Instance.HideSlots();

            // 2. Kamerayi Geri Getir
            if (camCtrl != null)
            {
                camCtrl.RestoreCameraState();
            }
            
            isBuildModeActive = false;
        }
    }
}
