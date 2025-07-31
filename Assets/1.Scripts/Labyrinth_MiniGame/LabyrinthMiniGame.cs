using UnityEngine;
using System.Collections.Generic;

public class LabyrinthMiniGame : MiniGame
{
    public TorqueRotater rotater;
    public GameObject prefab_ballToEscape;
    GameObject inst_ballToEscape;
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

        GameObject createdLayout = Instantiate(selectedLayoutPrefab);
        createdLayout.transform.parent = inst_movingLabyrinth.transform;

        selectedLayout = createdLayout.GetComponent<LabyrinthLayout>();
    }
    public override void Init()
    {
        PickLayout();
        inst_movingLabyrinth.SetFromLayout(selectedLayout);
        inst_movingLabyrinth.transform.rotation = Quaternion.identity;

        inst_ballToEscape = Instantiate(prefab_ballToEscape);
        inst_ballToEscape.transform.parent = transform;
        inst_ballToEscape.transform.position = selectedLayout.spawnPoint.position;

        inst_asBall = inst_ballToEscape.GetComponent<RollingBall>();
        inst_asBall.MG = this;

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
        inst_movingLabyrinth.transform.rotation = Quaternion.identity;
        inst_movingLabyrinth.Reset();

        PC.RemovePositionTracker(rotater);
        if (!!inst_ballToEscape)
            Destroy(inst_ballToEscape.gameObject);
        IsActiveMiniGame = false;
    }
    public override void Win()
    {
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
