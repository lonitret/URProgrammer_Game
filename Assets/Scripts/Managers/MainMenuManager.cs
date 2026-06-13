using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        AudioManager.Instance?.PlaySFX(SoundType.UIClick);
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        AudioManager.Instance?.PlaySFX(SoundType.UIClick);
        Debug.Log("Игра закрывается...");
        Application.Quit();
    }
}