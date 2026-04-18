using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manages end-of-game states including player death, victory, and scene transitions
public class GameEndManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject objectiveUI;

    public GameObject interactionUI;

    // Initiates the death sequence with a delay before showing the end screen
    public void PlayerDied()
    {
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // Delay allows death animation or effects to complete
        yield return new WaitForSeconds(5f);

        // Disable gameplay UI elements
        if (objectiveUI != null)
            objectiveUI.SetActive(false);

        PlayerInteract interact = FindFirstObjectByType<PlayerInteract>();
        if (interact != null)
            interact.DisableInteraction();

        if (interactionUI != null)
            interactionUI.SetActive(false);

        // Disable player combat functionality
        Weapon weapon = FindFirstObjectByType<Weapon>();
        if (weapon != null)
            weapon.enabled = false;

        // Display end screen
        gameOverScreen.SetActive(true);

        // Restore cursor for menu interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause game and audio
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    // Handles win condition and transitions to end screen
    public void PlayerWon()
    {
        // Disable gameplay UI elements
        if (objectiveUI != null)
            objectiveUI.SetActive(false);

        PlayerInteract interact = FindFirstObjectByType<PlayerInteract>();
        if (interact != null)
            interact.DisableInteraction();

        if (interactionUI != null)
            interactionUI.SetActive(false);

        // Disable player combat functionality
        Weapon weapon = FindFirstObjectByType<Weapon>();
        if (weapon != null)
            weapon.enabled = false;

        // Display end screen
        gameOverScreen.SetActive(true);

        // Restore cursor for menu interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause game and audio
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    // Reloads the current scene and restores gameplay state
    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Exits the application (handles both editor and build environments)
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
