using UnityEngine;
using System.Collections.Generic;

public class DamageNumberPool : MonoBehaviour
{
    [SerializeField] private DamageNumberUI prefab;
    [SerializeField] private int startAmount = 20;

    private Queue<DamageNumberUI> pool = new();

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();

        for (int i = 0; i < startAmount; i++)
        {
            Create();
        }
    }

    private DamageNumberUI Create()
    {
        DamageNumberUI obj = Instantiate(prefab, canvas.transform);
        obj.gameObject.SetActive(false);

        pool.Enqueue(obj);

        return obj;
    }

    public DamageNumberUI Get()
    {
        if (pool.Count == 0)
            Create();

        DamageNumberUI obj = pool.Dequeue();
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void Release(DamageNumberUI obj)
    {
        obj.gameObject.SetActive(false);

        pool.Enqueue(obj);
    }

    public void Spawn(int damage, Vector3 worldPos)
    {
        DamageNumberUI obj = Get();
        obj.Init(damage, worldPos, this);
    }
}