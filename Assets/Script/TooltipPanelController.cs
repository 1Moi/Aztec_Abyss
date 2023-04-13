using UnityEngine;

public class TooltipPanelController : MonoBehaviour
{
    public static TooltipPanelController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
