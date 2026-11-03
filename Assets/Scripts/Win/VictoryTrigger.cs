using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] private GameObject victoryText;

    [Header("Feedback")]
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private ParticleSystem victoryParticles;

    private AudioSource audioSource;
    private bool hasWon = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!hasWon) return;

        // el PlayerInput quedo apagado, asi que la R se lee directo del teclado
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasWon) return;
        if (!other.CompareTag("Player")) return;

        hasWon = true;
        StopPlayer(other.gameObject);
        victoryText.SetActive(true);

        if (audioSource != null && victoryClip != null)
        {
            audioSource.PlayOneShot(victoryClip);
        }

        if (victoryParticles != null)
        {
            victoryParticles.Play();
        }

        // suelta el mouse para poder leer el cartel
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Apagar solo el PlayerInput no alcanza: el ultimo input queda pegado y el jugador sigue andando
    private void StopPlayer(GameObject player)
    {
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        CameraController cameraController = player.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }

        PlayerFootsteps footsteps = player.GetComponent<PlayerFootsteps>();
        if (footsteps != null)
        {
            footsteps.enabled = false;
        }
    }
}