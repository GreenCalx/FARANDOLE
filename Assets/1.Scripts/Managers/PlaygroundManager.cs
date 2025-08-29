using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using System.Threading.Tasks;
using static Utils;

public class PlaygroundManager : MonoBehaviour, IManager
{
    public Bounds bounds;
    public CompositeCollider2D compositeCollider;
    public Material diff1Mat;
    public Material diff2Mat;
    public Material diff3Mat;
    public Material playFieldMat;
    public Material clearAnimMat;
    public Material forgroundFrameMat;
    public Sprite clearAnimSprite;
    public Texture2D LoopLevelColorGrading;
    public bool AnimateBG = true;
    public float AnimationDeltaTime = 0.5f;
    private float currAnimationDeltaTime;
    List<Color> loopLevelColors;
    GameObject go_colliders, go_fg, go_playfield;
    DoorAnim doorAnimation;
    MeshRenderer FG_MR, PF_MR;
    Coroutine AnimationCoroutine;
    LayerManager2D LM2D;
    MiniGameManager MGM;
    LineRenderer forgroundFrameLR;
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
        MGM = iGameManager.MGM;

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
                            .WithLocalPosition(new Vector3(0f,0f,-1f))
                            .WithMeshFilter(FG_Mesh, true)
                            .WithRenderer(diff1Mat)
                            .Build();
        FG_MR = go_fg.GetComponent<MeshRenderer>();


        go_playfield = GOBuilder.Create()
                            .WithName("PlayField")
                            .WithParent(transform)
                            .WithLocalPosition(new Vector3(0f,0f,1f))
                            .WithMeshFilter(Playfield_Mesh, true)
                            .WithRenderer(playFieldMat)
                            .Build();
        PF_MR = go_playfield.GetComponent<MeshRenderer>();
        bounds = PF_MR.bounds;

        Mesh halfWidthMesh = CreateHalfWidthMesh();
        doorAnimation = GOBuilder.Create()
                        .WithName("DoorAnimation")
                        .WithParent(transform)
                        .WithPosition(new Vector2(0f, bounds.min.y))
                        .Build().AddComponent<DoorAnim>();
        GameObject doorLeft = GOBuilder.Create()
                                .WithName("DoorLeft")
                                .WithParent(doorAnimation.transform)
                                .WithLocalPosition(new Vector3(bounds.min.x, 0f))
                                .WithMeshFilter(halfWidthMesh, false)
                                .WithRenderer(clearAnimMat)
                                .Build();
        GameObject doorRight = GOBuilder.Create()
                                .WithName("DoorRight")
                                .WithParent(doorAnimation.transform)
                                .WithLocalPosition(Vector3.zero)
                                .WithMeshFilter(halfWidthMesh, false)
                                .WithRenderer(clearAnimMat)
                                .Build();
        doorAnimation.Init(doorLeft.transform, doorRight.transform, bounds.size.x/2f);
        doorAnimation.ForceOpen();

        forgroundFrameLR = GOBuilder.Create()
                                    .WithName("ForgroundFrame")
                                    .WithParent(transform)
                                    .WithLocalPosition(Vector3.zero)
                                    .WithLineRenderer(forgroundFrameMat)
                                    .BuildAs<LineRenderer>();


        LM2D.PlaceForgroundReserve(doorLeft.GetComponent<Renderer>());
        LM2D.PlaceForgroundReserve(doorRight.GetComponent<Renderer>());
        LM2D.PlaceForgroundReserve(go_fg.GetComponent<Renderer>());
        LM2D.PlaceForgroundReserve(forgroundFrameLR.GetComponent<Renderer>());

        LM2D.PlaceBackgroundReserve(go_playfield.GetComponent<Renderer>());

        UpdateForgroundFrame();
        ResetAnimation();
    }

    public void UpdateForgroundFrame()
    {
        Vector3[] pos = new Vector3[4];
        pos[0] = new Vector3(bounds.max.x, bounds.max.y, forgroundFrameLR.transform.position.z);
        pos[1] = new Vector3(bounds.max.x, bounds.min.y, forgroundFrameLR.transform.position.z);
        pos[2] = new Vector3(bounds.min.x, bounds.min.y, forgroundFrameLR.transform.position.z);
        pos[3] = new Vector3(bounds.min.x, bounds.max.y, forgroundFrameLR.transform.position.z);
        forgroundFrameLR.positionCount = 4;
        forgroundFrameLR.loop = true;
        forgroundFrameLR.startWidth = 0.05f;
        forgroundFrameLR.endWidth = 0.05f;
        forgroundFrameLR.SetPositions(pos);
    }

    public Mesh CreateHalfWidthMesh()
    {
        Mesh m = new Mesh();
        Color c = Color.black;

        VertexHelper vh = new VertexHelper();
        float half_w = bounds.size.x / 2f;
        float h = bounds.size.y;

        vh.AddVert(new Vector3(0, 0), c, new Vector2(0f, 0f));
        vh.AddVert(new Vector3(0, h), c, new Vector2(0f, 1f));
        vh.AddVert(new Vector3(half_w, h), c, new Vector2(1f, 1f));
        vh.AddVert(new Vector3(half_w, 0), c, new Vector2(1f, 0f));

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);

        vh.FillMesh(m);

        return m;
    }

    void OnDestroy()
    {
        if (AnimationCoroutine != null)
        {
            StopCoroutine(AnimationCoroutine);
            AnimationCoroutine = null;
        }
    }

    public void ResetAnimation()
    {
        if (AnimationCoroutine != null)
        {
            StopCoroutine(AnimationCoroutine);
            AnimationCoroutine = null;
        }
        AnimationCoroutine = StartCoroutine(AnimateCo());
    }

    public void ClearPlaygroundAnim()
    {
        // doorAnimation.OnCloseCB.AddListener(()=> MGM.Stop());
        // doorAnimation.OnOpenCB.AddListener( ()=> MGM.Play());
        doorAnimation.ClapAnim();
    }

    public async Task ClosePlaygroundAnim()
    {
        await doorAnimation.CloseCo();
    }

    public async Task OpenPlaygroundAnim()
    {
        await doorAnimation.OpenCo();
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
        forgroundFrameLR.material.SetColor("_Color", c);
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
