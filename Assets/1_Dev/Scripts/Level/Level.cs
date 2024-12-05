using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Level : MonoBehaviour
{
    public List<GameObject> modulesObject = new List<GameObject>();
    public List<Module> modules = new List<Module>();

    [SerializeField] private List<Material> groundMaterials;

    private void OnEnable()
    {
        modulesObject.Clear();
        modules.Clear(); 

        foreach (Transform child in transform)
        {
            Module module = child.GetComponent<Module>();
            if (module != null)
            {
                modulesObject.Add(module.gameObject);
            }
        }

        foreach (GameObject item in modulesObject)
        {
            modules.Add(item.GetComponent<Module>());
        }

        groundMaterials = ColorManager.Instance.GenerateGroundColorList(modules.Count);
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].SetColor(groundMaterials[i]);
        }
    }
}
