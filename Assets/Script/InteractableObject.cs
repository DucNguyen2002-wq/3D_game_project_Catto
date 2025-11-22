using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ItemName;
    public bool canBePickedUp = false; // Có thể nhặt được không

    public string GetItemName()
    {
        return ItemName;
    }

    void Start()
    {
        // Tự động thêm tag "canPickUp" nếu object có thể nhặt
        if (canBePickedUp && !gameObject.CompareTag("canPickUp"))
        {
            gameObject.tag = "canPickUp";
        }
    }
}
