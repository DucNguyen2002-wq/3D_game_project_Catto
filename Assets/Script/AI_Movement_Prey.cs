using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement_Prey : MonoBehaviour
{
    Animator animator;

    public float moveSpeed = 0.2f;
    public float runSpeed = 1.5f; // Tốc độ chạy tùy chỉnh

    Vector3 stopPosition;

    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;

    // Cài đặt cho chạy
    public float runTime = 3f; // Thời gian chạy tùy chỉnh
    private float runCounter;
    public float runCooldown = 5f; // Thời gian cooldown trước khi có thể chạy lại
    private float runCooldownCounter;
    private bool canRun = true;

    // Tham chiếu đến PlayerDetector component
    private PlayerDetector playerDetector;

    // Properties để truy cập thông tin từ PlayerDetector
    private Transform player => playerDetector != null ? playerDetector.detectedPlayer : null;
    private bool playerInRange => playerDetector != null && playerDetector.isPlayerInRange;

    int WalkDirection;

    public bool isWalking;
    public bool isRunning;
    public bool isDead = false; // Trạng thái chết

    void Start()
    {
        animator = GetComponent<Animator>();

        // Lấy component PlayerDetector trên cùng GameObject
        playerDetector = GetComponent<PlayerDetector>();

        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);

        waitCounter = waitTime;
        walkCounter = walkTime;
        runCooldownCounter = 0;

        ChooseDirection();
    }

    void Update()
    {
        // Nếu đã chết thì không làm gì cả
        if (isDead)
        {
            return;
        }

        // Kiểm tra nếu người chơi trong vùng trigger
        if (playerInRange && canRun && !isRunning)
        {
            StartRunning();
        }

        // Cập nhật cooldown
        if (!canRun)
        {
            runCooldownCounter -= Time.deltaTime;
            if (runCooldownCounter <= 0)
            {
                canRun = true;
            }
        }

        // Trạng thái chạy
        if (isRunning)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);

            runCounter -= Time.deltaTime;

            // Chạy theo hướng hiện tại với tốc độ cao hơn
            transform.position += transform.forward * runSpeed * Time.deltaTime;

            if (runCounter <= 0)
            {
                // Kết thúc chạy
                isRunning = false;
                animator.SetBool("isRunning", false);

                // Bắt đầu cooldown
                canRun = false;
                runCooldownCounter = runCooldown;

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

    void StartRunning()
    {
        if (player == null) return;

        // Tính hướng chạy ra xa người chơi
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        directionAwayFromPlayer.y = 0; // Giữ trên mặt phẳng ngang

        if (directionAwayFromPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionAwayFromPlayer);
        }

        // Chuyển sang trạng thái chạy
        isRunning = true;
        isWalking = false;
        runCounter = runTime;
    }

    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);
        isWalking = true;
        walkCounter = walkTime;
    }

    // Hàm được gọi khi prey bị bắt
    public void Die()
    {
        if (isDead) return; // Tránh gọi nhiều lần

        isDead = true;
        isWalking = false;
        isRunning = false;

        // Dừng tất cả animation
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isDead", true);
        }

        // Tắt PlayerDetector nếu có
        if (playerDetector != null)
        {
            playerDetector.enabled = false;
        }

    }
}
