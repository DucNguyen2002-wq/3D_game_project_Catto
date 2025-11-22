using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; set; }

    [Header("Hunger System")]
    public float maxHunger = 100f;
    public float currentHunger;
    public float hungerDecayIdle = 2f;
    public float hungerDecayWalking = 5f;
    public float hungerDecayRunning = 10f;
    
    [Header("Thirst System")]
    public float maxThirst = 100f;
    public float currentThirst;
    public float thirstDecayIdle = 3f;
    public float thirstDecayWalking = 8f;
    public float thirstDecayRunning = 15f;
    
    [Header("Player Movement Reference")]
    public PlayerMovement playerMovement;
    
    [Header("UI References")]
    public Slider hungerBar;
    public Slider thirstBar;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        
        UpdateUI();
    }

    void Update()
    {
        DecayStats();
        UpdateUI();
    }

    void DecayStats()
    {
        float hungerDecay = GetHungerDecayRate();
        float thirstDecay = GetThirstDecayRate();
        
        if (currentHunger > 0)
        {
            currentHunger -= hungerDecay * Time.deltaTime / 60f;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        }
        
        if (currentThirst > 0)
        {
            currentThirst -= thirstDecay * Time.deltaTime / 60f;
            currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
        }
    }

    float GetHungerDecayRate()
    {
        if (playerMovement == null || playerMovement.animator == null)
            return hungerDecayIdle;
        
        bool isRunning = playerMovement.animator.GetBool("isRunning");
        bool isWalking = playerMovement.animator.GetBool("isWalking");
        
        if (isRunning)
            return hungerDecayRunning;
        else if (isWalking)
            return hungerDecayWalking;
        else
            return hungerDecayIdle;
    }

    float GetThirstDecayRate()
    {
        if (playerMovement == null || playerMovement.animator == null)
            return thirstDecayIdle;
        
        bool isRunning = playerMovement.animator.GetBool("isRunning");
        bool isWalking = playerMovement.animator.GetBool("isWalking");
        
        if (isRunning)
            return thirstDecayRunning;
        else if (isWalking)
            return thirstDecayWalking;
        else
            return thirstDecayIdle;
    }

    void UpdateUI()
    {
        if (hungerBar != null)
        {
            hungerBar.value = currentHunger / maxHunger;
            
            Image fillImage = hungerBar.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float percent = currentHunger / maxHunger;
                if (percent <= 0.2f)
                    fillImage.color = Color.red;
                else if (percent <= 0.5f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = new Color(1f, 0.6f, 0f);
            }
        }
        
        if (thirstBar != null)
        {
            thirstBar.value = currentThirst / maxThirst;
            
            Image fillImage = thirstBar.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float percent = currentThirst / maxThirst;
                if (percent <= 0.2f)
                    fillImage.color = new Color(1f, 0.3f, 0.3f);
                else if (percent <= 0.5f)
                    fillImage.color = new Color(0.3f, 0.7f, 1f);
                else
                    fillImage.color = new Color(0.2f, 0.5f, 1f);
            }
        }
    }

    public void AddHunger(float amount)
    {
        float oldValue = currentHunger;
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        
        Debug.Log($"[PlayerStats] Hunger: {oldValue:F1} -> {currentHunger:F1} (+{amount})");
        
        UpdateUI();
    }

    public void AddThirst(float amount)
    {
        float oldValue = currentThirst;
        currentThirst += amount;
        currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
        
        Debug.Log($"[PlayerStats] Thirst: {oldValue:F1} -> {currentThirst:F1} (+{amount})");
        
        UpdateUI();
    }

    public bool IsHungry()
    {
        return currentHunger < maxHunger * 0.3f;
    }

    public bool IsThirsty()
    {
        return currentThirst < maxThirst * 0.3f;
    }

    public bool IsCritical()
    {
        return currentHunger <= 0 || currentThirst <= 0;
    }

    //void OnGUI()
    //{
    //    if (!showDebugInfo) return;
        
    //    if (playerMovement != null && playerMovement.animator != null)
    //    {
    //        bool isRunning = playerMovement.animator.GetBool("isRunning");
    //        bool isWalking = playerMovement.animator.GetBool("isWalking");

    //        string state = isRunning ? "RUNNING" : isWalking ? "WALKING" : "IDLE";
    //        float hungerRate = GetHungerDecayRate();
    //        float thirstRate = GetThirstDecayRate();

    //        GUI.Box(new Rect(10, 10, 350, 110), "Player Stats");
    //        GUI.Label(new Rect(20, 35, 330, 20), $"State: {state}");
    //        GUI.Label(new Rect(20, 55, 330, 20), $"Hunger: {currentHunger:F1}/{maxHunger} (-{hungerRate}/min)");
    //        GUI.Label(new Rect(20, 75, 330, 20), $"Thirst: {currentThirst:F1}/{maxThirst} (-{thirstRate}/min)");
    //        GUI.Label(new Rect(20, 95, 330, 20), $"Instance: {(Instance != null ? "OK" : "NULL")}");
    //    }
    //    else
    //    {
    //        GUI.Box(new Rect(10, 10, 350, 80), "Player Stats");
    //        GUI.Label(new Rect(20, 35, 330, 20), $"Hunger: {currentHunger:F1}/{maxHunger}");
    //        GUI.Label(new Rect(20, 55, 330, 20), $"Thirst: {currentThirst:F1}/{maxThirst}");
    //    }
    //}
}
