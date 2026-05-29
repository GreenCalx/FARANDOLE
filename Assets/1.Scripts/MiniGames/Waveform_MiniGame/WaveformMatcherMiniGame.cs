using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static Utils;
public class WaveformMatcherMiniGame : MiniGame, IArcadeFamily
{
    internal enum EWaveForms
    {
        SIN = 0,
        SQR = 1,
        TRI = 2
    }
    private struct Waveshape
    {
        public float freq;
        public float amp;
        public EWaveForms form;
        public Waveshape(float iFreq, float iAmp, EWaveForms iForm) { freq = iFreq; amp = iAmp; form = iForm; }
    }

    [Header("WaveformMatcherMiniGame")]
    public UIArcadeInputs m_ArcadeInputs;
    public Vector2 windowSize;
    [Header("Tweaks")]
    public float minAmp = 0f;
    public float maxAmp = 1f;
    public float minFreq = 1f;
    public float maxFreq = 2f;
    public float[] ampRangeMulByDiff;
    public float[] freqRangeMulByDiff;
    [Range(0f, 1f)]
    public float targetRandGround = 0.1f; // unit ircle center exclusion
    [Range(0f,1f)]
    public float precisionDiff = 0f;

    [Header("Func Draw")]
    public int resolution = 60;
    // Careful if changing below range
    // Upper bound must be reachable by XY Controller
    // current clamp code doesn't check if the 'limited' randpoint
    // is still reachable within unit circle
    // Examples : diagonals 
    public Transform targetWaveformPoint;
    public Transform controlledWaveformPoint;
    public Material LRTargetMat;
    public Material LRControllerMat;
    public Material LROnMatchMat;
    [Header("GameUI Feedbacks")]
    public Material defaultFormMat;
    public Material selectedFormMat;
    public Image sinImgSelector;
    public Image sqrImgSelector;
    public Image triImgSelector;

    Waveshape target;
    LineRenderer targetLR;
    Waveshape controlled;
    LineRenderer controlledLR;
    float minAmpByDiff, maxAmpByDiff;
    float minFreqByDiff, maxFreqByDiff;
    float freqCentroid, ampCentroid;
    Vector2 successPoint;
    public override void Init()
    {
        //Reset();
    }

    public override void Reset()
    {
        int rank = MGM.miniGamesDifficulty - 1;
        minAmpByDiff = Mathf.Max(0f, minAmp - (minAmp*ampRangeMulByDiff[rank]));
        maxAmpByDiff = maxAmp + (maxAmp*ampRangeMulByDiff[rank]);
                
        minFreqByDiff   = Mathf.Max(0f, minFreq - (minFreq*freqRangeMulByDiff[rank]));
        maxFreqByDiff   = maxFreq +  (maxFreq*freqRangeMulByDiff[rank]);

        freqCentroid = (maxFreqByDiff + minFreqByDiff) / 2f;
        ampCentroid = (maxAmpByDiff + minAmpByDiff) / 2f;

        // Get a random target value, excluding centroid values
        // A targetRandGround too high transform the random range to a 'box'
        // leading to unreachable values via XY circle controller

        successPoint = Utils.RandomInsideDisc(1f, targetRandGround);
        target = new Waveshape(
            Utils.Remap(successPoint.x, -1f, 1f, minFreqByDiff, maxFreqByDiff),
            Utils.Remap(successPoint.y, -1f, 1f, minAmpByDiff, maxAmpByDiff),
            PickRandomFormFunc()
            );

        targetLR = GOBuilder.Create()
                    .WithName("TargetWaveform")
                    .WithParent(transform)
                    .WithPosition(Vector3.zero)
                    .WithLineRenderer(LRTargetMat)
                    .BuildAs<LineRenderer>();
        targetLR.startWidth = 0.05f;
        targetLR.endWidth = 0.05f;


        controlled = new Waveshape(freqCentroid, ampCentroid, PickRandomFormFunc());
        controlledLR = GOBuilder.Create()
                    .WithName("ControlledWaveform")
                    .WithParent(transform)
                    .WithPosition(Vector3.zero)
                    .WithLineRenderer(LRControllerMat)
                    .BuildAs<LineRenderer>();
        controlledLR.startWidth = 0.05f;
        controlledLR.endWidth = 0.05f;


        // Arcade inputs
        // Form
        m_ArcadeInputs.Btn1Unbind();
        m_ArcadeInputs.OnBtn1Press(() => { ChangeSelectedFunc(); });
        m_ArcadeInputs.OnBtn1Release(() => {  });
        
        // XY
        PC.AddPositionTracker(m_ArcadeInputs.xyController);
        UnityAction<Vector2> xyListener = (v) =>
                                    {
                                        Debug.Log("controller : " + v);
                                        controlled.freq = Utils.Remap(v.x, -1f, 1f, minFreqByDiff, maxFreqByDiff);
                                        controlled.amp  = Utils.Remap(v.y, -1f, 1f, minAmpByDiff, maxAmpByDiff);
                                        DrawControlled();
                                    };
        m_ArcadeInputs.XYBind(xyListener);

        FuncSelect();

        DisableUnavailableFunc();
    }
    public override void Play()
    {
        if (IsActiveMiniGame)
            return;

        FuncSelect();
        DrawTarget();
        DrawControlled();
        IsActiveMiniGame = true;
    }
    public override void Stop()
    {
        Destroy(targetLR.gameObject);
        targetLR = null;

        Destroy(controlledLR.gameObject);
        controlledLR = null;

        m_ArcadeInputs.xyController.Reset();
        PC.RemovePositionTracker(m_ArcadeInputs.xyController);
        IsActiveMiniGame = false;
    }
    
