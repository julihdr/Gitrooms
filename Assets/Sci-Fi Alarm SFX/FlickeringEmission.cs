using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FlickeringEmission : MonoBehaviour
{
    public AudioClip alarmClip;  // Reference to the AudioClip of the alarm sound
    public Color emissionColor = Color.red;  // Emission color to flicker
    public float intensity1 = 0.5f; // First intensity value
    public float intensity2 = 2.0f; // Second intensity value
    public float flickerSpeed = 0.1f; // Speed of the flickering effect

    private Renderer renderer;
    private Material material;
    private float timeSinceLastFlicker = 0f;
    private float alarmLength = 0f;
    private float alarmTimer = 0f;
    private bool useFirstIntensity = true;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        material = renderer.material;

        if (alarmClip == null)
        {
            Debug.LogError("No AudioClip assigned to FlickeringEmission script.");
        }
        else
        {
            alarmLength = alarmClip.length;
        }

        // Enable emission keyword
        material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (alarmClip != null)
        {
            alarmTimer += Time.deltaTime;

            if (alarmTimer >= alarmLength)
            {
                alarmTimer = 0f;
            }

            timeSinceLastFlicker += Time.deltaTime;

            if (timeSinceLastFlicker >= flickerSpeed)
            {
                Flicker();
                timeSinceLastFlicker = 0f;
            }
        }
    }

    void Flicker()
    {
        float intensity = useFirstIntensity ? intensity1 : intensity2;
        material.SetColor("_EmissionColor", emissionColor * intensity);
        useFirstIntensity = !useFirstIntensity; // Toggle between the two intensity values
    }
}
