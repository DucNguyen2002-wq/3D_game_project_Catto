using UnityEngine;
using System.Collections.Generic;

public class QuestRewardSystem : MonoBehaviour
{
    [Header("Reward Prefabs")]
    public GameObject[] foodPrefabs;
    public GameObject[] waterPrefabs;
    
    [Header("Reward Settings")]
    public Transform rewardSpawnPoint;
    public int minRewardCount = 2;
    public int maxRewardCount = 5;
    
    [Header("Spawn Settings")]
    public float horizontalSpread = 0.5f;
    public float verticalOffset = 0.5f;
    public float stackHeight = 0.2f;

    public void GiveReward()
    {
        if (!ValidateRewardSettings())
        {
            return;
        }
        
        List<GameObject> rewardItems = GenerateRewardList();
        SpawnRewardItems(rewardItems);
        
        Debug.Log($"[QuestRewardSystem] Da trao {rewardItems.Count} reward items!");
    }

    private bool ValidateRewardSettings()
    {
        if (rewardSpawnPoint == null)
        {
            Debug.LogError("[QuestRewardSystem] Reward Spawn Point chua duoc gan!");
            return false;
        }
        
        if (foodPrefabs == null || foodPrefabs.Length == 0)
        {
            Debug.LogWarning("[QuestRewardSystem] Khong co Food Prefabs de reward!");
            return false;
        }
        
        if (waterPrefabs == null || waterPrefabs.Length == 0)
        {
            Debug.LogWarning("[QuestRewardSystem] Khong co Water Prefabs de reward!");
            return false;
        }
        
        // Validate Food Prefabs
        Debug.Log($"[QuestRewardSystem] Validating {foodPrefabs.Length} food prefabs:");
        for (int i = 0; i < foodPrefabs.Length; i++)
        {
            if (foodPrefabs[i] != null)
            {
                ConsumableItem consumable = foodPrefabs[i].GetComponent<ConsumableItem>();
                if (consumable != null)
                {
                    Debug.Log($"  - Food Prefab {i}: {foodPrefabs[i].name} (Type: {consumable.itemType})");
                    if (consumable.itemType != ConsumableItem.ItemType.Food)
                    {
                        Debug.LogWarning($"    WARNING: Food prefab '{foodPrefabs[i].name}' co itemType = {consumable.itemType}!");
                    }
                }
                else
                {
                    Debug.LogWarning($"  - Food Prefab {i}: {foodPrefabs[i].name} KHONG CO ConsumableItem component!");
                }
            }
        }
        
        // Validate Water Prefabs
        Debug.Log($"[QuestRewardSystem] Validating {waterPrefabs.Length} water prefabs:");
        for (int i = 0; i < waterPrefabs.Length; i++)
        {
            if (waterPrefabs[i] != null)
            {
                ConsumableItem consumable = waterPrefabs[i].GetComponent<ConsumableItem>();
                if (consumable != null)
                {
                    Debug.Log($"  - Water Prefab {i}: {waterPrefabs[i].name} (Type: {consumable.itemType})");
                    if (consumable.itemType != ConsumableItem.ItemType.Water)
                    {
                        Debug.LogWarning($"    WARNING: Water prefab '{waterPrefabs[i].name}' co itemType = {consumable.itemType}!");
                    }
                }
                else
                {
                    Debug.LogWarning($"  - Water Prefab {i}: {waterPrefabs[i].name} KHONG CO ConsumableItem component!");
                }
            }
        }
        
        return true;
    }

