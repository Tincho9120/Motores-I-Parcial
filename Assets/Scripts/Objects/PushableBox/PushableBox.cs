using UnityEngine;

public class PushableBox : MonoBehaviour
{
    [SerializeField] private SizeMask sizesThatCanPush = SizeMask.Medium | SizeMask.Large;
    public bool CanBePushedBy(SizeMask size)
    {
        return (sizesThatCanPush & size) != 0;
    }
}