using UnityEngine;

public class WaveformMatcherMiniGame : MiniGame
{
    private struct Waveshape
    {
        public float freq;
        public float amp;
        public Waveshape(float iFreq, float iAmp) { freq = iFreq; amp = iAmp; }
    }

    [Header("WaveformMatcherMiniGame")]
    public Vector2 windowSize;
    public float minAmp = 0f;
    public float maxAmp = 1f;
    public float minFreq = 1f;
    public float maxFreq = 2f;

    [Range(0f,1f)]
    public float ampDiffCoefRangeExtension;
    [Range(0f,1f)]
    public float freqDiffCoefRangeExtension;

    public XYController xyController;
    public int resolution = 60;
    // Careful if changing below range
    // Upper bound must be reachable by XY Controller
    // current clamp code doesn't check if the 'limited' randpoint
    // is still reachable within unit circle
    // Examples : diagonals 
    [Range(0f, 1f)]
    public float targetRandGround = 0.1f; // unit ircle center exclusion
    public float targetRandGroundExtension = 0.1f;

    [Range(0f,0.02f)]
    public float precisionDiff = 0f;
    public Transform targetWaveformPoint;
    public Transform controlledWaveformPoint;
    public Material LRTargetMat;
    public Material LRControllerMat;
    public Material LROnMatchMat;

    Waveshape target;
    LineRenderer targetLR;
    Waveshape controlled;
    LineRenderer controlledLR;
    float minAmpByDiff, maxAmpByDiff;
    float minFreqByDiff, maxFreqByDiff;
    float freqCentroid, ampCentroid;

    int func;
    public override void Init()
    {
        int rank = MGM.miniGamesDifficulty - 1;
        minAmpByDiff = minAmp * Mathf.Pow(1- ampDiffCoefRangeExtension, rank);
        minFreqByDiff = minFreq * Mathf.Pow(1 - freqDiffCoefRangeExtension, rank);
        maxFreqByDiff = maxFreq * Mathf.Pow(1 - freqDiffCoefRangeExtension, rank);
        maxAmpByDiff = maxAmp *  Mathf.Pow( 1 - ampDiffCoefRangeExtension, rank);

        freqCentroid = (maxFreqByDiff + minFreqByDiff) / 2f;
        ampCentroid = (maxAmpByDiff + minAmpByDiff) / 2f;

        Reset();
    }

    public override void Reset()
    {
        // Get a random target value, excluding centroid values
        // A targetRandGround too high transform the random range to a 'box'
        // leading to unreachable values via XY circle controller

        Vector2 randPoint = Random.insideUnitCircle;
        int i = 0;
        while(i < 50 || ((randPoint.x <= targetRandGround) && (randPoint.x >= -targetRandGround)) || ((randPoint.y <= targetRandGround) && (randPoint.y >= -targetRandGround)))
        {
            randPoint = Random.insideUnitCircle;
            i++;
        }
        if(i == 50){
            randPoint.x = targetRandGround;
            randPoint.y = targetRandGround;
        }

        target = new Waveshape(
            Utils.Remap(randPoint.x, -1f, 1f, minFreqByDiff, maxFreqByDiff),
            Utils.Remap(randPoint.y, -1f, 1f, minAmpByDiff, maxAmpByDiff)
            );

        targetLR = GOBuilder.Create()
                    .WithName("TargetWaveform")
                    .WithParent(transform)
                    .WithPosition(Vector3.zero)
                    .WithLineRenderer(LRTargetMat)
                    .BuildAs<LineRenderer>();
        targetLR.startWidth = 0.05f;
        targetLR.endWidth = 0.05f;
        //targetLR.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        controlled = new Waveshape(freqCentroid, ampCentroid);
        controlledLR = GOBuilder.Create()
                    .WithName("ControlledWaveform")
                    .WithParent(transform)
                    .WithPosition(Vector3.zero)
                    .WithLineRenderer(LRControllerMat)
                    .BuildAs<LineRenderer>();
        controlledLR.startWidth = 0.05f;
        controlledLR.endWidth = 0.05f;
        //controlledLR.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        PC.AddPositionTracker(xyController);

        func = Random.Range(0,3);
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

        xyController.Reset();
        PC.RemovePositionTracker(xyController);
        IsActiveMiniGame = false;
    }
    public override void Win()
    {
        MGM.WinMiniGame();
        PC.RemovePositionTracker(xyController);

        controlled.freq = target.freq;
        controlled.amp = target.amp;
        targetLR.material = LROnMatchMat;
        controlledLR.material = LROnMatchMat;
        DrawControlled();
    }
    public override void Lose()
    {

    }
    public override bool SuccessCheck()
    {
        bool eq_freq = Mathf.Abs(controlled.freq - target.freq) < 0.02f - precisionDiff * MGM.miniGamesDifficulty;
        bool eq_amp = Mathf.Abs(controlled.amp - target.amp) < 0.02f - precisionDiff * MGM.miniGamesDifficulty;
        return eq_freq && eq_amp;
    }

    void Update()
    {
        if (!IsActiveMiniGame || IsInPostGame)
            return;

        controlled.freq = Utils.Remap(xyController.XY.x, -1f, 1f, minFreqByDiff, maxFreqByDiff);
        controlled.amp = Utils.Remap(xyController.XY.y, -1f, 1f, minAmpByDiff, maxAmpByDiff);
        DrawControlled();
        if (SuccessCheck())
            Win();
    }

    void FuncSelect()
    {
        switch (MGM.miniGamesDifficulty)
        {
            case 2:
                selectedFunc = Random.Range(0, 3);
                break;
            case 3:
                selectedFunc = Random.Range(0, 3);
                break;
            default:
                selectedFunc = 0;
                break;
        }
    }
    void DrawControlled()
    {
        DrawFunc(func, controlledLR, controlled);
        //DrawFunc(Random.Range(0, 2), controlledLR);
    }
    void DrawTarget()
    {
        DrawFunc(func, targetLR, target);
        //DrawFunc(Random.Range(0, 2), targetLR);
    }

    void DrawFunc(int func, LineRenderer line, Waveshape wave)
    {
        if (func == 0) //Sin
        {
            line.SetPositions(GetSinWave(wave.amp, wave.freq, controlledWaveformPoint.position, line));
        }
        else if (func == 1)//tri
        {
            line.SetPositions(GetTriangularWave(wave.amp, wave.freq, controlledWaveformPoint.position, line));
        }
        else//Squared by default
        {

            line.SetPositions(GetSquaredWave(wave.amp, wave.freq, controlledWaveformPoint.position, line));
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
    float xOffset = xStep * Lerp(minFreqByDiff, maxFreqByDiff, iFreq);
    float x = 0;
    float y = iAmp / 2;
        for (int i = 0; i < cornerCount; i++)
        {
            if (i % 2 == 0)
                x = i * xStep;
            else
                y = -y;

            positions[i] = new Vector3(
                iWorldAnchor.x + x - xOffset,
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
        float xStep = windowSize.x * iFreq / 2f ;
        Vector3[] peaks = new Vector3[peakCount];
        float xOffset = xStep * Mathf.Lerp(minFreqByDiff, maxFreqByDiff, iFreq);
        float x = 0;
        float y = iAmp;

        for (int i = 0; i < peakCount; i++)
        {
            x = i * xStep;
            y = -y;

            peaks[i] = new Vector3(
                iWorldAnchor.x + x - xOffset,
                iWorldAnchor.y + y,
                -1f
            );
        }

        return peaks;
    }
}
