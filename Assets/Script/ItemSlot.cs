using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop to slot");

        // CH? CH?A 1 ITEM - n?u ?ã có item thì không cho drop
        if (Item == null)
        {
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = Vector2.zero;
        }
        else
        {
            Debug.Log("Slot already has an item!");
        }
    }
}