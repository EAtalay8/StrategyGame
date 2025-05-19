using UnityEngine;

/// <summary>
/// Þehir objesine otomatik olarak BoxCollider ekler.
/// Tüm alt objelerin mesh'lerine göre collider boyutunu ayarlar.
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
        // Editörde deðiþiklik olursa otomatik güncelle
        GenerateColliderBounds();
    }

    void GenerateColliderBounds()
    {
        // Tüm MeshRenderer'larý al
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"No mesh renderers found under {gameObject.name}.");
            return;
        }

        // Ýlk mesh ile baþla
        Bounds combinedBounds = renderers[0].bounds;

        // Diðer mesh'lerle birleþtir
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        // Yerel pozisyona çevir
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 centerLocal = transform.InverseTransformPoint(combinedBounds.center);
        box.center = centerLocal;
        box.size = combinedBounds.size;

        // Collider fizik etkileþimi istemiyorsan:
        // box.isTrigger = true;
    }
}