    public override void Win()
    {
        MGM.WinMiniGame();
        PC.RemovePositionTracker(m_ArcadeInputs.xyController);

        controlled.freq = target.freq;
        controlled.amp = target.amp;
        targetLR.material = LROnMatchMat;
        controlledLR.material = LROnMatchMat;
        DrawControlled();
    }
    public override void Lose()
    {
        base.Lose();
    }
    public override bool SuccessCheck()
    {
        if (GameIsLost)
            return false;

        bool eq_values = Vector2.Distance(successPoint, m_ArcadeInputs.xyController.XY ) < precisionDiff;
        bool eq_form = controlled.form == target.form;
        return eq_values && eq_form;
    }

    void Update()
    {
        if (!IsActiveMiniGame || IsInPostGame)
            return;
        if (SuccessCheck())
            Win();
    }

    void FuncSelect()
    {
        PickRandomFormFunc();
        SetSelectedUIForm();
    }

    void ChangeSelectedFunc()
    {
        Array enum_values = Enum.GetValues(typeof(EWaveForms));
        int range = Math.Min(enum_values.Length, MGM.miniGamesDifficulty);
        int current = (int)controlled.form;
        if (++current >= range)
        {
            current = 0;
        }
        controlled.form = (EWaveForms)current;
        SetSelectedUIForm();
        DrawControlled();
    }

    void DisableUnavailableFunc()
    {
        sinImgSelector.transform.parent.gameObject.SetActive(true);
        sqrImgSelector.transform.parent.gameObject.SetActive(true);
        triImgSelector.transform.parent.gameObject.SetActive(true);
        Array enum_values = Enum.GetValues(typeof(EWaveForms));
        int start = Math.Min(enum_values.Length, MGM.miniGamesDifficulty);
        for(int i = start; i < enum_values.Length; i++)
        {
            switch (enum_values.GetValue(i))
            {
                case EWaveForms.SIN:
                    sinImgSelector.transform.parent.gameObject.SetActive(false);
                    break;
                case EWaveForms.SQR:
                    sqrImgSelector.transform.parent.gameObject.SetActive(false);
                    break;
                case EWaveForms.TRI:
                    triImgSelector.transform.parent.gameObject.SetActive(false);
                    break;
                default:
                    break;                
            }


        }
             
    }
    EWaveForms PickRandomFormFunc()
    {
        Array enum_values = Enum.GetValues(typeof(EWaveForms));
        int range = Math.Min(enum_values.Length, MGM.miniGamesDifficulty);
        return (EWaveForms)enum_values.GetValue(UnityEngine.Random.Range(0, range));
    }

