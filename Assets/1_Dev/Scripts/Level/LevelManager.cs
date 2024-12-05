using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public ClickableObject frog;
    public GrapeObject grape;

    [SerializeField] Level[] levels;
    Level level;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        int newLevel = Random.Range(0, levels.Length);
        level = Instantiate(levels[newLevel]);
    }
}
