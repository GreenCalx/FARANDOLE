using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using static AsyncUtils;
using System.Collections.Generic;
public class TitleAnim : MonoBehaviour
{
    float initImageScale,
            initCellScale,
            initTimeScale,
            initTint,
            initMinDist;
    Color initInnerCellColor;
    Color initOutCellColor;
            
    const string sParmCellScale = "_CellScale";
    const string sParmTimeScale = "_TimeScale";
    const string sParmMinDist = "_MinDist";
    const string sParmInnerCellColor = "_InnerCellColor";
    const string sParmOutCellColor = "_OutCellColor";
    const string sParmTint = "_Tint";
    public Image image;
    Material titleMat;
    CancellationTokenSource cts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titleMat = image.material;
        initCellScale = titleMat.GetFloat(sParmCellScale);
        initTimeScale = titleMat.GetFloat(sParmTimeScale);
        initInnerCellColor = titleMat.GetColor(sParmInnerCellColor);
        initOutCellColor = titleMat.GetColor(sParmOutCellColor);
        initTint = titleMat.GetFloat(sParmTint);
        initMinDist = titleMat.GetFloat(sParmMinDist);
        initImageScale = image.rectTransform.localScale.x;

    }

    void OnDisable()
    {
        if (cts!=null)
            cts.Cancel();
        titleMat.SetFloat(sParmCellScale, initCellScale);
        titleMat.SetFloat(sParmMinDist, initMinDist);
    }

    public async UniTaskVoid ExitAnim()
    {
        cts = new CancellationTokenSource();
        List<UniTask> tasks = new List<UniTask>();

        tasks.Add(  LerpTask(initImageScale, initImageScale * 4f,
                    GameData.GetSettings.titleScreenFadeoutTime,
                    (f) => image.rectTransform.localScale = new Vector3(f, f, f),
                    cts.Token));

        tasks.Add(  LerpTask(initMinDist, 0f,
                    GameData.GetSettings.titleScreenFadeoutTime,
                    (f) => titleMat.SetFloat(sParmMinDist, f),
                    cts.Token));
        
        await UniTask.WhenAll(tasks);
    }
}
