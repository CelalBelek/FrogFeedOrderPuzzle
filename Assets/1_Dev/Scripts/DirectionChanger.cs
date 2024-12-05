using UnityEngine;
using System.Collections.Generic;

public class DirectionChanger : MonoBehaviour
{
    public Vector3 newDirection = Vector3.forward; // Yeni yön
    [SerializeField] MeshRenderer myRenderer;
    List<Material> groundMaterials = new List<Material>();

    private void Awake() {
        groundMaterials = ColorManager.Instance.groundMaterialList;
    }

    public Vector3 GetNewDirection()
    {
        return newDirection; // Forward yönünü döndür
    }

     public void SetColor(Material material)
    {
        if (myRenderer != null)
        {
            myRenderer.material = material;

            int materialIndex = groundMaterials.IndexOf(material);
        }
        else
        {
            Debug.LogError("Renderer atanmadı!");
        }
    }
}
