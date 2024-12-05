using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GrapeObject : MonoBehaviour
{
    public GrapeType type;
    public Animator animator;

    [SerializeField] MeshRenderer[] meshRenderers;
    List<Material> groundMaterials = new List<Material>();
    private bool trigger = false;

    private void Awake() {
        groundMaterials = ColorManager.Instance.groundMaterialList;
    }

    private void OnEnable() {
        transform.DOScale(Vector3.one * 1.4f, 0.1f).SetEase(Ease.InOutBounce).OnComplete(()=>{
            transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutQuad).OnComplete(()=>{
            });
        });
    }
    
    public void SetColor(Material newMaterial){

        foreach (MeshRenderer item in meshRenderers)
        {
            item.material = newMaterial;
        }

        int materialIndex = groundMaterials.IndexOf(newMaterial);
        type = (GrapeType)materialIndex; 
    }

    public void SetTrigger()
    {
        if (trigger) return;
        trigger = true;

        Shake();
    }

    private void Shake()
    {
        Vector3 originalPosition = transform.localPosition;

        transform.DOScale(Vector3.one * 1.4f, 0.1f).SetEase(Ease.InOutBounce).OnComplete(()=>{
            transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutQuad).OnComplete(()=>{
                transform.localPosition = originalPosition;
                trigger = false;
            });
        });
    }
}
