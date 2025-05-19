using UnityEngine;

/// <summary>
/// �ehir objesine otomatik olarak BoxCollider ekler.
/// T�m alt objelerin mesh'lerine g�re collider boyutunu ayarlar.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class AutoCityCollider : MonoBehaviour
{
    void Reset()
    {
        GenerateColliderBounds();
    }

    void OnValidate()
    {
        // Edit�rde de�i�iklik olursa otomatik g�ncelle
        GenerateColliderBounds();
    }

    void GenerateColliderBounds()
    {
        // T�m MeshRenderer'lar� al
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"No mesh renderers found under {gameObject.name}.");
            return;
        }

        // �lk mesh ile ba�la
        Bounds combinedBounds = renderers[0].bounds;

        // Di�er mesh'lerle birle�tir
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        // Yerel pozisyona �evir
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 centerLocal = transform.InverseTransformPoint(combinedBounds.center);
        box.center = centerLocal;
        box.size = combinedBounds.size;

        // Collider fizik etkile�imi istemiyorsan:
        // box.isTrigger = true;
    }
}