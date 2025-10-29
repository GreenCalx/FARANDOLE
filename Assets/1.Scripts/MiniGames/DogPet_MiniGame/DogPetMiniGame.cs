using UnityEngine;
using TMPro;

public class DogPetMiniGame : MiniGame, IDogMod, IRegularMod
{
    [Header("DogPetMiniGame")]
    public GameObject prefab_dogHead;
    DogHead inst_dogHead;
    int pettings = 0;
    int targetPetting = 10;

    public int[] difficultyTapForces;
    public bool[] SpringAnchorDrag;
    public float[] SpringAnchorDragStrength;
    private int tapForce;
    public int bounceOnRandomTreshold;
    public TextMeshProUGUI UIworld_petCounter;
    public override void Init()
    {
        inst_dogHead = GOBuilder.Create(prefab_dogHead)
                        .WithName("DogHead")
                        .WithPosition(Vector3.zero)
                        .WithParent(transform)
                        .BuildAs<DogHead>();
        inst_dogHead.Init();
        //inst_dogHead.InitBounds(MGM.PG.bounds);
    }
    public override void Reset()
    {
        // inst_dogHead = GOBuilder.Create(prefab_dogHead)
        //                 .WithName("DogHead")
        //                 .WithPosition(Vector3.zero)
        //                 .WithParent(transform)
        //                 .BuildAs<DogHead>();
        // inst_dogHead.Init();
        inst_dogHead.Reset();

        pettings = 0;
        tapForce = difficultyTapForces[MGM.miniGamesDifficulty - 1];
        if (MGM.miniGamesDifficulty >= bounceOnRandomTreshold)
        {
            inst_dogHead.bounceRandomOnTap = true;
        }
        else
            inst_dogHead.bounceRandomOnTap = false;
        UpdateMGUI();
        PC.AddTapTracker(inst_dogHead);

        inst_dogHead.SpringAnchorDrag = SpringAnchorDrag[MGM.miniGamesDifficulty - 1];
        inst_dogHead.m_SpringAnchorDragStrength = SpringAnchorDragStrength[MGM.miniGamesDifficulty - 1];
        inst_dogHead.tapCB.AddListener(RegisterPetting);
    }
    public override void Play()
    {
        IsActiveMiniGame = true;
    }
    public override void Stop()
    {
        inst_dogHead.StopAnim();
        //inst_dogHead.Reset();
        inst_dogHead.tapCB.RemoveListener(RegisterPetting);
        PC.RemoveTapTracker(inst_dogHead);
        IsActiveMiniGame = false;
    }

    void OnDestroy()
    {
        Destroy(inst_dogHead.gameObject);
    }
    public override void Win()
    {
        PC.RemoveTapTracker(inst_dogHead);
        inst_dogHead.StopAnim();
        inst_dogHead.SR.sprite = inst_dogHead.onWinSprite;
        inst_dogHead.FadeOutAnim();
        MGM.WinMiniGame();
    }
    public override void Lose()
    {

    }
    public override bool SuccessCheck()
    {
        return pettings >= targetPetting;
    }

    public void RegisterPetting()
    {
        pettings = (pettings >= targetPetting) ? targetPetting : pettings + 1;
        UpdateMGUI();
        inst_dogHead.TapEffect(tapForce);
        if (SuccessCheck())
            Win();
    }

    public void UpdateMGUI()
    {
        UIworld_petCounter.text = (targetPetting - pettings).ToString();
    }

    #region MODS
    public void ApplyMod()
    {

    }

    #endregion
}
