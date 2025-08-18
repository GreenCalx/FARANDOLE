using UnityEngine;

public class PaperFolderMiniGame : MiniGame
{
    public GameObject prefab_foldPaper;
    FoldPaper inst_foldPaper;
    
    public override void Init()
    {
        inst_foldPaper = GOBuilder.Create(prefab_foldPaper)
                        .WithName("FoldPaper")
                        .WithParent(transform)
                        .WithRenderer()
                        .BuildAs<FoldPaper>();
        Vector2 paperSize = new Vector2(2f,2f);
        for (int i=0;i<MGM.miniGamesDifficulty;i++)
        {
            paperSize << 2;
        }
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
