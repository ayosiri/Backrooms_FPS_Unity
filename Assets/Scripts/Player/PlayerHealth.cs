using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float health;

    public Image healthBar;

    public GameEndManager gameEndManager;

    void Start()
    {
        health = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            TakeDamage(Random.Range(5, 10));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            RestoreHealth(Random.Range(5, 10));
        }
    }

    void UpdateHealthUI()
    {
        healthBar.fillAmount = health / maxHealth;
    }

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

    void Die()
    {
        Debug.Log("PLAYER DEAD");

        CameraController cam = GetComponentInChildren<CameraController>();

        if (cam != null)
        {
            cam.enabled = false;
        }

        StartCoroutine(FallBack());

        if (gameEndManager != null)
        {
            gameEndManager.PlayerDied();
        }
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }

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