using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class PaperFolderMiniGame : MiniGame
{
    public Transform handle_centerAnchor;
    public float max_size_w, max_size_h;
    public GameObject prefab_foldPaper;
    public Material paperMat;
    FoldPaper inst_foldPaper;
    int paper_w;
    int paper_h;
    
    public override void Init()
    {
        inst_foldPaper = GOBuilder.Create(prefab_foldPaper)
                        .WithName("FoldPaper")
                        .WithParent(handle_centerAnchor)
                        .BuildAs<FoldPaper>();
        inst_foldPaper.Init(this);
        PC.AddSwipeTracker(inst_foldPaper);
    }
    public override void Play()
    {
        IsActiveMiniGame = true;
        IsInPostGame = false;
    }
    public override void Stop()
    {
        IsActiveMiniGame = false;
        IsInPostGame = false;
        
    }
    public override void Win()
    {
        PC.RemoveSwipeTracker(inst_foldPaper);
        IsInPostGame = true;
    }
    public override void Lose()
    {
        IsInPostGame = false;
    }
    public override bool SuccessCheck()
    {
        return false;
    }


}
