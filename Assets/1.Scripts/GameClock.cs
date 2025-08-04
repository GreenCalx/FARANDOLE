using UnityEngine;

public class GameClock
{
    bool frozen = false;
    public float miniGameStartTime;
    public float miniGameMaxTime;
    public float lastTick;

    public GameClock()
    {
        miniGameMaxTime = GameData.Get.gameSettings.MiniGameTime;
        frozen = true;
    }

    public void Reset()
    {
        frozen = false;
        miniGameStartTime = Time.time;
    }

    public void Tick()
    {
        if (frozen)
            return;
        lastTick = Time.time;
    }

    public void Freeze(bool iState)
    {
        frozen = iState;
    }

    public float GetRemainingTime()
    {
        return Mathf.Clamp(miniGameMaxTime - (lastTick - miniGameStartTime), 0f, miniGameMaxTime);
    }

    public bool MiniGameTimeExpired()
    {
        return (lastTick - miniGameStartTime) > miniGameMaxTime;
    }
}
