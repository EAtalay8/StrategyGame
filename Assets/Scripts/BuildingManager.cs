using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [Header("Settings")]
    public GameObject slotPrefab; // Kare prefabimiz
    public float slotSpacing = 2f; // Kareler arasi bosluk

    // Simdilik gecici olarak aktif olan slotlari tutalim
    private List<GameObject> activeSlots = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Sehir secilince slotlari olustur (veya havuzdan cek)
    public void ShowSlots(City city, Vector3 cityPosition, Quaternion cityRotation)
    {
        ClearSlots();

        // 3x3 Grid olusturuyoruz (Merkez haric)
        // x: -1, 0, 1
        // z: -1, 0, 1
        
        int currentIndex = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                // Merkeze (0,0) slot koymuyoruz, orada sehir var
                if (x == 0 && z == 0) continue;

                Vector3 offset = new Vector3(x * slotSpacing, 0, z * slotSpacing);
                Vector3 rotatedOffset = cityRotation * offset; // Rotasyonu uygula
                Vector3 spawnPos = cityPosition + rotatedOffset;
                
                // TODO: Prefab yerine PrimitiveCube kullaniyorum test icin.
                // Gercek projede slotPrefab instantiate edecegiz.
                GameObject slotObj;
                if (slotPrefab != null)
                {
                    slotObj = Instantiate(slotPrefab, spawnPos, cityRotation); // Slotu da dondur
                }
                else
                {
                    slotObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    slotObj.transform.position = spawnPos;
                    slotObj.transform.rotation = cityRotation; // Slotu da dondur
                    slotObj.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f); // Yassi kare
                }

                BuildingSlot slotScript = slotObj.GetComponent<BuildingSlot>();
                if (slotScript == null) slotScript = slotObj.AddComponent<BuildingSlot>();

                slotScript.slotIndex = currentIndex;
                
                // Kilit Mantigi: Ornek olarak her 1000 nufus 1 slot acar
                // (Bu mantigi daha sonra detaylandirabiliriz)
                int requiredPop = currentIndex * 1000; 
                bool isUnlocked = city.population >= requiredPop;
                
                // Doluluk kontrolu (City verisinden gelmeli)
                // Simdilik bos varsayalim
                bool isOccupied = false; 

                slotScript.SetState(isUnlocked, isOccupied);

                activeSlots.Add(slotObj);
                currentIndex++;
            }
        }
    }

    public void HideSlots()
    {
        ClearSlots();
    }

    void ClearSlots()
    {
        foreach (var slot in activeSlots)
        {
            Destroy(slot);
        }
        activeSlots.Clear();
    }
}
