using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
public class UISinglesPanel : UIPanel
{
    public UICompletionBar completionBar;
    public UIMiniGameSelectionGrid miniGameGrid;
    public override void OnNavEnter(CancellationToken iCT)
    {
        LoadPanel();


        //pre
        base.OnNavEnter(iCT);
    }

    public override void OnNavExit(CancellationToken iCT)
    {
        base.OnNavExit(iCT);
        // post

        miniGameGrid.Clear();
    }

    async UniTask LoadPanel()
    {
        List<int> ids_to_load = new List<int>();
        foreach(MiniGameSO MGDesc in GameData.GetMGBank.GameBank)
        { 
            ids_to_load.Add(MGDesc.ID);
        }

        //Dictionary<int, Dictionary<string, object>> data = await CloudSaveManager.LoadSinglesBatch(ids_to_load);

        // Rebuild Singles Data

        // data available, proceed
        completionBar.Setup(0f, (float)ids_to_load.Count);
        miniGameGrid.Setup(completionBar);
        await UniTask.Yield();
        miniGameGrid.ShowAll();
    }
}
