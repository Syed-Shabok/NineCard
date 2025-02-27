using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    public Transform PlayerHand = null;
	public Transform PlaceholderParent = null;
	private GameObject _placeholder = null;
	
	// Runs when player starts dragging a card.
	public void OnBeginDrag(PointerEventData eventData) 
    {
		_placeholder = new GameObject();
		_placeholder.transform.SetParent(this.transform.parent);
		LayoutElement le = _placeholder.AddComponent<LayoutElement>();

		_placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
		
		PlayerHand = this.transform.parent;
		PlaceholderParent = PlayerHand;
		this.transform.SetParent(this.transform.parent.parent);
		
		GetComponent<Image>().raycastTarget = false;

        Debug.Log ("Started Dragging.");
	}
	
	// Runs while player is dragging a card.
	public void OnDrag(PointerEventData eventData) 
    {
		this.transform.position = eventData.position;

		if(_placeholder.transform.parent != PlaceholderParent)
        {
            _placeholder.transform.SetParent(PlaceholderParent);
        }

		int newSiblingIndex = PlaceholderParent.childCount;

		for(int i=0; i < PlaceholderParent.childCount; ++i) 
        {
			if(this.transform.position.x < PlaceholderParent.GetChild(i).position.x) 
            {
				newSiblingIndex = i;

				if(_placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    --newSiblingIndex;
                }

				break;
			}
		}

		_placeholder.transform.SetSiblingIndex(newSiblingIndex);
	}
	
	// Runs when player stops dragging a card.
	public void OnEndDrag(PointerEventData eventData) 
    {
		this.transform.SetParent(PlayerHand);
		this.transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());
		GetComponent<Image>().raycastTarget = true;

		Destroy(_placeholder);

        Debug.Log ("Stopped Dragging.");
	}
}
