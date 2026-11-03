using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Pasos")]
    [SerializeField] private AudioClip[] stepClips;
    [SerializeField] private float stepDistance = 2f;
    [SerializeField] private float volume = 0.5f;

    private CharacterController characterController;
    private AudioSource audioSource;
    private float distanceWalked;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!characterController.isGrounded)
        {
            distanceWalked = 0f;
            return;
        }

        Vector3 velocity = characterController.velocity;
        velocity.y = 0f;
        distanceWalked += velocity.magnitude * Time.deltaTime;

        // Un paso cada tantos metros: corriendo suenan más seguido, y de chico más cortito
        if (distanceWalked >= stepDistance * transform.localScale.x)
        {
            distanceWalked = 0f;
            PlayStep();
        }
    }

    private void PlayStep()
    {
        if (audioSource == null || stepClips == null || stepClips.Length == 0) return;

        AudioClip clip = stepClips[Random.Range(0, stepClips.Length)];
        audioSource.PlayOneShot(clip, volume);
    }
}