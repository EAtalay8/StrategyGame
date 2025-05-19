using UnityEngine;
using UnityEngine.UI;

public class CityInformationPanel : MonoBehaviour
{
    public static CityInformationPanel Instance;

    public Text cityNameText;
    public Text populationText;
    public Text religionText;
    public Text taxRateText;
    public Text rebellionText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /*public void ShowCityInfo(string cityName)
    {
        City city = GameManager.Instance.GetCityByName(cityName);
        if (city == null) return;

        cityNameText.text = "City: " + city.cityName;
        populationText.text = "Population: " + city.population;
        religionText.text = "Religion: " + city.religion;
        taxRateText.text = "Tax Rate: %" + city.taxRate;
        rebellionText.text = city.isRebelling ? "Rebellion Status: ? �syan Var!" : "�syan Durumu: Huzur Var";

        gameObject.SetActive(true);
    }*/

    public void ShowCityInfo(int cityID)
    {
        City city = GameManager.Instance.GetCityByID(cityID);
        if (city == null) return;

        cityNameText.text = "City: " + city.cityName;
        populationText.text = "Population: " + city.population;
        religionText.text = "Religion: " + city.religion;
        taxRateText.text = "Tax Rate: %" + city.taxRate;
        rebellionText.text = city.isRebelling ? "Rebellion Status: ? �syan Var!" : "�syan Durumu: Huzur Var";

        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}

/*// Singleton 
public static CityInformationPanel Instance { get; private set; }

public Text cityNameText;  // �ehir ismini yazd�raca��m�z Text UI eleman�

private string currentCityName;

private void Awake()
{
    // E�er ba�ka bir CityInformationPanel �rne�i varsa, bu objeyi yok et
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
    }
    else
    {
        Instance = this;  // Singleton �rne�ini ayarla
        DontDestroyOnLoad(gameObject);  // Bu objenin sahneler aras� kaybolmamas�n� sa�la
    }
}
// Bu fonksiyon �ehre t�kland���nda ismi g�ncelleyecek
public void SetCityName(string cityName)
{
    currentCityName = cityName;
    cityNameText.text = currentCityName;  // �ehir ismini Text'e at�yoruz
}

public void ShowCityInfo(string cityName)
{
    SetCityName(cityName);  // �ehir ad�n� g�ncelle
}*/ 