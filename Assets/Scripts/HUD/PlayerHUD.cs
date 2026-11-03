using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Fuentes de datos")]
    [SerializeField] private PlayerSize playerSize;
    [SerializeField] private PlayerClone playerClone;
    [SerializeField] private PlayerInteractor playerInteractor;

    [Header("Indicador de tamaño")]
    [SerializeField] private Image smallIcon;
    [SerializeField] private Image mediumIcon;
    [SerializeField] private Image largeIcon;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Textos")]
    [SerializeField] private TMP_Text cloneText;
    [SerializeField] private TMP_Text promptText;

    private void OnEnable()
    {
        playerSize.OnSizeChanged += HandleSizeChanged;
        playerClone.OnCloneCountChanged += HandleCloneCountChanged;
        playerInteractor.OnPromptChanged += HandlePromptChanged;
    }

    private void OnDisable()
    {
        playerSize.OnSizeChanged -= HandleSizeChanged;
        playerClone.OnCloneCountChanged -= HandleCloneCountChanged;
        playerInteractor.OnPromptChanged -= HandlePromptChanged;
    }

    private void Start()
    {
        // Estado inicial, antes de que pase cualquier evento
        HandleSizeChanged(playerSize.CurrentSizeMask);
        HandleCloneCountChanged(playerClone.CloneCount, playerClone.MaxClones);
        HandlePromptChanged(null);
    }

    private void HandleSizeChanged(SizeMask size)
    {
        smallIcon.color = size == SizeMask.Small ? activeColor : inactiveColor;
        mediumIcon.color = size == SizeMask.Medium ? activeColor : inactiveColor;
        largeIcon.color = size == SizeMask.Large ? activeColor : inactiveColor;
    }

    private void HandleCloneCountChanged(int count, int max)
    {
        cloneText.text = $"{count}/{max}";
    }

    private void HandlePromptChanged(string prompt)
    {
        promptText.text = prompt ?? string.Empty;
    }
}