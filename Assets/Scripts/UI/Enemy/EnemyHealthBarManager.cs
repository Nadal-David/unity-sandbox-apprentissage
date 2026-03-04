using UnityEngine;

public class EnemyHealthBarManager : MonoBehaviour
{
    public static EnemyHealthBarManager Instance;

    [SerializeField] private EnemyHealthBarUI healthBarPrefab;

    private Canvas canvas;

    private void Awake()
    {
        Instance = this;
        canvas = GetComponent<Canvas>();
    }

    public EnemyHealthBarUI CreateBar(Transform enemy)
    {
        EnemyHealthBarUI bar = Instantiate(healthBarPrefab, canvas.transform);
        bar.gameObject.SetActive(false); 
        bar.Init(enemy);

        return bar;
    }
}