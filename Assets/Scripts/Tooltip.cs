using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipText;
    public int size = 1;
    GameObject tooltip;

    bool hover;
    float hoverdelayDelta;
    float hoverdelaytime;
    bool interactable;

    void Start() {
        tooltip = this.transform.GetChild(2).gameObject;
        hoverdelaytime = 0.5f;
        tooltip.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(tooltip.transform.GetComponent<RectTransform>().sizeDelta.x, size * 15);
    }
    
    void Update() {
        interactable = this.GetComponent<Button>().interactable;
        if (hover && interactable && hoverdelayDelta >= hoverdelaytime) {
            tooltip.SetActive(true);
            tooltip.transform.GetChild(0).GetComponent<TMP_Text>().text = tooltipText;
            this.transform.parent.SetAsLastSibling();
            
        } else if (hover && interactable && hoverdelayDelta < hoverdelaytime) {
            hoverdelayDelta += Time.deltaTime;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover = false;
        hoverdelayDelta = 0;
        tooltip.SetActive(false);
    }
}