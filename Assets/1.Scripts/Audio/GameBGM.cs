using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class GameBGM : MonoBehaviour
{
    FMODUnity.StudioEventEmitter emitter;
    readonly string rankParm = "MiniGameRank";
    readonly string gameSpeedParm = "GameSpeed";
    readonly string gameStateParm = "GameState";

    void Start()
    {
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        RefreshRank(LoopRank.Z);
    }

    public void RefreshRank(LoopRank iLoopRank)
    {
        Debug.Log("BGM rank refresh : " + (int)iLoopRank);
        emitter.SetParameter(rankParm, (int)iLoopRank);
    }
    public void RefreshSpeed(float iSpeed)
    {
        emitter.SetParameter(gameSpeedParm, iSpeed);
    }

    public void RefreshGameState()
    {
        //emitter.SetParameter(gameStateParm, );
    }

}
