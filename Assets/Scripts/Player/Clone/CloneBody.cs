using System;
using UnityEngine;

public class CloneBody : MonoBehaviour
{
    public event Action OnCloneDestroyed;

    private void OnDestroy()
    {
        Debug.Log($"[CloneBody] {name} destruido, invocando evento");
        OnCloneDestroyed?.Invoke();
    }
}