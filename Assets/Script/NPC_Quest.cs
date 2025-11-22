using UnityEngine;
using UnityEngine.UI;

public class NPC_Quest : MonoBehaviour
{
    [Header("Quest Info")]
    public string questName = "Bat Chuot";
    public string questDescription = "Hay bat {0} con chuot va mang ve khu vuc tap ket!";
    public int requiredCount = 5;
    
    [Header("Quest Components")]
    public QuestSpawnSystem spawnSystem;
    public QuestObjectiveTracker objectiveTracker;
    public QuestRewardSystem rewardSystem;
    
    [Header("UI Settings")]
    public GameObject questUI;
    public Text questTitleText;
    public Text questDescriptionText;
    public Text questProgressText;
    
    [Header("Quest Behavior")]
    public bool autoSpawnOnStart = true;
    public KeyCode startQuestKey = KeyCode.Q;
    
    private PlayerDetector playerDetector;
    private bool playerInRange => playerDetector != null && playerDetector.isPlayerInRange;
    
    private bool questActive = false;
    private bool questCompleted = false;

    void Start()
    {
        InitializeComponents();
        SetupEventListeners();
        
        if (questUI != null)
        {
            questUI.SetActive(false);
        }
        
        UpdateQuestUI();
    }

    void Update()
    {
        HandlePlayerInteraction();
        
        if (questActive && !questCompleted)
        {
            TrackObjectives();
        }
    }

    private void InitializeComponents()
    {
        playerDetector = GetComponent<PlayerDetector>();
        if (playerDetector == null)
        {
            Debug.LogError("[NPC_Quest] Can co PlayerDetector component!");
        }
        
        // Auto-find components neu chua gan
        if (spawnSystem == null)
        {
            spawnSystem = GetComponent<QuestSpawnSystem>();
        }
        
        if (objectiveTracker == null)
        {
            objectiveTracker = GetComponent<QuestObjectiveTracker>();
        }
        
        if (rewardSystem == null)
        {
            rewardSystem = GetComponent<QuestRewardSystem>();
        }
        
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (spawnSystem == null)
        {
            Debug.LogWarning("[NPC_Quest] QuestSpawnSystem chua duoc gan!");
        }
        
        if (objectiveTracker == null)
        {
            Debug.LogWarning("[NPC_Quest] QuestObjectiveTracker chua duoc gan!");
        }
        
        if (rewardSystem == null)
        {
            Debug.LogWarning("[NPC_Quest] QuestRewardSystem chua duoc gan!");
        }
    }

    private void SetupEventListeners()
    {
        if (objectiveTracker != null)
        {
            objectiveTracker.OnProgressChanged += OnObjectiveProgressChanged;
            objectiveTracker.OnCompleted += OnObjectiveCompleted;
        }
    }

    private void HandlePlayerInteraction()
    {
        if (playerInRange && !questCompleted)
        {
            ShowQuestUI();
            
            if (!questActive && Input.GetKeyDown(startQuestKey))
            {
                StartQuest();
            }
        }
        else
        {
            HideQuestUI();
        }
    }

    private void TrackObjectives()
    {
        if (objectiveTracker != null)
        {
            objectiveTracker.CountObjectsInArea(requiredCount);
        }
    }

    private void StartQuest()
    {
        questActive = true;
        Debug.Log($"[NPC_Quest] Quest '{questName}' đã bắt đầu!");
        
        if (autoSpawnOnStart && spawnSystem != null)
        {
            spawnSystem.SpawnObjects(requiredCount);
        }
        
        UpdateQuestUI();
    }

    private void OnObjectiveProgressChanged(int current, int required)
    {
        UpdateQuestUI();
    }

    private void OnObjectiveCompleted()
    {
        if (!questCompleted)
        {
            CompleteQuest();
        }
    }

    private void CompleteQuest()
    {
        questCompleted = true;
        Debug.Log($"[NPC_Quest] Quest '{questName}' hoàn thành!");
        
        UpdateQuestUI();
        
        if (rewardSystem != null)
        {
            rewardSystem.GiveReward();
        }
    }

    private void ShowQuestUI()
    {
        if (questUI != null && !questUI.activeSelf)
        {
            questUI.SetActive(true);
        }
    }

    private void HideQuestUI()
    {
        if (questUI != null && questUI.activeSelf && !questCompleted)
        {
            questUI.SetActive(false);
        }
    }

    private void UpdateQuestUI()
    {
        if (questTitleText != null)
        {
            questTitleText.text = questName;
        }
        
        if (questDescriptionText != null)
        {
            questDescriptionText.text = string.Format(questDescription, requiredCount);
            
            if (!questActive)
            {
                questDescriptionText.text += $"\n\n[{startQuestKey}] Nhận nhiệm vụ";
            }
        }
        
        if (questProgressText != null)
        {
            if (!questActive)
            {
                questProgressText.text = "Chưa bắt đầu";
                questProgressText.color = Color.white;
            }
            else if (questCompleted)
            {
                questProgressText.text = "Hoàn thành!";
                questProgressText.color = Color.green;
            }
            else
            {
                int current = objectiveTracker != null ? objectiveTracker.GetCurrentCount() : 0;
                questProgressText.text = $"Tiến độ: {current}/{requiredCount}";
                questProgressText.color = Color.yellow;
            }
        }
    }

    //public void ResetQuest()
    //{
    //    questActive = false;
    //    questCompleted = false;
        
    //    if (spawnSystem != null)
    //    {
    //        spawnSystem.ClearSpawnedObjects();
    //    }
        
    //    if (objectiveTracker != null)
    //    {
    //        objectiveTracker.ResetProgress();
    //    }
        
    //    UpdateQuestUI();
    //    Debug.Log($"[NPC_Quest] Quest '{questName}' da duoc reset!");
    //}

    //void OnDestroy()
    //{
    //    // Cleanup event listeners
    //    if (objectiveTracker != null)
    //    {
    //        objectiveTracker.OnProgressChanged -= OnObjectiveProgressChanged;
    //        objectiveTracker.OnCompleted -= OnObjectiveCompleted;
    //    }
    //}
}
