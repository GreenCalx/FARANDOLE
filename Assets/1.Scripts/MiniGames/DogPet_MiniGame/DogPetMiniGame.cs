using UnityEngine;
using TMPro;

using static LocalUtils;

public class DogPetMiniGame : MiniGame, IDogFamily
{
    [Header("DogPetMiniGame")]
    readonly string pettingCountKey = "DogPettings";
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
        ITargetObjectiveExtension objectiveExt = GetExtension<ITargetObjectiveExtension>();
        targetPetting = objectiveExt?.Get(pettingCountKey, MGM.MGLoop.rank) ?? 10;

        inst_dogHead.Reset();

        pettings = 0;
        tapForce = LocalUtils.GetValueFromRank(difficultyTapForces, MGM);
        if (MGM.miniGamesDifficulty >= bounceOnRandomTreshold)
        {
            inst_dogHead.bounceRandomOnTap = true;
        }
        else
            inst_dogHead.bounceRandomOnTap = false;
        UpdateMGUI();
        PC.AddTapTracker(inst_dogHead);

        inst_dogHead.SpringAnchorDrag = LocalUtils.GetValueFromRank(SpringAnchorDrag, MGM);
        inst_dogHead.m_SpringAnchorDragStrength = LocalUtils.GetValueFromRank(SpringAnchorDragStrength, MGM);
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
        base.Lose();
    }
    public override bool SuccessCheck()
    {
        if (GameIsLost)
            return false;
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
