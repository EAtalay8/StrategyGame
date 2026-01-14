using UnityEngine;
using UnityEngine.UI;

public class CityInformationPanel : MonoBehaviour
{
    public static CityInformationPanel Instance;
    public GameObject panel;
    
    [Header("UI Text Components")]
    public Text cityNameText;
    public Text populationText;
    public Text religionText;
    public Text taxRateText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    public void ShowCityInfo(int cityID)
    {
        City city = GameManager.Instance.GetCityByID(cityID);
        if (city != null)
        {
            if (cityNameText != null) cityNameText.text = city.cityName;
            if (populationText != null) populationText.text = "Population: " + city.population.ToString();
            if (religionText != null) religionText.text = "Religion: " + city.religion;
            if (taxRateText != null) taxRateText.text = "Tax: %" + city.taxRate.ToString();
        }
    }
}
