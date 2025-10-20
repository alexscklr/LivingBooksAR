using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject scanningUI;
    public GameObject endScreen;

    public void ShowStartMenu(bool show)
    {
        if (startMenu)
            startMenu.SetActive(show);
    }

    public void ShowPauseMenu(bool show)
    {
        if (pauseMenu)
            pauseMenu.SetActive(show);
    }

    public void ShowScanningUI(bool show)
    {
        if (scanningUI)
            scanningUI.SetActive(show);
    }

    public void ShowEndScreen()
    {
        if (endScreen)
            endScreen.SetActive(true);
    }

    // Button events
    public void OnStartButtonPressed()
    {
        GameManager.Instance.OnStartButtonPressed();
    }

    public void OnPauseButtonPressed()
    {
        GameManager.Instance.PauseGame();
    }

    public void OnResumeButtonPressed()
    {
        GameManager.Instance.ResumeGame();
    }

    public void OnQuitButtonPressed()
    {
        GameManager.Instance.EndGame();
    }
}
