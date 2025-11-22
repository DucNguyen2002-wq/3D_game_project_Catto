using UnityEngine;
using System.Collections.Generic;

public class QuestSpawnSystem : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject spawnPrefab;
    public Transform[] spawnPoints;
    
    [Header("Spawn Randomization")]
    public float randomOffsetRange = 1f;
    public bool enableRandomRotation = false;
    
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void SpawnObjects(int count)
    {
        if (!ValidateSpawnSettings())
        {
            return;
        }
        
        ClearSpawnedObjects();
        
        Debug.Log($"[QuestSpawnSystem] Dang spawn {count} objects...");
        
        for (int i = 0; i < count; i++)
        {
            SpawnSingleObject(i, count);
        }
    }

    private bool ValidateSpawnSettings()
    {
        if (spawnPrefab == null)
        {
            Debug.LogError("[QuestSpawnSystem] Spawn Prefab chua duoc gan!");
            return false;
        }
        
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[QuestSpawnSystem] Chua co spawn points!");
            return false;
        }
        
        return true;
    }

    private void SpawnSingleObject(int index, int totalCount)
    {
        Transform spawnPoint = GetRandomSpawnPoint();
        Vector3 spawnPos = CalculateSpawnPosition(spawnPoint);
        Quaternion spawnRot = CalculateSpawnRotation(spawnPoint);
        
        GameObject spawnedObject = Instantiate(spawnPrefab, spawnPos, spawnRot);
        spawnedObjects.Add(spawnedObject);
        
        Debug.Log($"[QuestSpawnSystem] Spawned object {index+1}/{totalCount} tai {spawnPoint.name}");
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private Vector3 CalculateSpawnPosition(Transform spawnPoint)
    {
        return spawnPoint.position + new Vector3(
            Random.Range(-randomOffsetRange, randomOffsetRange),
            0f,
            Random.Range(-randomOffsetRange, randomOffsetRange)
        );
    }

    private Quaternion CalculateSpawnRotation(Transform spawnPoint)
    {
        if (enableRandomRotation)
        {
            return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
        return spawnPoint.rotation;
    }

    public void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
    }

    public int GetSpawnedCount()
    {
        // Dem so object con song
        int count = 0;
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                count++;
            }
        }
        return count;
    }

    public List<GameObject> GetSpawnedObjects()
    {
        return new List<GameObject>(spawnedObjects);
    }

    void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                    Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
                    
                    // Ve khu vuc random offset
                    Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
                    Gizmos.DrawWireCube(spawnPoint.position, new Vector3(randomOffsetRange * 2f, 0.1f, randomOffsetRange * 2f));
                    Gizmos.color = Color.red;
                }
            }
        }
    }
}
