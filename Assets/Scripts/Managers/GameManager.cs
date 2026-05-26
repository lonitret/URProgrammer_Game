using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool isPaused = false;
    public static bool isGameOver = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject gameOverMenuUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverMenuUI != null) gameOverMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
    }

    public void Resume()
    {
        if (isGameOver) return;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeAllSounds();
        }
    }

    public void Pause()
    {
        if (isGameOver) return;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseAllSounds();
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Time.timeScale = 0f;

        if (gameOverMenuUI != null)
        {
            gameOverMenuUI.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSounds();
            // —юда можно дописать музыку поражени€
            // AudioManager.Instance.PlayMusic(MusicType.GameOverTheme);
        }
    }

    public void RestartDay()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;

        if (AudioManager.Instance != null) AudioManager.Instance.StopAllSounds();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSounds();
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}