    public void SetSelectedUIForm()
    {
        bool isSin = controlled.form == EWaveForms.SIN;
        bool isTri = controlled.form == EWaveForms.TRI;
        bool isSqr = controlled.form == EWaveForms.SQR;

        sqrImgSelector.material = isSqr ? selectedFormMat : defaultFormMat;
        triImgSelector.material = isTri ? selectedFormMat : defaultFormMat;
        sinImgSelector.material = isSin ? selectedFormMat : defaultFormMat;
    }

    void DrawControlled()
    {
        DrawFunc(controlledLR, controlled);

    }
    void DrawTarget()
    {
        DrawFunc(targetLR, target);
    }

    void DrawFunc(LineRenderer line, Waveshape wave)
    {
        switch (wave.form)
        {
            case EWaveForms.SIN:
                line.SetPositions(GetSinWave(wave.amp, wave.freq, controlledWaveformPoint.position, line));
                break;
            case EWaveForms.SQR:
                line.SetPositions(GetSquaredWave(wave.amp, wave.freq, controlledWaveformPoint.position, line));
                break;
            case EWaveForms.TRI:
                line.SetPositions(GetTriangularWave(wave.amp, wave.freq, controlledWaveformPoint.position, line));
                break;
            default:
                line.SetPositions(GetSinWave(wave.amp, wave.freq, controlledWaveformPoint.position, line));
                break;
        }
    }
    
    public Vector3[] GetSinWave(float iAmp, float iFreq, Vector3 iWorldAnchor, LineRenderer line)
    {
        Vector3[] positions = new Vector3[resolution];
        line.positionCount = resolution;
        float fPoints = (float)resolution;
        float x = 0f, y = 0f;
        float xStep = windowSize.x / fPoints;
        for (int i = 0; i < resolution; i++)
        {
            x = i * xStep;
            y = iAmp * Mathf.Sin(x / iFreq);
            positions[i] = new Vector3(
                iWorldAnchor.x + /*Utils.Remap(x, xStart, xFinish, 0f, 1f)*/ +x,
                iWorldAnchor.y + /*Utils.Remap(y, -iAmp, iAmp, -0.2f, 0.2f)*/ +y,
                -1f
            );
        }
        return positions;
    }
    
public Vector3[] GetSquaredWave(float iAmp, float iFreq, Vector3 iWorldAnchor, LineRenderer line)
{
    int cornerCount = Mathf.FloorToInt(3 / iFreq) + 4;
    line.positionCount = cornerCount;

    Vector3[] positions = new Vector3[cornerCount];

    float xStep = windowSize.x * iFreq / 3;
    float x = 0;
    float y = iAmp / 2;
        for (int i = 0; i < cornerCount; i++)
        {
            if (i % 2 == 0)
                x = i * xStep;
            else
                y = -y;

            positions[i] = new Vector3(
                iWorldAnchor.x + x,
                iWorldAnchor.y + y,
                -1f
            );
        }

    return positions;
}

    public Vector3[] GetTriangularWave(float iAmp, float iFreq, Vector3 iWorldAnchor, LineRenderer line)
    {
        int peakCount = Mathf.FloorToInt(2 / iFreq) + 2;
        line.positionCount = peakCount;
        float xStep = windowSize.x * iFreq / 2f;
        Vector3[] peaks = new Vector3[peakCount];
        float x = 0;
        float y = iAmp;

        for (int i = 0; i < peakCount; i++)
        {
            x = i * xStep;
            y = -y;

            peaks[i] = new Vector3(
                iWorldAnchor.x + x,
                iWorldAnchor.y + y,
                -1f
            );
        }

        return peaks;
    }
    
    #region MODS
    public void ApplyMod()
    {

    }

    #endregion
}
