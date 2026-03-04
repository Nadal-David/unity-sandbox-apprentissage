using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fill;

    private Transform target;
    private Camera cam;

    private Vector3 offset = new Vector3(0, 1f, 0);

    private float targetFill;
    private Coroutine hideCoroutine;

    public void Init(Transform enemy)
    {
        target = enemy;
        cam = Camera.main;

        gameObject.SetActive(false); // caché au début
    }

    public void SetHealth(float current, float max)
    {
        targetFill = current / max;

        // affiche la barre
        gameObject.SetActive(true);

        // reset timer de disparition
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private void Update()
    {
        // animation smooth
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, targetFill, Time.deltaTime * 10f);
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);

        if (screenPos.z < 0) return;

        transform.position = screenPos;
    }
}