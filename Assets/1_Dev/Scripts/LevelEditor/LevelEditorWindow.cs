using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LevelEditorWindow : EditorWindow
{
    private Dictionary<string, List<GameObject>> categorizedPrefabs = new Dictionary<string, List<GameObject>>()
    {
        { "Single", new List<GameObject>() },
        { "Diagonal", new List<GameObject>() },
        { "Zigzag", new List<GameObject>() }
    };

    private List<GameObject> sceneObjects = new List<GameObject>(); // Sahneye eklenen geçici objelerin listesi
    private string selectedCategory = "Single"; // Varsayılan kategori
    private string prefabFolderPath = "Assets/1_Dev/Prefabs/Levels"; // Kayıt klasörü
    private string levelName = "Level_0"; // Level ismi
    private int objectCount = 1; // Kullanıcının girdiği obje sayısı
    private float verticalOffset = 0.1f; // Y eksenindeki mesafe (yükseklik)

    private Vector2 scrollPosition; // Prefab listesi için kaydırma
    private float thumbnailSize = 64f; // Thumbnail boyutu
    private float currentHeight = 0f; // Bir sonraki objenin yerleştirileceği yükseklik

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Category Selection", EditorStyles.boldLabel);

        // Kategori Butonları
        GUILayout.BeginHorizontal(); // Butonları yan yana dizmek için
        DrawCategoryButton("Single");
        DrawCategoryButton("Diagonal");
        DrawCategoryButton("Zigzag");
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label($"{selectedCategory} Prefabs", EditorStyles.boldLabel);

        // Kategoriye Göre Prefab Listesi
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        if (categorizedPrefabs.ContainsKey(selectedCategory))
        {
            List<GameObject> prefabList = categorizedPrefabs[selectedCategory];
            for (int i = 0; i < prefabList.Count; i++)
            {
                GUILayout.BeginHorizontal();

                // Thumbnail Gösterimi
                Texture2D preview = prefabList[i] != null ? AssetPreview.GetAssetPreview(prefabList[i]) : null;
                if (preview != null)
                {
                    if (GUILayout.Button(preview, GUILayout.Width(thumbnailSize), GUILayout.Height(thumbnailSize)))
                    {
                        PlacePrefabInScene(prefabList[i]);
                    }
                }
                else
                {
                    GUILayout.Label("No Preview", GUILayout.Width(thumbnailSize), GUILayout.Height(thumbnailSize));
                }

                // Prefab Object Field
                prefabList[i] = (GameObject)EditorGUILayout.ObjectField(prefabList[i], typeof(GameObject), false);

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    prefabList.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Prefab"))
        {
            AddPrefabToCategory();
        }

        GUILayout.Space(10);

        GUILayout.Label("Drag GameObjects Here to Add", EditorStyles.boldLabel);

        // Sürükle Bırak İşlemi
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop Prefabs Here");

        HandleDragAndDrop(dropArea);

        GUILayout.Space(10);

        GUILayout.Label("Automatic Placement", EditorStyles.boldLabel);

        objectCount = EditorGUILayout.IntField("Object Count", objectCount);
        verticalOffset = EditorGUILayout.FloatField("Vertical Offset", verticalOffset);

        if (GUILayout.Button("Place Objects Automatically"))
        {
            PlaceObjectsAutomatically();
        }

        GUILayout.Space(10);

        levelName = EditorGUILayout.TextField("Level Name", levelName);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Level as Prefab"))
        {
            AlignSceneObjectsInstantly();
            SaveLevelAsPrefab();
        }

        if (GUILayout.Button("Clear"))
        {
            AlignSceneObjectsInstantly();
            ClearSceneObjects();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("Alignment Options", EditorStyles.boldLabel);

        if (GUILayout.Button("Align Scene Objects"))
        {
            AlignSceneObjectsInstantly();
        }
    }

    private void DrawCategoryButton(string categoryName)
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);

        if (categoryName == selectedCategory)
        {
            style.normal.textColor = Color.green;
        }

        if (GUILayout.Button(categoryName, style, GUILayout.Width(100)))
        {
            selectedCategory = categoryName;
        }
    }

    private void HandleDragAndDrop(Rect dropArea)
    {
        Event evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is GameObject gameObject)
                        {
                            categorizedPrefabs[selectedCategory].Add(gameObject);
                        }
                    }

                    evt.Use();
                }
                break;
        }
    }

    private void AddPrefabToCategory()
    {
        if (categorizedPrefabs.ContainsKey(selectedCategory))
        {
            categorizedPrefabs[selectedCategory].Add(null);
        }
    }

    private void PlacePrefabInScene(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("No prefab selected!");
            return;
        }

        Vector3 originalPosition = prefab.transform.localPosition;
        Vector3 spawnPosition = new Vector3(originalPosition.x, currentHeight, originalPosition.z);

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = spawnPosition;

        currentHeight += verticalOffset;
        sceneObjects.Add(instance);

        Selection.activeGameObject = instance;
    }

    private void PlaceObjectsAutomatically()
    {
        if (!categorizedPrefabs.ContainsKey(selectedCategory) || categorizedPrefabs[selectedCategory].Count == 0)
        {
            Debug.LogError("No prefabs in the selected category!");
            return;
        }

        for (int i = 0; i < objectCount; i++)
        {
            GameObject prefab = categorizedPrefabs[selectedCategory][Random.Range(0, categorizedPrefabs[selectedCategory].Count)];
            PlacePrefabInScene(prefab);
        }

        currentHeight = 0f;
    }

   private void AlignSceneObjectsInstantly()
{
    foreach (GameObject currentObject in sceneObjects)
    {
        if (currentObject == null)
            continue;

        Module module = currentObject.GetComponent<Module>();
        if (module == null || (module.childObjects == null && module.directionChangers == null))
            continue;

        // ChildObjects ve DirectionChangers listelerini birleştir
        List<GameObject> allObjects = new List<GameObject>();
        if (module.childObjects != null)
            allObjects.AddRange(module.childObjects);
        if (module.directionChangers != null)
            allObjects.AddRange(module.directionChangers);

        foreach (GameObject obj in allObjects)
        {
            if (obj == null)
                continue;

            Ray ray = new Ray(obj.transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.GetComponent<Module>() || hitObject.CompareTag("Ground"))
                {
                    Vector3 newPosition = new Vector3(
                        obj.transform.position.x,
                        hit.point.y + verticalOffset,
                        obj.transform.position.z
                    );

                    Undo.RecordObject(obj.transform, "Align Object");
                    obj.transform.position = newPosition;
                }
            }
        }
    }
}

    private void SaveLevelAsPrefab()
    {
        if (string.IsNullOrEmpty(levelName))
        {
            Debug.LogError("Level name is empty!");
            return;
        }

        if (sceneObjects.Count == 0)
        {
            Debug.LogError("No objects to save!");
            return;
        }

        if (!AssetDatabase.IsValidFolder(prefabFolderPath))
        {
            string[] folders = prefabFolderPath.Split('/');
            string currentPath = "Assets";

            for (int i = 1; i < folders.Length; i++)
            {
                string nextFolder = $"{currentPath}/{folders[i]}";
                if (!AssetDatabase.IsValidFolder(nextFolder))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = nextFolder;
            }
        }

        GameObject levelParent = new GameObject(levelName);

        foreach (GameObject obj in sceneObjects)
        {
            obj.transform.SetParent(levelParent.transform);
        }

        levelParent.AddComponent<Level>();
        levelParent.GetComponent<Level>().modulesObject = sceneObjects;
        string path = $"{prefabFolderPath}/{levelName}.prefab";
        PrefabUtility.SaveAsPrefabAsset(levelParent, path);

        DestroyImmediate(levelParent);

        Debug.Log($"Level saved as prefab at {path}");

        sceneObjects.Clear();
        currentHeight = 0f;
    }

    private void ClearSceneObjects()
    {
        foreach (GameObject obj in sceneObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

        sceneObjects.Clear();
        currentHeight = 0f;
    }
}
