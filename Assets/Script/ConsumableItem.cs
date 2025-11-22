using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    [Header("Item Type")]
    public ItemType itemType = ItemType.Food;
    
    [Header("Consumable Values")]
    public float hungerRestore = 30f;
    public float thirstRestore = 30f;
    
    public enum ItemType
    {
        Food,
        Water,
        Both
    }

    public void Consume()
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogError("[ConsumableItem] PlayerStats.Instance is NULL!");
            return;
        }
        
        switch (itemType)
        {
            case ItemType.Food:
                PlayerStats.Instance.AddHunger(hungerRestore);
                Debug.Log($"[ConsumableItem] Consumed {gameObject.name} - Restored {hungerRestore} hunger");
                break;
                
            case ItemType.Water:
                PlayerStats.Instance.AddThirst(thirstRestore);
                Debug.Log($"[ConsumableItem] Consumed {gameObject.name} - Restored {thirstRestore} thirst");
                break;
                
            case ItemType.Both:
                PlayerStats.Instance.AddHunger(hungerRestore);
                PlayerStats.Instance.AddThirst(thirstRestore);
                Debug.Log($"[ConsumableItem] Consumed {gameObject.name} - Restored {hungerRestore} hunger, {thirstRestore} thirst");
                break;
        }
        
        Destroy(gameObject);
    }
}
