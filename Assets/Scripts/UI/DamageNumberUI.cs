using UnityEngine;
using TMPro;
using System.Collections;

public class DamageNumberUI : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Camera cam;

    private float lifeTime = 1f;

    private Vector3 velocity;

    private DamageNumberPool pool;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        cam = Camera.main;
    }

    public void Init(int damage, Vector3 worldPos, DamageNumberPool poolRef)
    {
        pool = poolRef;

        text.text = damage.ToString();

        transform.position = cam.WorldToScreenPoint(worldPos);

        velocity = new Vector3(Random.Range(-20f,20f), 60f,0);

        StartCoroutine(Life());
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;

        velocity.y -= 80f * Time.deltaTime;
    }

    private IEnumerator Life()
    {
        float timer = 0;

        Color startColor = text.color;

        while(timer < lifeTime)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1,0,timer/lifeTime);
            text.color = new Color(startColor.r,startColor.g,startColor.b,alpha);

            yield return null;
        }

        pool.Release(this);
    }
}