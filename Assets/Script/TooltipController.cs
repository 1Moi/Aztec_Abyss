using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject tooltipPanel;
    private Image tooltipImage;
    private TextMeshProUGUI abilityNameText;
    private bool isPointerOver = false;
    private Vector2 offset = new Vector2(350, 20);

    void Start()
    {
        abilityNameText = transform.Find("AbilityName").GetComponent<TextMeshProUGUI>();

        if (TooltipPanelController.Instance != null)
        {
            tooltipPanel = TooltipPanelController.Instance.gameObject;
            tooltipImage = TooltipPanelController.Instance.GetComponent<Image>();
        }

        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);
        }
        string abilityName = abilityNameText.text;
        Sprite abilitySprite = Resources.Load<Sprite>("HoverAbilityInfo/" + abilityName);
        if (abilitySprite != null && tooltipImage != null)
        {
            tooltipImage.sprite = abilitySprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isPointerOver && tooltipPanel != null)
        {
            Vector2 mousePosition = Input.mousePosition;
            tooltipPanel.transform.position = mousePosition + offset;
        }
    }
}
