using System;
using UnityEngine;

public class CloneBody : MonoBehaviour
{
    public event Action OnCloneDestroyed;
    public SizeMask Size { get; set; }
    private void OnDestroy()
    {
        OnCloneDestroyed?.Invoke();
    }
}