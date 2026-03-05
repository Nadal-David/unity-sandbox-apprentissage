using UnityEngine;

public class FadeOnPlayerBehind : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] sprites;
    [SerializeField] private float fadeAlpha = 0.35f;
    [SerializeField] private float fadeSpeed = 5f;

    private float targetAlpha = 1f;

    void Update()
    {
        foreach (var sr in sprites)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            sr.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            targetAlpha = fadeAlpha;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            targetAlpha = 1f;
        }
    }
}