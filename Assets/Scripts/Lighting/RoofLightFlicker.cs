using UnityEngine;

public class RoofLightFlicker : MonoBehaviour
{
    Renderer[] tiles;

    public float minIntensity = 0.6f;
    public float maxIntensity = 1.2f;

    public float minFlickerTime = 0.05f;
    public float maxFlickerTime = 0.2f;

    float timer;

    void Start()
    {
        tiles = GetComponentsInChildren<Renderer>();
        timer = Random.Range(minFlickerTime, maxFlickerTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            float intensity = Random.value < 0.1f ? 0.05f : Random.Range(minIntensity, maxIntensity);

            foreach (Renderer r in tiles)
            {
                Material mat = r.material;
                mat.SetColor("_EmissionColor", Color.white * intensity);
            }

            timer = Random.Range(minFlickerTime, maxFlickerTime);
        }
    }
}