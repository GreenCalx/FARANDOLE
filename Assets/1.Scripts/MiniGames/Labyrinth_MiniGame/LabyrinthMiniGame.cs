using UnityEngine;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
public class LabyrinthMiniGame : MiniGame
{
    [Header("LabyrinthMiniGame")]
    public FinishLine finishLine;
    public TorqueRotater rotater;
    public GameObject prefab_ballToEscape;
    GameObject inst_ballToEscape;
    List<GameObject> inst_RollingBalls;
    RollingBall inst_asBall;
    public Labyrinth inst_movingLabyrinth;
    public List<GameObject> diff1Layouts;
    public List<GameObject> diff2Layouts;
    public List<GameObject> diff3Layouts;
    LabyrinthLayout selectedLayout;

    public void ClearLayout()
    {
        if (selectedLayout.gameObject != null)
            Destroy(selectedLayout.gameObject);
        if (inst_movingLabyrinth.visualLab != null)
            Destroy(inst_movingLabyrinth.visualLab.gameObject);
    }

    public void PickLayout()
    {
        if (selectedLayout != null)
        {
            GameObject.DestroyImmediate(selectedLayout);
        }
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
                        .WithParent(inst_movingLabyrinth.transform)
                        .BuildAs<LabyrinthLayout>();
    }
    public override void Init()
    {
        inst_RollingBalls = new List<GameObject>();
        //Reset();
    }

    public override void Reset()
    {
        PickLayout();
        inst_movingLabyrinth.SetFromLayout(selectedLayout);
        inst_movingLabyrinth.transform.rotation = Quaternion.identity;

        for (int i = 0; i < MGM.miniGamesDifficulty; i++)
        {
            RollingBall newBall = GOBuilder.Create(prefab_ballToEscape)
                                .WithParent(transform)
                                .WithPosition(selectedLayout.spawnPoint.position)
                                .BuildAs<RollingBall>();
            newBall.MG = this;
            newBall.OnFinishCB = new UnityEvent();
            newBall.OnFinishCB.AddListener(
                () =>
                    {
                        finishLine.ExplodeAt(newBall.transform.position);
                        inst_RollingBalls.Remove(newBall.gameObject);
                        //GameObject.Destroy(newBall.gameObject);
                    }
                );
            newBall.OnFinishCB.AddListener(() => { if (SuccessCheck()) { Win(); } });
            inst_RollingBalls.Add(newBall.gameObject);
        }

        rotater.Init();
        PC.AddPositionTracker(rotater);
    }
    public override void Play()
    {
        IsActiveMiniGame = true;
    }
    public override void Stop()
    {
        ClearLayout();
        finishLine.ResetParticles();
        inst_movingLabyrinth.transform.rotation = Quaternion.identity;
        inst_movingLabyrinth.Reset();
        foreach (GameObject rollingBall in inst_RollingBalls)
        {
            if (rollingBall!=null)
                GameObject.Destroy(rollingBall);
        }

        PC.RemovePositionTracker(rotater);
        IsActiveMiniGame = false;
    }
    public override void Win()
    {
        PC.RemovePositionTracker(rotater);
        MGM.WinMiniGame();
    }

    public override bool SuccessCheck()
    {
        //inst_RollingBalls = inst_RollingBalls.ForEach(e => e != null);
        return inst_RollingBalls.Where(e => e != null).ToList().Count == 0;
    }
}
