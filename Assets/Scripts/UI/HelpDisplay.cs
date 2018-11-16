using UnityEngine;
using UnityEngine.EventSystems;

public class HelpDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject help;

    public void OnPointerEnter(PointerEventData eventData)
    {
        help.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        help.SetActive(false);
    }
}
