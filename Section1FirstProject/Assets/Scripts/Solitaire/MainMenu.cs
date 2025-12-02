using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject rulesPanel;
    public GameObject themesPanel;

    // Static so the selected theme persists when we load the Freecell scene
    public static int selectedThemeIndex = 0;

    // --- CORE ---

    public void StartGame()
    {
        SceneManager.LoadScene("Freecell");   // make sure this matches your FreeCell scene name exactly
    }

    public void QuitGame()
    {
        Application.Quit();
        // Note: this does nothing in the editor, only in a built game.
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

    // --- THEMES PANEL ---

    public void ShowThemes()
    {
        if (themesPanel != null) themesPanel.SetActive(true);
    }

    public void HideThemes()
    {
        if (themesPanel != null) themesPanel.SetActive(false);
    }

    // Called by theme buttons to set the current theme index
    public void SelectTheme(int index)
    {
        selectedThemeIndex = index;
        HideThemes();
    }
}