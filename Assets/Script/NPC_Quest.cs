using UnityEngine;
using UnityEngine.UI;

public class NPC_Quest : MonoBehaviour
{
    [Header("Quest Settings")]
    public string questName = "Bat Chuot";
    public string questDescription = "Hay bat 5 con chuot va mang ve khu vuc tap ket!";
    public int requiredRatCount = 5; // So luong chuot can bat
    
    [Header("UI Settings")]
    public GameObject questUI; // Panel hien thi quest
    public Text questTitleText; // Text hien thi ten quest
    public Text questDescriptionText; // Text hien thi mo ta
    public Text questProgressText; // Text hien thi tien do (0/5)
    
    [Header("Collection Area")]
    public Transform collectionArea; // Khu vuc tap ket chuot
    public float collectionRadius = 3f; // Ban kinh khu vuc tap ket
    
    // Tham chieu den PlayerDetector component
    private PlayerDetector playerDetector;
    
    // Properties de truy cap thong tin tu PlayerDetector
    private Transform player => playerDetector != null ? playerDetector.detectedPlayer : null;
    private bool playerInRange => playerDetector != null && playerDetector.isPlayerInRange;
    
    private bool questCompleted = false;
    private int currentRatCount = 0;

    void Start()
    {
        // Lay component PlayerDetector tren cung GameObject
        playerDetector = GetComponent<PlayerDetector>();
        
        if (playerDetector == null)
        {
            Debug.LogError("NPC_Quest can co PlayerDetector component!");
        }
        
        // An UI ban dau
        if (questUI != null)
        {
            questUI.SetActive(false);
        }
        
        UpdateQuestUI();
    }

    void Update()
    {
        // Kiem tra player co trong range khong (su dung PlayerDetector)
        if (playerInRange && !questCompleted)
        {
            ShowQuestUI();
        }
        else
        {
            HideQuestUI();
        }
        
        // Dem so chuot trong khu vuc tap ket
        if (!questCompleted)
        {
            CountRatsInCollectionArea();
        }
    }

    void CountRatsInCollectionArea()
    {
        if (collectionArea == null) return;
        
        // Tim tat ca object co tag "canPickUp"
        GameObject[] allPreys = GameObject.FindGameObjectsWithTag("canPickUp");
        int ratCount = 0;
        
        foreach (GameObject prey in allPreys)
        {
            // Kiem tra xem co phai la chuot chet khong
            AI_Movement_Prey preyScript = prey.GetComponent<AI_Movement_Prey>();
            if (preyScript != null && preyScript.isDead)
            {
                // Kiem tra xem co trong khu vuc tap ket khong
                float distance = Vector3.Distance(collectionArea.position, prey.transform.position);
                if (distance <= collectionRadius)
                {
                    ratCount++;
                }
            }
        }
        
        // Cap nhat so luong
        if (ratCount != currentRatCount)
        {
            currentRatCount = ratCount;
            UpdateQuestUI();
            
            // Kiem tra hoan thanh quest
            if (currentRatCount >= requiredRatCount && !questCompleted)
            {
                CompleteQuest();
            }
        }
    }

    void ShowQuestUI()
    {
        if (questUI != null && !questUI.activeSelf)
        {
            questUI.SetActive(true);
        }
    }

    void HideQuestUI()
    {
        if (questUI != null && questUI.activeSelf && !questCompleted)
        {
            questUI.SetActive(false);
        }
    }

    void UpdateQuestUI()
    {
        if (questTitleText != null)
        {
            questTitleText.text = questName;
        }
        
        if (questDescriptionText != null)
        {
            questDescriptionText.text = questDescription;
        }
        
        if (questProgressText != null)
        {
            questProgressText.text = $"Tien do: {currentRatCount}/{requiredRatCount}";
            
            if (questCompleted)
            {
                questProgressText.text = "Hoan thanh!";
                questProgressText.color = Color.green;
            }
        }
    }

    void CompleteQuest()
    {
        questCompleted = true;
        Debug.Log("Quest hoan thanh!");
        
        UpdateQuestUI();
        
        // Co the them phan thuong o day
        // GiveReward();
    }

    // Ve gizmos de debug
    void OnDrawGizmosSelected()
    {
        // Ve khu vuc tap ket
        if (collectionArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(collectionArea.position, collectionRadius);
        }
    }
}
