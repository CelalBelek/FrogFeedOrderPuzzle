using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance;
    public List<Material> groundMaterialList;
    public List<Material> frogMaterialList;

    private void Awake()
    {
        // Singleton örneği
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private List<Material> GenerateColorList(List<Material> sourceList, int count)
    {
        if (sourceList == null || sourceList.Count == 0)
        {
            Debug.LogError("Material listesi boş veya atanmadı!");
            return new List<Material>();
        }

        List<Material> colorList = new List<Material>();
        int index = 0; // Sıra ile ilerlemek için başlangıç indeksi

        for (int i = 0; i < count; i++)
        {
            // Sıradaki materyali seç ve listeye ekle
            colorList.Add(sourceList[index]);

            // İndeksi bir artır, eğer son materyali geçtiysek başa dön
            index = (index + 1) % sourceList.Count;
        }

        return colorList;
    }
    public List<Material> GenerateGroundColorList(int count)
    {
        return GenerateColorList(groundMaterialList, count);
    }
    public List<Material> GenerateFrogColorList(int count)
    {
        return GenerateColorList(frogMaterialList, count);
    }
}
