using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject objectiveUI;


    // NEW
    public GameObject interactionUI;

    public void PlayerDied()
    {
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(5f);

        if (objectiveUI != null)
            objectiveUI.SetActive(false);

        PlayerInteract interact = FindFirstObjectByType<PlayerInteract>();
        if (interact != null)
            interact.DisableInteraction();

        // NEW
        if (interactionUI != null)
            interactionUI.SetActive(false);

        Weapon weapon = FindFirstObjectByType<Weapon>();
        if (weapon != null)
            weapon.enabled = false;

        gameOverScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void PlayerWon()
    {
        if (objectiveUI != null)
            objectiveUI.SetActive(false);

        PlayerInteract interact = FindFirstObjectByType<PlayerInteract>();
        if (interact != null)
            interact.DisableInteraction();

        // NEW
        if (interactionUI != null)
            interactionUI.SetActive(false);

        Weapon weapon = FindFirstObjectByType<Weapon>();
        if (weapon != null)
            weapon.enabled = false;

        gameOverScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}