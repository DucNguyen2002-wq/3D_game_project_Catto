using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ItemName;
    public bool canBePickedUp = false;

    public string GetItemName()
    {
        return ItemName;
    }

    void Start()
    {
        if (canBePickedUp && !gameObject.CompareTag("canPickUp"))
        {
            gameObject.tag = "canPickUp";
        }
    }
}
