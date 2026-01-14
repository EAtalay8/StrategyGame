using UnityEngine;

public class BuildingSlot : MonoBehaviour
{
    public int slotIndex;
    public bool isUnlocked = false;
    public bool isOccupied = false;
    
    // Rengi degistirmek icin Material referansi
    private Renderer rend;
    private Color originalColor;
    
    [Header("Visual Settings")]
    public GameObject plusIcon; // Arti isareti objesi
    // Daha "Hafif/Hayalet" renkler
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.0f); // Kilitli: Siyah ve Gorunmez (veya cok az)
    public Color openColor = new Color(1f, 1f, 1f, 0.2f);         // Acik: HAFIF BEYAZ (Cam gibi)
    public Color occupiedColor = new Color(1f, 1f, 1f, 0.0f);     // Dolu: Gorunmez (Bina olacak cunku)
    public Color hoverColor = new Color(0.2f, 1f, 0.6f, 0.5f);    // Hover: Tatli bir Neon Yesil/Mavi Parlama

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) originalColor = rend.material.color;
    }

    void OnMouseEnter()
    {
        if (isUnlocked && !isOccupied)
        {
            // Mouse uzerine gelince parlasin
            if (rend != null) rend.material.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        // Eski rengine don
        if (rend != null) rend.material.color = originalColor;
    }

    void OnMouseDown()
    {
        if (!isUnlocked)
        {
            Debug.Log($"Slot {slotIndex} is LOCKED. Population requirement not met.");
            return;
        }

        if (isOccupied)
        {
            Debug.Log($"Slot {slotIndex} is already occupied.");
            // Belki bina bilgilerini goster?
            return;
        }

        Debug.Log($"Slot {slotIndex} clicked! Opening Build Menu...");
        // TODO: UI Manager'a haber ver ve insaat panelini ac
    }

    public void SetState(bool unlocked, bool occupied)
    {
        isUnlocked = unlocked;
        isOccupied = occupied;

        // Gorsel geri bildirim
        if (rend != null)
        {
            if (!isUnlocked) rend.material.color = lockedColor;
            else if (isOccupied) rend.material.color = occupiedColor;
            else rend.material.color = openColor;
            
            originalColor = rend.material.color; // Orijinal rengi guncelle
        }

        // Arti isaretini yonet
        if (plusIcon != null)
        {
            // Sadece 'Acik' ve 'Bos' ise arti gozuksun
            bool showPlus = isUnlocked && !isOccupied;
            plusIcon.SetActive(showPlus);
        }
    }
}
