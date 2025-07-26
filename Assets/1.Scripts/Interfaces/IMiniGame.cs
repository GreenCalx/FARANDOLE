using UnityEngine;

public interface IMiniGame
{
    public void Init();
    public void Play();
    public void Stop();
    public bool SuccessCheck();
}
