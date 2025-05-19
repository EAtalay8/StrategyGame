using UnityEngine;

/// <summary>
/// �ehir se�ildi�inde Particle Circle'� otomatik a�ar/kapat�r ve UI panelini y�netir.
/// </summary>
public class CitySelector : MonoBehaviour
{
    [Header("Selection Particle Settings")]
    public GameObject selectionParticlePrefab; // Se�im �emberi i�in Particle Circle prefab (Inspector�dan atanacak)
    private GameObject activeParticle;

    [Header("UI Settings")]
    public CityReference cityRef; // �ehir bilgisi i�in referans (ID)
    private static CitySelector activeCity; // �u anda se�ili �ehir

    void Start()
    {
        if (selectionParticlePrefab == null)
        {
            Debug.LogWarning("Selection Particle Prefab atanmam��!");
        }

        // Ba�lang��ta se�im efekti gizli olmal�
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
                // �ehre t�klan�p t�klanmad���n� kontrol et
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
    /// �ehri se�er ve Particle Circle ile UI panelini a�ar.
    /// </summary>
    public void SelectCity()
    {
        // Ba�ka bir �ehir se�iliyse �nce onu kapat
        if (activeCity != null && activeCity != this)
        {
            activeCity.DeselectCity();
        }

        // Particle Circle'� olu�tur veya aktif et
        if (selectionParticlePrefab != null)
        {
            if (activeParticle == null)
            {
                activeParticle = Instantiate(selectionParticlePrefab, transform);
                activeParticle.transform.localPosition = Vector3.zero;
            }

            activeParticle.SetActive(true);
        }

        // UI paneli ve �ehir bilgisi g�ster
        if (cityRef != null)
        {
            CityInformationPanel.Instance.ShowCityInfo(cityRef.cityID);
        }

        UIManager.Instance.ShowPanel();
        activeCity = this;
    }

    /// <summary>
    /// �ehir se�imini kald�r�r, Particle Circle ve UI panelini kapat�r.
    /// </summary>
    public void DeselectCity()
    {
        // Particle Circle'� kapat
        if (activeParticle != null)
        {
            activeParticle.SetActive(false);
        }

        // UI panelini kapat
        UIManager.Instance.HidePanel();
        activeCity = null;
    }

    /// <summary>
    /// UI panelinden �ehir kapat�ld���nda �a�r�l�r.
    /// </summary>
    public void ClosePanel()
    {
        DeselectCity();
    }
}
