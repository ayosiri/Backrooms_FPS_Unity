using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages player health, damage handling, UI updates, and death behavior
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float health;

    public Image healthBar;

    public GameEndManager gameEndManager;

    void Start()
    {
        // Initialize health and update UI
        health = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        // Debug inputs for testing damage and healing
        if (Input.GetKeyDown(KeyCode.N))
        {
            TakeDamage(Random.Range(5, 10));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            RestoreHealth(Random.Range(5, 10));
        }
    }

    // Updates the health bar fill amount based on current health
    void UpdateHealthUI()
    {
        healthBar.fillAmount = health / maxHealth;
    }

    // Applies damage to the player and checks for death
    public void TakeDamage(float damage)
    {
        if (health <= 0) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        if (health <= 0)
        {
            Die();
        }
    }

    // Handles player death behavior including disabling camera and triggering game over
    void Die()
    {
        Debug.Log("PLAYER DEAD");

        // Disable camera control
        CameraController cam = GetComponentInChildren<CameraController>();
        if (cam != null)
        {
            cam.enabled = false;
        }

        // Trigger camera fall-back animation
        StartCoroutine(FallBack());

        // Notify game manager of death
        if (gameEndManager != null)
        {
            gameEndManager.PlayerDied();
        }
    }

    // Restores player health and updates UI
    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }

    // Simulates player falling backward by rotating the camera
    IEnumerator FallBack()
    {
        Transform cam = Camera.main.transform;

        Quaternion startRot = cam.rotation;
        Quaternion endRot = Quaternion.Euler(80f, cam.eulerAngles.y, 0);

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            cam.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }
}
