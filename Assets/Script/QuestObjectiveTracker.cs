using UnityEngine;

public class QuestObjectiveTracker : MonoBehaviour
{
    [Header("Objective Settings")]
    public Transform collectionArea;
    public float collectionRadius = 3f;
    public string targetTag = "canPickUp";
    
    private int currentCount = 0;
    
    public delegate void ObjectiveProgressChanged(int current, int required);
    public event ObjectiveProgressChanged OnProgressChanged;
    
    public delegate void ObjectiveCompleted();
    public event ObjectiveCompleted OnCompleted;

    public int CountObjectsInArea(int requiredCount)
    {
        if (collectionArea == null)
        {
            Debug.LogWarning("[QuestObjectiveTracker] Collection Area chua duoc gan!");
            return 0;
        }
        
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(targetTag);
        int count = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (IsObjectValid(obj) && IsObjectInArea(obj))
            {
                count++;
            }
        }
        
        // Cap nhat tien do
        if (count != currentCount)
        {
            currentCount = count;
            OnProgressChanged?.Invoke(currentCount, requiredCount);
            
            // Kiem tra hoan thanh
            if (currentCount >= requiredCount)
            {
                OnCompleted?.Invoke();
            }
        }
        
        return count;
    }

    private bool IsObjectValid(GameObject obj)
    {
        // Kiem tra xem co phai la prey chet khong
        AI_Movement_Prey preyScript = obj.GetComponent<AI_Movement_Prey>();
        if (preyScript != null)
        {
            return preyScript.isDead;
        }
        
        // Neu khong phai prey thi mac dinh la valid
        return true;
    }

    private bool IsObjectInArea(GameObject obj)
    {
        float distance = Vector3.Distance(collectionArea.position, obj.transform.position);
        return distance <= collectionRadius;
    }

    public int GetCurrentCount()
    {
        return currentCount;
    }

    public void ResetProgress()
    {
        currentCount = 0;
    }

    void OnDrawGizmosSelected()
    {
        if (collectionArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(collectionArea.position, collectionRadius);
            
            // Ve label
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(collectionArea.position + Vector3.up * 2f, "Collection Area");
            #endif
        }
    }
}
