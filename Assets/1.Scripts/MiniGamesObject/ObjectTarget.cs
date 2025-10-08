using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class ObjectTarget : MonoBehaviour
{
    public bool DestroyOnHit = true;
    public UnityEvent<ObjectTarget> OnTargetHit;

    private bool vulnerable = true;

    public void OnHit()
    {
        if (vulnerable)
            OnTargetHit.Invoke(this);
    }

    protected async UniTask IFrames(float time)
    {
        vulnerable = false;
        await UniTask.WaitForSeconds(time);
        vulnerable = true;
    }
}
