using UnityEngine;

/// <summary>
/// Þehir seçildiðinde Particle Circle'ý otomatik açar/kapatýr ve UI panelini yönetir.
/// </summary>
public class CitySelector : MonoBehaviour
{
    [Header("Selection Particle Settings")]
    public GameObject selectionParticlePrefab; // Seçim çemberi için Particle Circle prefab (Inspector’dan atanacak)
    private GameObject activeParticle;

    [Header("UI Settings")]
    public CityReference cityRef; // Þehir bilgisi için referans (ID)
    private static CitySelector activeCity; // Þu anda seçili þehir

    void Start()
    {
        if (selectionParticlePrefab == null)
        {
            Debug.LogWarning("Selection Particle Prefab atanmamýþ!");
        }

        // Baþlangýçta seçim efekti gizli olmalý
        if (activeParticle != null)
        {
            activeParticle.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Þehre týklanýp týklanmadýðýný kontrol et
                CitySelector clickedCity = hit.transform.GetComponentInParent<CitySelector>();

                if (clickedCity != null)
                {
                    clickedCity.SelectCity();
                }
                else if (activeCity != null)
                {
                    activeCity.DeselectCity();
                }
            }
            else if (activeCity != null)
            {
                activeCity.DeselectCity();
            }
        }
    }

    /// <summary>
    /// Þehri seçer ve Particle Circle ile UI panelini açar.
    /// </summary>
    public void SelectCity()
    {
        // Baþka bir þehir seçiliyse önce onu kapat
        if (activeCity != null && activeCity != this)
        {
            activeCity.DeselectCity();
        }

        // Particle Circle'ý oluþtur veya aktif et
        if (selectionParticlePrefab != null)
        {
            if (activeParticle == null)
            {
                activeParticle = Instantiate(selectionParticlePrefab, transform);
                activeParticle.transform.localPosition = Vector3.zero;
            }

            activeParticle.SetActive(true);
        }

        // UI paneli ve þehir bilgisi göster
        if (cityRef != null)
        {
            CityInformationPanel.Instance.ShowCityInfo(cityRef.cityID);
        }

        UIManager.Instance.ShowPanel();
        activeCity = this;
    }

    /// <summary>
    /// Þehir seçimini kaldýrýr, Particle Circle ve UI panelini kapatýr.
    /// </summary>
    public void DeselectCity()
    {
        // Particle Circle'ý kapat
        if (activeParticle != null)
        {
            activeParticle.SetActive(false);
        }

        // UI panelini kapat
        UIManager.Instance.HidePanel();
        activeCity = null;
    }

    /// <summary>
    /// UI panelinden þehir kapatýldýðýnda çaðrýlýr.
    /// </summary>
    public void ClosePanel()
    {
        DeselectCity();
    }
}
