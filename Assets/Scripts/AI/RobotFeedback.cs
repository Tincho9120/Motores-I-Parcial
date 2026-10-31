using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RobotFeedback : MonoBehaviour
{
    [Header("Luces")]
    [SerializeField] private Light patrolLight;   // verde
    [SerializeField] private Light alertLight;    // roja
    [SerializeField] private float warningBlinkRate = 4f; // parpadeos por segundo

    [Header("Sonidos")]
    [SerializeField] private AudioClip alertClip;
    [SerializeField] private AudioClip destroyClip;

    [Header("Partículas")]
    [SerializeField] private ParticleSystem destroyParticlesPrefab;
    [SerializeField] private float particlesLifetime = 3f;

    private AudioSource audioSource;
    private bool warningActive;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Durante la advertencia, la luz roja parpadea
        if (warningActive && alertLight != null)
        {
            alertLight.enabled = Mathf.FloorToInt(Time.time * warningBlinkRate * 2f) % 2 == 0;
        }
    }

    public void ShowPatrol()
    {
        warningActive = false;
        SetLights(true, false);
    }

    public void ShowChase()
    {
        warningActive = false;
        SetLights(false, true);
    }

    public void ShowWarning()
    {
        warningActive = true;
        SetLights(false, true);
    }

    public void PlayAlert()
    {
        PlayClip(alertClip);
    }

    public void PlayDestroyEffects(Vector3 position)
    {
        PlayClip(destroyClip);

        if (destroyParticlesPrefab != null)
        {
            ParticleSystem fx = Instantiate(destroyParticlesPrefab, position, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, particlesLifetime);
        }
    }

    private void SetLights(bool patrol, bool alert)
    {
        if (patrolLight != null) patrolLight.enabled = patrol;
        if (alertLight != null) alertLight.enabled = alert;
    }

    private void PlayClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}