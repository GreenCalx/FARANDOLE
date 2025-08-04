using UnityEngine;
using TMPro;

public class DogPetMiniGame : MiniGame
{
    [Header("DogPetMiniGame")]
    public GameObject prefab_dogHead;
    DogHead inst_dogHead;
    int pettings = 0;
    int targetPetting = 10;

    public TextMeshProUGUI UIworld_petCounter;
    public override void Init()
    {
        inst_dogHead = GOBuilder.Create(prefab_dogHead)
                        .WithName("DogHead")
                        .WithPosition(Vector3.zero)
                        .Build().GetComponent<DogHead>();
        inst_dogHead.tapCB.AddListener(RegisterPetting);
        pettings = 0;
        UpdateMGUI();
        PC.AddTapTracker(inst_dogHead);
    }
    public override void Play()
    {
        IsActiveMiniGame = true;
    }
    public override void Stop()
    {
        PC.RemoveTapTracker(inst_dogHead);
        Destroy(inst_dogHead.gameObject);
        IsActiveMiniGame = false;
    }
    public override void Win()
    {
        PC.RemoveTapTracker(inst_dogHead);
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
        inst_dogHead.TapEffect(MGM.miniGamesDifficulty);
        if (SuccessCheck())
            Win();
    }

    public void UpdateMGUI()
    {
        UIworld_petCounter.text = (targetPetting - pettings).ToString();
    }
}
