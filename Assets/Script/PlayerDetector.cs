using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public string targetTag = "Player";

    // Các thuộc tính public để AI_Movement có thể truy cập
    public Transform detectedPlayer { get; private set; }
    public bool isPlayerInRange { get; private set; }

    // Phát hiện khi người chơi vào vùng trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            detectedPlayer = other.transform;
            isPlayerInRange = true;
        }
    }

    // Phát hiện khi người chơi ở trong vùng trigger
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isPlayerInRange = true;
        }
    }

    // Phát hiện khi người chơi rời khỏi vùng trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            isPlayerInRange = false;
            detectedPlayer = null;
        }
    }
}
