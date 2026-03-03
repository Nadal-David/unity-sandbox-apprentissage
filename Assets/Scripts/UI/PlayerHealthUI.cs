using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerHealthUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TextMeshProUGUI healthText;

    private Player player;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        UpdateHealthBar();
        healthText.gameObject.SetActive(false);
    }

    public void UpdateHealthBar()
    {
        if (player == null) return;

        float healthPercent = player.GetHealthPercent();
        healthFillImage.fillAmount = Mathf.Clamp01(healthPercent);

        healthText.text = $"{player.GetHealth()} / {player.GetMaxHealth()}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        healthText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        healthText.gameObject.SetActive(false);
    }
}