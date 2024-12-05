using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public static ClickManager Instance;
    public GameObject movingObjectPrefab; 

    private void Awake() {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ClickableObject clickable = hit.collider.GetComponent<ClickableObject>();
                if (clickable != null)
                {
                    clickable.OnClick();
                }
            }
        }
    }
}
