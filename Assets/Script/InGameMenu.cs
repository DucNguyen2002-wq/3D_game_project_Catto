using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject menuCanvas;
    private bool isMenuOpen = false;

    void Start()
    {
        // Đảm bảo menu bị ẩn khi bắt đầu game
        menuCanvas.SetActive(false);
        isMenuOpen = false;
        
        // Lock và ẩn chuột khi bắt đầu chơi
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Kiểm tra khi bấm phím ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen; 
        menuCanvas.SetActive(isMenuOpen);
        
        // Toggle timescale
        Time.timeScale = isMenuOpen ? 0f : 1f;
        
        // Toggle cursor lock và visibility
        if (isMenuOpen)
        {
            // Mở menu: unlock và hiện chuột
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Đóng menu: lock và ẩn chuột
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        // Unlock chuột trước khi về main menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void ResumeGame()
    {
        isMenuOpen = false;
        menuCanvas.SetActive(false);
        Time.timeScale = 1f;
        
        // Lock và ẩn chuột khi resume
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
