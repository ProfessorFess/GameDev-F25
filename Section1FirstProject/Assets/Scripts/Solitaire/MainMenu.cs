using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Optional panels you can hook up in the Inspector
    public GameObject rulesPanel;

    // --- CORE ---

    public void StartGame()
    {
        SceneManager.LoadScene("Freecell");   // exact scene name
    }

    public void QuitGame()
    {
        Application.Quit();
        // (won't do anything in editor, only in build)
    }

    // --- RULES PANEL ---

    public void ShowRules()
    {
        if (rulesPanel != null) rulesPanel.SetActive(true);
    }

    public void HideRules()
    {
        if (rulesPanel != null) rulesPanel.SetActive(false);
    }

}