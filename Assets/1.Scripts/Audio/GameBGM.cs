using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class GameBGM : MonoBehaviour
{
    FMODUnity.StudioEventEmitter emitter;
    FMOD.Studio.EventDescription emitterEvtDesc;
    FMOD.Studio.EventInstance emitterEvtInst;
    FMOD.ChannelGroup emitterChannelGroup;

    readonly string rankParm = "MiniGameRank";
    readonly string rankLerpParm = "MiniGameRankLerp";
    readonly string gameSpeedParm = "GameSpeed";
    readonly string gameStateParm = "GameState";

    void Start()
    {
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        emitterEvtInst = emitter.EventInstance;
        // system.getEvent(emitter.asset.path, out emitterEvtDesc);
        // evtDesrcription.createInstance(out emitterEvtInst);
        // emitterEvtInst.start();


        // emitterChannelGroup = emitterEvtInst.getChannelGroup(out emitterChannelGroup);
        // system.flushCommands();
        // system.update();

        RefreshRank(LoopRank.Z);
    }

    public void RefreshRank(LoopRank iLoopRank)
    {
        Debug.Log("BGM rank refresh : " + (int)iLoopRank);
        //emitter.SetParameter(rankParm, (int)iLoopRank);
    }

    public void RefreshRankLerp(float iLerpValue)
    {
        emitter.SetParameter(rankLerpParm, iLerpValue);
    }
    public void RefreshSpeed(float iSpeed)
    {
        //emitter.SetParameter(gameSpeedParm, iSpeed);
        // emitterEvtInst.setPitch(iSpeed);
    }

    public void RefreshGameState()
    {
        //emitter.SetParameter(gameStateParm, );
    }

}
