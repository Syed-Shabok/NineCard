using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardArea : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Runs when player's coursor enters the Card Area. 
	public void OnPointerEnter(PointerEventData eventData) 
    {
		if(eventData.pointerDrag == null)
        {
            return;
        }

		DraggableObject d = eventData.pointerDrag.GetComponent<DraggableObject>();
		if(d != null) 
        {
			d.PlaceholderParent = this.transform;
		}
	}
	
	// Runs when player's cursor exits the Card Area. 
	public void OnPointerExit(PointerEventData eventData) 
    {
		if(eventData.pointerDrag == null)
        {
            return;
        }

		DraggableObject d = eventData.pointerDrag.GetComponent<DraggableObject>();
		if(d != null && d.PlaceholderParent == this.transform) 
        {
			d.PlaceholderParent = d.PlayerHand;
		}
	}
	
	// Runs when player drops card into Card Area. 
	public void OnDrop(PointerEventData eventData) 
    {
		DraggableObject d = eventData.pointerDrag.GetComponent<DraggableObject>();
		if(d != null) 
        {
			d.PlayerHand = this.transform;
		}

        Debug.Log (eventData.pointerDrag.name + " was dropped on " + gameObject.name);

	}
}
