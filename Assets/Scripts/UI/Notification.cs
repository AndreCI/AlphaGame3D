using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Notification : MonoBehaviour, IPointerClickHandler
{
    public enum NOTIFICATION_TYPE
    {
        START_GAME,
        NEED_FOOD,
        SPELL_CASTABLE,
        BUILDING_FINISH,
        UNIT_DEATH
    }
    public Image image;
    public Image imageMask;
    public Image imageDescription;
    public NOTIFICATION_TYPE type;
    public TextMeshProUGUI text;
    public GameObject handler;
    public RectTransform rectTransform;
    public HexCell target;
    private bool moving;
    private bool appearing;
    private bool disappearing;
    private float oldPosition;
    private float newPosition;
    private float counter;
    public bool active;


    public void Setup(string text_, NOTIFICATION_TYPE type_, Sprite image_, HexCell target_ = null)
    {
        target = target_;
        text.text = text_;
        type = type_;
        imageDescription.sprite = image_;
          
    }

    public void Update()
    {
        if (moving)
        {
            counter+= Time.deltaTime;
            //Vector3.Lerp(oldPosition, newPosition, counter); 
            rectTransform.anchoredPosition = new Vector3(0,
                    (1 - counter) * oldPosition + counter * newPosition,
                    0);
            if (counter >= 1f)
            {
                rectTransform.anchoredPosition = new Vector3(0, newPosition, 0);
                
                moving = false;
            }
        }else if (appearing)
        {
            counter += Time.deltaTime;
            image.canvasRenderer.SetAlpha(counter);
            imageDescription.canvasRenderer.SetAlpha(counter);
            imageMask.canvasRenderer.SetAlpha(counter);
            text.canvasRenderer.SetAlpha(counter);
            if (counter >= 1f)
            {
                appearing = false;
                active = true;
            }
        }else if (disappearing)
        {
            counter += Time.deltaTime;
            image.canvasRenderer.SetAlpha(1 - counter);
            imageDescription.canvasRenderer.SetAlpha(1 - counter);
            imageMask.canvasRenderer.SetAlpha(1 - counter);
            text.canvasRenderer.SetAlpha(1 - counter);
            if (counter >= 1f)
            {
                disappearing = false;
                gameObject.SetActive(false);
            }
        }
    }
    public void Add(GameObject handler_)
    {
        counter = 0f;
        handler = handler_;
        active = false;
        transform.SetParent(handler.transform);
        image.canvasRenderer.SetAlpha(0);
        text.canvasRenderer.SetAlpha(0);
        imageMask.canvasRenderer.SetAlpha(0);
        imageDescription.canvasRenderer.SetAlpha(0);
        rectTransform.anchoredPosition = new Vector3(0, 0, 0);// handler.rectTransform.anchoredPosition.y, 0);
        active = true;
        appearing = true;
    }

    public void ChangeHandler(GameObject handler_)
    {
        transform.SetParent(handler_.transform);
        oldPosition = rectTransform.anchoredPosition.y;
        newPosition = 0;// handler_.transform.position.y;
        moving = true;
        handler = handler_;
        counter = 0f;
    }

    public void Remove()
    {
        counter = 0f;
        disappearing = true;
        active = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (active)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                NotificationsList.Instance.RemoveNotification(this);
                Remove();
            }else if(eventData.button == PointerEventData.InputButton.Left && target)
            {
                HexMapCamera.instance.SetPosition(target.Position.x, target.Position.z);
            }else if(eventData.button == PointerEventData.InputButton.Left && type == NOTIFICATION_TYPE.START_GAME)
            {
                GameManager.Instance.tutorialStart.SetActive(true);
                NotificationsList.Instance.RemoveNotification(this);
                Remove();
            }
        }
    }
}

