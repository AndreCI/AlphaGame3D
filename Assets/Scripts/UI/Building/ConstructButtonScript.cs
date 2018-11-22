using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

public abstract class ConstructButtonScript : MonoBehaviour, IObserver, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public GameObject messagePanel;
    protected TextMeshProUGUI messageText;

    void Awake()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        TurnManager.Instance.ButtonUpdateSubject.AddObserver(this);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {

    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {

    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        CardDisplay.Instance.DisableCardDisplay();
        if (Selector.Instance.currentObject != null)
        {
            Selector.Instance.currentObject.UpdateCardDisplayInfo();
        }
    }
    public virtual void UpdateInfo()
    {
        UpdateText();
    }
    protected void UpdateText()
    {
        if (messageText == null)
        {
            messageText = messagePanel.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    protected void DisplayMessage(string message)
    {
        messagePanel.SetActive(true);
        UpdateText();
        messageText.text = message;
    }
    protected void RemoveDisplayMessage()
    {
        messagePanel.SetActive(false);
    }

    public void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        if (player.Equals(TurnManager.Instance.currentPlayer))
        {
            if(type==TurnSubject.NOTIFICATION_TYPE.START_OF_TURN || type == TurnSubject.NOTIFICATION_TYPE.UI_BUTTON_PRESSED)
            {
                UpdateInfo();
            }
        }
    }
}
