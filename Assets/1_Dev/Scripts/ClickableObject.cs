using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClickableObject : MonoBehaviour
{
    public GrapeType type; 
    public MeshRenderer myRenderer;
    private Vector3 offset;
    List<Material> groundMaterials = new List<Material>();
    public Module module;
    private BoxCollider boxCollider;

    private void Awake() {
        offset = Vector3.up * 0.15f;
        groundMaterials = ColorManager.Instance.groundMaterialList;
        boxCollider = GetComponent<BoxCollider>();
    }
    
    public void OnClick()
    {
        boxCollider.enabled = false;
        GameObject movingObject = Instantiate(ClickManager.Instance.movingObjectPrefab, transform.position, Quaternion.identity);
        MovingObject movingScript = movingObject.GetComponent<MovingObject>();

        transform.DOScale(Vector3.one * 1.4f, 0.2f).SetEase(Ease.InOutBounce).OnComplete(()=>{
            transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutQuad).OnComplete(()=>{
                boxCollider.enabled = true;
            });
        });
        
        if (movingScript != null)
        {
            movingScript.Initialize(type, transform.forward, module);
        }
    }

    public void SetColor(Material material)
    {
        if (myRenderer != null)
        {
            myRenderer.material = material;

            int materialIndex = groundMaterials.IndexOf(material);
            type = (GrapeType)materialIndex; 
        }
        else
        {
            Debug.LogError("Renderer atanmadı!");
        }
    }

    public void SetPosition(Transform targetTransform)
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position + offset;
        }
        else
        {
            Debug.LogError("Hedef transform atanmadı!");
        }
    }

    public void SetRotation(Quaternion rotation)
    {
        if (rotation != null)
        {
            transform.rotation = rotation;
        }
        else
        {
            Debug.LogError("Hedef transform atanmadı!");
        }
    }

    public void SetModule(Module module){
        this.module = module;
    }
}