using UnityEngine;

public class ObjectOutline : MonoBehaviour
{
    public Color outlineColor = Color.yellow; // Çizgi rengi
    public float outlineWidth = 0.05f; // Çizgi kalýnlýðý

    private GameObject outlineObj;
    private static ObjectOutline activeOutline; // Þu an seçili obje
    private bool isOutlined = false;

    void Start()
    {
        CreateOutlineObject();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol týk
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    SetActiveOutline();
                }
                else if (activeOutline != null && UIManager.Instance.IsPanelOpen() == false)
                {
                    ClearActiveOutline();
                }
            }
            else if (UIManager.Instance.IsPanelOpen() == false)
            {
                ClearActiveOutline();
            }

            // Þehre týklama iþlemi yapýldýðýnda:
            //CityInformationPanel.Instance.SetCityName(gameObject.name);  // Burada "Þehir Ýsmi", týklanan þehrin adý olacak
            //CityInformationPanel.Instance.ShowCityInfo(GameManager.Instance.GetCityByID(cityRef.cityID));
        }
    }

    void CreateOutlineObject()
    {
        outlineObj = Instantiate(gameObject, transform.position, transform.rotation);
        outlineObj.transform.localScale = transform.localScale * (1f + outlineWidth);

        DestroyImmediate(outlineObj.GetComponent<ObjectOutline>());
        if (outlineObj.GetComponent<Collider>() != null)
            DestroyImmediate(outlineObj.GetComponent<Collider>());

        Renderer rend = outlineObj.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Unlit/Color"));
        rend.material.color = outlineColor;

        outlineObj.SetActive(false);
    }

    void SetActiveOutline()
    {
        if (activeOutline != null && activeOutline != this)
        {
            activeOutline.ToggleOutline(false);
            UIManager.Instance.HidePanel();
        }

        isOutlined = true;
        outlineObj.SetActive(true);

        // **Týklanan þehrin adýný panelde göster**
        //CityInformationPanel.Instance.ShowCityInfo(gameObject.name);

        // ?? CityReference ile ID al, sonra GameManager'dan þehri bul
        CityReference cityRef = GetComponent<CityReference>();
        if (cityRef != null)
        {
            CityInformationPanel.Instance.ShowCityInfo(cityRef.cityID);
        }

        UIManager.Instance.ShowPanel();
        activeOutline = this;
    }

    void ClearActiveOutline()
    {
        if (activeOutline == this)
        {
            ToggleOutline(false);
            UIManager.Instance.HidePanel(); // UIManager üzerinden paneli kapat
            activeOutline = null;
        }
    }

    void ToggleOutline(bool state)
    {
        isOutlined = state;
        outlineObj.SetActive(state);
    }

    public void ClosePanel()
    {
        ClearActiveOutline();
    }
}
