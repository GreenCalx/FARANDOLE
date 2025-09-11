using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using static Utils;
public class AudioManager : MonoBehaviour, IManager
{
    GameManager GM;
    public GameBGM gameBGM;
    readonly string rankLerpParm = "MiniGameRankLerp";
    public void Init(GameManager iGameManager)
    {
        GM = iGameManager;
    }
    public bool IsReady()
    {
        return true;
    }

    public void LerpRank(float iRankFrom, float iRankTo)
    {
        if (Mathf.Approximately(iRankFrom, iRankTo))
            return;
        LerpRankTsk(iRankFrom, iRankTo);
    }

    async UniTaskVoid LerpRankTsk(float iRankFrom, float iRankTo)
    {
        await LerpBGM(iRankFrom, iRankTo);
    }

    async UniTask LerpBGM(float iRankFrom, float iRankTo)
    {
        float startTime = Time.time;
        float frac = 0f;
        while (frac < 1f)
        {
            frac = (Time.time - startTime) / GameData.GetSettings.BGMRankLerpTimeSec;
            gameBGM.RefreshRankLerp(Utils.Lerp(iRankFrom, iRankTo, frac));
        }
        gameBGM.RefreshRankLerp(iRankTo);
    }

}
