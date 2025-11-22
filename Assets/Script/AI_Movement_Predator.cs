using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement_Predator : MonoBehaviour
{
    Animator animator;

    public float moveSpeed = 0.2f;
    public float chaseSpeed = 2.0f; // Tốc độ đuổi theo tùy chỉnh

    Vector3 stopPosition;

    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;

    // Cài đặt cho đuổi theo
    public float chaseTime = 5f; // Thời gian đuổi theo tùy chỉnh
    private float chaseCounter;
    public float chaseCooldown = 3f; // Thời gian cooldown trước khi có thể đuổi lại
    private float chaseCooldownCounter;
    private bool canChase = true;

    // Tham chiếu đến PlayerDetector component
    private PlayerDetector playerDetector;

    // Properties để truy cập thông tin từ PlayerDetector
    private Transform player => playerDetector != null ? playerDetector.detectedPlayer : null;
    private bool playerInRange => playerDetector != null && playerDetector.isPlayerInRange;

    int WalkDirection;

    public bool isWalking;
    public bool isRunning;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Lấy component PlayerDetector trên cùng GameObject
        playerDetector = GetComponent<PlayerDetector>();

        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);

        waitCounter = waitTime;
        walkCounter = walkTime;
        chaseCooldownCounter = 0;

        ChooseDirection();
    }

    void Update()
    {
        // Kiểm tra nếu người chơi trong vùng trigger
        if (playerInRange && canChase && !isRunning)
        {
            StartChasing();
        }

        // Cập nhật cooldown
        if (!canChase)
        {
            chaseCooldownCounter -= Time.deltaTime;
            if (chaseCooldownCounter <= 0)
            {
                canChase = true;
            }
        }

        // Trạng thái đuổi theo
        if (isRunning)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);

            chaseCounter -= Time.deltaTime;

            // Luôn quay mặt về phía người chơi
            if (player != null)
            {
                Vector3 directionToPlayer = player.position - transform.position;
                directionToPlayer.y = 0; // Giữ trên mặt phẳng ngang

                if (directionToPlayer != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(directionToPlayer);
                }

                // Di chuyển về phía người chơi
                transform.position += transform.forward * chaseSpeed * Time.deltaTime;
            }

            if (chaseCounter <= 0)
            {
                // Kết thúc đuổi theo
                isRunning = false;
                animator.SetBool("isRunning", false);

                // Bắt đầu cooldown
                canChase = false;
                chaseCooldownCounter = chaseCooldown;

                // Chuyển sang trạng thái chờ
                isWalking = false;
                waitCounter = waitTime;
            }
        }
        // Trạng thái đi bộ
        else if (isWalking)
        {
            animator.SetBool("isWalking", true);

            walkCounter -= Time.deltaTime;

            switch (WalkDirection)
            {
                case 0:
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 1:
                    transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 2:
                    transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 3:
                    transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
            }

            if (walkCounter <= 0)
            {
                stopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                isWalking = false;
                transform.position = stopPosition;
                animator.SetBool("isWalking", false);
                waitCounter = waitTime;
            }
        }
        // Trạng thái chờ
        else
        {
            waitCounter -= Time.deltaTime;

            if (waitCounter <= 0)
            {
                ChooseDirection();
            }
        }
    }

    void StartChasing()
    {
        if (player == null) return;

        // Tính hướng đuổi theo người chơi (ngược lại với flee)
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Giữ trên mặt phẳng ngang

        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        // Chuyển sang trạng thái đuổi theo
        isRunning = true;
        isWalking = false;
        chaseCounter = chaseTime;
    }

    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);
        isWalking = true;
        walkCounter = walkTime;
    }
}
