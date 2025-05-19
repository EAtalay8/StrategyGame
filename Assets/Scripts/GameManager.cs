using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<City> cities = new List<City>();

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
        InitializeCities();
    }

    void InitializeCities()
    {
        cities.Add(new City(34, "Istanbul", 1500000, "Islam", 17));
        cities.Add(new City(10, "Rome", 900000, "Catholic", 15));
        cities.Add(new City(26, "Athens", 500000, "Paganism", 12));
        cities.Add(new City(16, "Bursa", 500000, "Islam", 11));
    }

    public City GetCityByName(string name)
    {
        return cities.Find(city => city.cityName == name);
    }

    public City GetCityByID(int id)
    {
        return cities.Find(city => city.cityID == id);
    }
}

