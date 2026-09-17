using System;
using UnityEngine;

public class CloneBody : MonoBehaviour
{
    public event Action OnCloneDestroyed;

    private void OnDestroy()
    {
        OnCloneDestroyed?.Invoke();
    }
}