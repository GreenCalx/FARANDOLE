using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class DragLabyrinthMiniGame : MiniGame
{
    [Header("DragLabyrinthMiniGame")]
    public FinishLine finishLine;
    public GameObject prefab_ballToEscape;
    GameObject inst_ballToEscape;
    RollingBall inst_asBall;
    Dragable    inst_asDragable;

    public Labyrinth inst_Labyrinth;
    public List<GameObject> diff1Layouts;
    public List<GameObject> diff2Layouts;
    public List<GameObject> diff3Layouts;
    LabyrinthLayout selectedLayout;


    public void ClearLayout()
    {
        if (selectedLayout.gameObject != null)
            Destroy(selectedLayout.gameObject);
        if (inst_Labyrinth.visualLab != null)
            Destroy(inst_Labyrinth.visualLab.gameObject);
    }

    public void PickLayout()
    {
        GameObject selectedLayoutPrefab = null;
        switch (MGM.miniGamesDifficulty)
        {
            case 1:
                selectedLayoutPrefab = diff1Layouts[UnityEngine.Random.Range(0, diff1Layouts.Count)];
                break;
            case 2:
                selectedLayoutPrefab = diff2Layouts[UnityEngine.Random.Range(0, diff2Layouts.Count)];
                break;
            case 3:
                selectedLayoutPrefab = diff3Layouts[UnityEngine.Random.Range(0, diff3Layouts.Count)];
                break;
            default:
                Debug.LogError("Very bad error on layout pick for labyrinth with " + MGM.miniGamesDifficulty + " mini game difficutly. NO LAYOUTS !!");
                break;
        }
        if (selectedLayoutPrefab == null)
            return;

        selectedLayout = GOBuilder.Create(selectedLayoutPrefab)
                        .WithParent(inst_Labyrinth.transform)
                        .BuildAs<LabyrinthLayout>();
    }
    public override void Init()
    {
        //Reset();
    }
    public override void Reset()
    {
        PickLayout();
        inst_Labyrinth.SetFromLayout(selectedLayout);
        inst_Labyrinth.transform.rotation = Quaternion.identity;

        inst_ballToEscape = Instantiate(prefab_ballToEscape);
        inst_ballToEscape.transform.parent = transform;
        inst_ballToEscape.transform.position = selectedLayout.spawnPoint.position;

        inst_asBall = inst_ballToEscape.GetComponent<RollingBall>();
        inst_asBall.MG = this;
        inst_asBall.OnFinishCB = new UnityEvent();
        inst_asBall.OnFinishCB.AddListener(() => finishLine.ExplodeAt(inst_asBall.transform.position));
        inst_asBall.OnFinishCB.AddListener(Win);

        inst_asDragable = inst_ballToEscape.GetComponent<Dragable>();
        PC.AddPositionTracker(inst_asDragable);
    }

    public override void Play()
    {
        IsActiveMiniGame = true;
    }
    public override void Stop()
    {
        ClearLayout();
        finishLine.ResetParticles();

        PC.RemovePositionTracker(inst_asDragable);
        if (!!inst_ballToEscape)
            Destroy(inst_ballToEscape.gameObject);
        IsActiveMiniGame = false;
    }
    public override void Win()
    {
        PC.RemovePositionTracker(inst_asDragable);
        MGM.WinMiniGame();
    }
    public override void Lose()
    {

    }
    public override bool SuccessCheck()
    {
        return false;
    }
}
