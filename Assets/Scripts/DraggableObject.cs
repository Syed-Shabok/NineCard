using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    public Transform playerHand = null;
	public Transform placeholderParent = null;
	private GameObject placeholder = null;
	
	public void OnBeginDrag(PointerEventData eventData) 
    {
		placeholder = new GameObject();
		placeholder.transform.SetParent(this.transform.parent);
		LayoutElement le = placeholder.AddComponent<LayoutElement>();

		placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
		
		playerHand = this.transform.parent;
		placeholderParent = playerHand;
		this.transform.SetParent(this.transform.parent.parent);
		
		GetComponent<Image>().raycastTarget = false;

        Debug.Log ("Started Dragging.");
	}
	
	public void OnDrag(PointerEventData eventData) 
    {
		this.transform.position = eventData.position;

		if(placeholder.transform.parent != placeholderParent)
        {
            placeholder.transform.SetParent(placeholderParent);
        }

		int newSiblingIndex = placeholderParent.childCount;

		for(int i=0; i < placeholderParent.childCount; ++i) 
        {
			if(this.transform.position.x < placeholderParent.GetChild(i).position.x) 
            {
				newSiblingIndex = i;

				if(placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    --newSiblingIndex;
                }

				break;
			}
		}

		placeholder.transform.SetSiblingIndex(newSiblingIndex);
	}
	
	public void OnEndDrag(PointerEventData eventData) 
    {
		this.transform.SetParent(playerHand);
		this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
		GetComponent<Image>().raycastTarget = true;

		Destroy(placeholder);

        Debug.Log ("Stopped Dragging.");
	}
}
