using UnityEngine;
using UnityEngine.Events;

public class ObjectTarget : MonoBehaviour
{
    public bool DestroyOnHit = true;
    public UnityEvent<ObjectTarget> OnTargetHit;

    public void OnHit()
    {
        OnTargetHit.Invoke(this);
    }
}
