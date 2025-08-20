using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using static Utils;

public class PlaygroundManager : MonoBehaviour, IManager
{
    public Bounds bounds;
    public CompositeCollider2D compositeCollider;
    public Material diff1Mat;
    public Material diff2Mat;
    public Material diff3Mat;
    public Material playFieldMat;
    public Texture2D LoopLevelColorGrading;
    public bool AnimateBG = true;
    public float AnimationDeltaTime = 0.5f;
    private float currAnimationDeltaTime;
    List<Color> loopLevelColors;
    GameObject go_colliders, go_fg, go_playfield;
    MeshRenderer FG_MR, PF_MR;
    Coroutine AnimationCoroutine;
    LayerManager2D LM2D;
    public float height
    {
        get
        {
            return bounds.size.y;
        }
    }
    public float width
    {
        get
        {
            return bounds.size.x;
        }
    }
    // public string postProcessLayer = "PostFX";
    // public PostProcessProfile ref_postProcessProfile;
    // GameObject LocalPostFX;
    #region IManager
    public void Init(GameManager iGameManager)
    {
        LM2D = iGameManager.LM2D;

        InitColorGrading();
        BuildPlayground();
    }
    public bool IsReady()
    {
        return (
            (go_colliders != null) &&
            (go_fg != null) &&
            (go_playfield != null)
        );
    }
    #endregion

    void InitColorGrading()
    {
        Color32[] colors = LoopLevelColorGrading.GetPixels32(0);
        loopLevelColors = new List<Color>(colors.Length);
        for (int i = 0; i < colors.Length; i++)
        {
            loopLevelColors.Add(colors[i]);
        }
    }

    void BuildPlayground()
    {
        Vector2 fullScreenWorldSize = GetFullScreenWorldSize();
        Vector2 gameFieldSize = new Vector2(fullScreenWorldSize.x * GameData.GetSettings.GameFieldScreenProportion,
                                            fullScreenWorldSize.y * GameData.GetSettings.GameFieldScreenProportion);

        Mesh BG_Mesh, Playfield_Mesh, FG_Mesh;
        go_colliders = GOBuilder.Create()
                                    .WithName("Playground Colliders")
                                    .WithParent(transform)
                                    .WithPosition(GetWorldPos(new Vector2(Screen.safeArea.size.x / 2f, Screen.safeArea.size.y / 2f)))
                                    .WithRB2D(RigidbodyType2D.Static)
                                    .WithBoxCollider2DAndMesh(Vector2.zero, fullScreenWorldSize, out BG_Mesh, Collider2D.CompositeOperation.Merge)
                                    .WithBoxCollider2DAndMesh(Vector2.zero, gameFieldSize, out Playfield_Mesh, Collider2D.CompositeOperation.Difference)
                                    .WithCompositeCollider2D(out FG_Mesh)
                                    .Build();

        go_fg = GOBuilder.Create()
                            .WithName("PlaygroundForground")
                            .WithParent(transform)
                            .WithLocalPosition(Vector3.zero)
                            .WithMeshFilter(FG_Mesh, true)
                            .WithRenderer(diff1Mat)
                            .Build();
        FG_MR = go_fg.GetComponent<MeshRenderer>();


        go_playfield = GOBuilder.Create()
                            .WithName("PlayField")
                            .WithParent(transform)
                            .WithLocalPosition(Vector3.zero)
                            .WithMeshFilter(Playfield_Mesh, true)
                            .WithRenderer(playFieldMat)
                            .Build();
        PF_MR = go_playfield.GetComponent<MeshRenderer>();
        bounds = PF_MR.bounds;

        LM2D.PlaceForgroundRoot(go_fg.GetComponent<Renderer>());
        LM2D.PlaceBackgroundRoot(go_playfield.GetComponent<Renderer>());

        AnimationCoroutine = StartCoroutine(AnimateCo());
    }

    void OnDestroy()
    {
        if (AnimationCoroutine != null)
        {
            StopCoroutine(AnimationCoroutine);
            AnimationCoroutine = null;
        }
    }

    public void RefreshMatFromDiff(int iDifficultyLevel)
    {
        if (FG_MR == null)
            return;
        switch (iDifficultyLevel)
        {
            case 1:
                FG_MR.material = diff1Mat;
                break;
            case 2:
                FG_MR.material = diff2Mat;
                break;
            case 3:
                FG_MR.material = diff3Mat;
                break;
            default:
                break;
        }
    }

    public void RefreshMatFromLoopLevel(int iLoopLevel)
    {
        Color c = (iLoopLevel >= loopLevelColors.Count) ? loopLevelColors[loopLevelColors.Count - 1] : loopLevelColors[iLoopLevel];
        FG_MR.material.SetColor("_Color", c);
        currAnimationDeltaTime = AnimationDeltaTime / iLoopLevel;
    }

    IEnumerator AnimateCo()
    {
        currAnimationDeltaTime = AnimationDeltaTime;
        short phase = 0;
        Vector2 phase0 = Vector2.zero;
        Vector2 phase1 = new Vector2(0.5f, 0f);
        while (AnimateBG)
        {
            if (phase == 0)
            {
                FG_MR.material.SetTextureOffset("_MainTex", phase0);
                phase++;
            }
            else if (phase == 1)
            {
                FG_MR.material.SetTextureOffset("_MainTex", phase1);
                phase = 0;
            }
            yield return new WaitForSeconds(currAnimationDeltaTime);
        }

    }

    public bool IsWorldPosOOB(Vector2 iWorldPos)
    {
        return bounds.Contains(new Vector3(iWorldPos.x, iWorldPos.y, 0f));
    }

    public bool IsScreenPosOOB(Vector2 iScreenPos)
    {
        Vector3 proj = Camera.main.ScreenToWorldPoint(iScreenPos);
        return bounds.Contains(proj);
    }

    public float GetYPosFromHeightFrac(float iFrac)
    {
        return bounds.min.y + (height * iFrac);
    }

    public float GetXPosFromWidthFrac(float iFrac)
    {
        return bounds.min.x + (width * iFrac);
    }
}
