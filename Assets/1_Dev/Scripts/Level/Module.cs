using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Module : MonoBehaviour
{
    public List<GameObject> childObjects = new List<GameObject>();
    public Transform endPos;
    public List<GameObject> directionChangers = new List<GameObject>();
    public List<GameObject> directionObjects = new List<GameObject>();
    
    [SerializeField] private Transform startPos;
    [SerializeField] private float moveSpeed = 5f;

    private List<GameObject> spawned = new List<GameObject>(); 
    private List<GameObject> grapes = new List<GameObject>();
    private bool first = false;
    private Vector3 offset;
    private ClickableObject clickableObject;

    float duration = 0.6f; 
    float jumpPower = 1.5f; 
    int jumpCount = 0; 

    private void OnEnable() {
        LevelEvents.NewModule += CheckAboveObjects;
    }

    private void Start() {
        offset = Vector3.up * 0.1f;

        CheckAboveObjects();
    }

    public void SetColor(Material newMaterial)
    {
        foreach (var child in childObjects)
        {
            if (child != null)
            {
                Renderer renderer = child.GetComponent<Renderer>();

                if (renderer != null && renderer.materials.Length > 0)
                {
                    Material[] materials = renderer.materials;
                    materials[0] = newMaterial;
                    renderer.materials = materials; 
                }
            }
        }

        clickableObject = Instantiate(LevelManager.Instance.frog);

        Transform secondChildTransform = childObjects[1].transform;
        Vector3 direction = secondChildTransform.position - startPos.position;
        Quaternion rotation = Quaternion.LookRotation(-direction);

        clickableObject.SetColor(newMaterial);
        clickableObject.SetPosition(startPos);
        clickableObject.SetRotation(rotation);
        clickableObject.SetModule(this);

        spawned.Add(clickableObject.gameObject);

        foreach (var child in childObjects)
        {
            if (child == null || directionChangers.Contains(child) || child.transform == startPos)
                continue;

            GrapeObject spawnedGrape = Instantiate(LevelManager.Instance.grape, child.transform.position, Quaternion.identity);
            spawnedGrape.SetColor(newMaterial);
            spawned.Add(spawnedGrape.gameObject);
            grapes.Add(spawnedGrape.gameObject);
            
            if (endPos == child.transform){
                spawnedGrape.tag = "End";
            }
        }

        foreach (var child in directionChangers)
        {
            if (child == null)
                continue;
            
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null && renderer.materials.Length > 0)
            {
                Material[] materials = renderer.materials;
                materials[0] = newMaterial;
                renderer.materials = materials; 
            }
            // child.GetComponent<DirectionChanger>().SetColor(newMaterial);
        }
    }

    private void CheckAboveObjects()
    {
        StartCoroutine(CheckAboveObjectsIE());
    }

    private IEnumerator CheckAboveObjectsIE()
{
    yield return new WaitForSeconds(0.1f);

    // ChildObjects ve DirectionObjects'i birlikte kontrol ediyoruz
    for (int i = 0; i < childObjects.Count; i++)
    {
        if (this == null) yield break;

        if (spawned[i] == null)
        {
            continue;
        }

        // Child Object için Raycast
        Ray ray = new Ray(childObjects[i].transform.position, Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            spawned[i].gameObject.SetActive(false);
        }
        else
        {
            spawned[i].gameObject.SetActive(true);
        }
    }

    // DirectionObjects'in kontrolü
    for (int j = 0; j < directionChangers.Count; j++)
    {
        if (this == null) yield break;

        GameObject directionObject = directionObjects[j];
        GameObject directionChanger = directionChangers[j];

        if (directionObject == null || directionChanger == null)
            continue;

        Ray ray = new Ray(directionChanger.transform.position, Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject != directionObject)
                directionObject.SetActive(false);
        }
        else
        {
            directionObject.SetActive(true);
            directionObject.transform.position = directionChanger.transform.position + (Vector3.up * 0.15f);
        }
    }
}

    public void StartMoving(){
        StartCoroutine(MoveObjectsToStart());
    }

    public IEnumerator MoveObjectsToStart()
    {
        List<Tween> tweens = new List<Tween>(); 

        for (int i = grapes.Count - 1; i >= 0; i--)
        {
            if (this == null) yield break;

            GameObject currentObject = grapes[i];

            if (currentObject == null)
                continue;

            currentObject.GetComponent<GrapeObject>().animator.SetBool("Open", true);
            
            yield return new WaitForSeconds(0.1f);

            tweens.Add(
                currentObject.transform.DOJump(startPos.position + offset, jumpPower, jumpCount, duration)
            );
            tweens.Add(
                currentObject.transform.DOScale(Vector3.one * 0.8f, duration).OnComplete(() => Destroy(currentObject))
            );
        }

        foreach (var tween in tweens)
            yield return tween.WaitForCompletion();

        tweens.Clear();

        for (int i = childObjects.Count - 1; i >= 0; i--)
        {
            if (this == null) yield break;
            if (childObjects[i] == null) continue;

            tweens.Add(childObjects[i].transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => Destroy(childObjects[i])));
        }

        foreach (var tween in tweens)
            yield return tween.WaitForCompletion();

        tweens.Clear();

        if (startPos != null)
        {
            tweens.Add(startPos.DOScale(Vector3.zero, 0.2f));
        }

        if (clickableObject != null)
        {
            tweens.Add(clickableObject.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => Destroy(clickableObject.gameObject)));
        }

        foreach (var tween in tweens)
            yield return tween.WaitForCompletion();

        LevelEvents.NewModule -= CheckAboveObjects;
        LevelEvents.TriggerNewModule();

        if (this != null && gameObject != null)
        {
            Destroy(gameObject);
        }
    }

}