using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardArea : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData) 
    {
		if(eventData.pointerDrag == null)
        {
            return;
        }

		DraggableObject d = eventData.pointerDrag.GetComponent<DraggableObject>();
		if(d != null) 
        {
			d.placeholderParent = this.transform;
		}
	}
	
	public void OnPointerExit(PointerEventData eventData) 
    {
		if(eventData.pointerDrag == null)
        {
            return;
        }

		DraggableObject d = eventData.pointerDrag.GetComponent<DraggableObject>();
		if(d != null && d.placeholderParent == this.transform) 
        {
			d.placeholderParent = d.playerHand;
		}
	}
	
	public void OnDrop(PointerEventData eventData) 
    {
		DraggableObject d = eventData.pointerDrag.GetComponent<DraggableObject>();
		if(d != null) 
        {
			d.playerHand = this.transform;
		}

        Debug.Log (eventData.pointerDrag.name + " was dropped on " + gameObject.name);

	}
}