    private List<GameObject> GenerateRewardList()
    {
        int totalRewards = Random.Range(minRewardCount, maxRewardCount + 1);
        List<GameObject> rewardItems = new List<GameObject>();
        
        Debug.Log($"[QuestRewardSystem] Generating {totalRewards} rewards...");
        
        // Them 1 food BAT BUOC
        GameObject randomFood = foodPrefabs[Random.Range(0, foodPrefabs.Length)];
        rewardItems.Add(randomFood);
        Debug.Log($"  - Reward 1: {randomFood.name} (FOOD - BAT BUOC)");
        
        // Them 1 water BAT BUOC
        GameObject randomWater = waterPrefabs[Random.Range(0, waterPrefabs.Length)];
        rewardItems.Add(randomWater);
        Debug.Log($"  - Reward 2: {randomWater.name} (WATER - BAT BUOC)");
        
        // Them cac item con lai (ngau nhien food hoac water)
        for (int i = 2; i < totalRewards; i++)
        {
            bool isFood = Random.value > 0.5f;
            if (isFood)
            {
                GameObject food = foodPrefabs[Random.Range(0, foodPrefabs.Length)];
                rewardItems.Add(food);
                Debug.Log($"  - Reward {i+1}: {food.name} (FOOD - random)");
            }
            else
            {
                GameObject water = waterPrefabs[Random.Range(0, waterPrefabs.Length)];
                rewardItems.Add(water);
                Debug.Log($"  - Reward {i+1}: {water.name} (WATER - random)");
            }
        }
        
        return rewardItems;
    }

    private void SpawnRewardItems(List<GameObject> rewardItems)
    {
        Debug.Log($"[QuestRewardSystem] Dang spawn {rewardItems.Count} reward items...");
        
        for (int i = 0; i < rewardItems.Count; i++)
        {
            Vector3 spawnPos = CalculateSpawnPosition(i);
            GameObject rewardItem = Instantiate(rewardItems[i], spawnPos, Quaternion.identity);
            
            // Log thong tin prefab va instance
            ConsumableItem prefabConsumable = rewardItems[i].GetComponent<ConsumableItem>();
            ConsumableItem instanceConsumable = rewardItem.GetComponent<ConsumableItem>();
            
            Debug.Log($"[QuestRewardSystem] Spawned reward {i+1}/{rewardItems.Count}:");
            Debug.Log($"  - Prefab: {rewardItems[i].name} (Type: {(prefabConsumable != null ? prefabConsumable.itemType.ToString() : "NULL")})");
            Debug.Log($"  - Instance: {rewardItem.name} (Type: {(instanceConsumable != null ? instanceConsumable.itemType.ToString() : "NULL")})");
            Debug.Log($"  - Position: {spawnPos}");
            
            SetupRewardItem(rewardItem);
        }
    }

    private Vector3 CalculateSpawnPosition(int index)
    {
        return rewardSpawnPoint.position + new Vector3(
            Random.Range(-horizontalSpread, horizontalSpread),
            verticalOffset + (index * stackHeight),
            Random.Range(-horizontalSpread, horizontalSpread)
        );
    }

    private void SetupRewardItem(GameObject rewardItem)
    {
        // Dam bao co ConsumableItem component
        ConsumableItem consumable = rewardItem.GetComponent<ConsumableItem>();
        if (consumable == null)
        {
            Debug.LogWarning($"[QuestRewardSystem] Reward item '{rewardItem.name}' khong co ConsumableItem component!");
        }
        else
        {
            Debug.Log($"[QuestRewardSystem] Setup item '{rewardItem.name}' with type: {consumable.itemType}");
        }
        
        // Dam bao co tag canPickUp
        if (!rewardItem.CompareTag("canPickUp"))
        {
            rewardItem.tag = "canPickUp";
        }
        
        // Dam bao co InteractableObject
        InteractableObject interactable = rewardItem.GetComponent<InteractableObject>();
        if (interactable == null)
        {
            interactable = rewardItem.AddComponent<InteractableObject>();
            interactable.canBePickedUp = true;
            interactable.ItemName = consumable != null ? 
                (consumable.itemType == ConsumableItem.ItemType.Food ? "Food" : "Water") : "Item";
            Debug.Log($"[QuestRewardSystem] Added InteractableObject to '{rewardItem.name}' with ItemName: {interactable.ItemName}");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (rewardSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rewardSpawnPoint.position, 0.5f);
            Gizmos.DrawLine(rewardSpawnPoint.position, rewardSpawnPoint.position + Vector3.up * 2f);
        }
    }
}
