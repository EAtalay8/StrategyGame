using UnityEngine;

[System.Serializable]
public class City
{
    public int cityID;
    public string cityName;
    public int population;
    public string religion;
    public int taxRate;
    public bool isRebelling;

    public City(int id, string name, int pop, string rel, int tax)
    {
        cityID = id;
        cityName = name;
        population = pop;
        religion = rel;
        taxRate = tax;
        isRebelling = false; // Baþlangýçta isyan yok
    }

    public void IncreaseTax(int amount)
    {
        taxRate += amount;
        if (taxRate > 50) isRebelling = true; // Vergi çok arttýðýnda isyan baþlasýn
    }
}
