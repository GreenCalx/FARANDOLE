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
    public XYController xyController;
    public int resolution = 60;
    // Careful if changing below range
    // Upper bound must be reachable by XY Controller
    // current clamp code doesn't check if the 'limited' randpoint
    // is still reachable within unit circle
    // Examples : diagonals 
    [Range(0f, 0.5f)]
    public float targetRandGround = 0.1f; // unit ircle center exclusion
    public Transform targetWaveformPoint;
    public Transform controlledWaveformPoint;
    public Material LRTargetMat;
    public Material LRControllerMat;

    Waveshape target;
    LineRenderer targetLR;
    Waveshape controlled;
    LineRenderer controlledLR;
    float minAmpByDiff, maxAmpByDiff;
    float minFreqByDiff, maxFreqByDiff;
    float freqCentroid, ampCentroid;
    public override void Init()
    {
        float rangeExtension = ((float)MGM.miniGamesDifficulty - 1f) / 10f;
        minAmpByDiff = minAmp   - (minAmp * rangeExtension);
        minFreqByDiff = minFreq - (minFreq * rangeExtension);
        maxFreqByDiff = maxFreq + (maxFreq * rangeExtension);
        maxAmpByDiff = maxAmp   + (maxAmp * rangeExtension);

        freqCentroid = (maxFreqByDiff + minFreqByDiff) / 2f;
        ampCentroid = (maxAmpByDiff + minAmpByDiff) / 2f;

        // Get a random target value, excluding centroid values
        // A targetRandGround too high transform the random range to a 'box'
        Vector2 randPoint = Random.insideUnitCircle;
        if ((randPoint.x <= targetRandGround)&&(randPoint.x >= -targetRandGround))
            randPoint.x = (randPoint.x < 0f) ? -targetRandGround : targetRandGround;
        if ((randPoint.y <= targetRandGround)&&(randPoint.y >= -targetRandGround))
            randPoint.y = (randPoint.y < 0f) ? -targetRandGround : targetRandGround;

        target = new Waveshape(
            Utils.Remap(randPoint.x, -1f, 1f, minFreqByDiff, maxFreqByDiff),
            Utils.Remap(randPoint.y, -1f, 1f, minAmpByDiff, maxAmpByDiff)
            );

        targetLR = GOBuilder.Create()
                    .WithName("TargetWaveform")
                    .WithParent(transform)
                    .WithPosition(Vector3.zero)
                    .WithLineRenderer(LRTargetMat)
                    .Build()
                    .GetComponent<LineRenderer>();
        targetLR.startWidth = 0.05f;
        targetLR.endWidth = 0.05f;

        controlled = new Waveshape(freqCentroid, ampCentroid);
        controlledLR = GOBuilder.Create()
                    .WithName("TargetWaveform")
                    .WithParent(transform)
                    .WithPosition(Vector3.zero)
                    .WithLineRenderer(LRControllerMat)
                    .Build()
                    .GetComponent<LineRenderer>();
        controlledLR.startWidth = 0.05f;
        controlledLR.endWidth = 0.05f;

        PC.AddPositionTracker(xyController);
    }
    public override void Play()
    {
        if (IsActiveMiniGame)
            return;

        DrawTarget();
        DrawControlled();
        IsActiveMiniGame = true;
    }
    public override void Stop()
    {
        Destroy(targetLR.gameObject);
        Destroy(controlledLR.gameObject);
        xyController.Reset();
        PC.RemovePositionTracker(xyController);
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
        bool eq_freq = Mathf.Abs(controlled.freq - target.freq) < 0.02f;
        bool eq_amp = Mathf.Abs(controlled.amp - target.amp) < 0.02f;
        return eq_freq && eq_amp;
    }

    void Update()
    {
        Debug.Log(xyController.XY);
        controlled.freq = Utils.Remap(xyController.XY.x, -1f, 1f, minFreqByDiff, maxFreqByDiff);
        controlled.amp = Utils.Remap(xyController.XY.y, -1f, 1f, minAmpByDiff, maxAmpByDiff);
        DrawControlled();
        if (SuccessCheck())
            Win();
    }
    void DrawControlled()
    {
        controlledLR.positionCount = resolution;
        controlledLR.SetPositions(GetSinWave(controlled.amp, controlled.freq, controlledWaveformPoint.position));
    }
    void DrawTarget()
    {
        targetLR.positionCount = resolution;
        targetLR.SetPositions(GetSinWave(target.amp, target.freq, targetWaveformPoint.position));
    }

    public Vector3[] GetSinWave(float iAmp, float iFreq, Vector3 iWorldAnchor)
    {
        Vector3[] positions = new Vector3[resolution];

        float fPoints = (float)resolution;
        float  x = 0f, y = 0f;
        float xStep = windowSize.x / fPoints;
        for (int i = 0; i < resolution; i++)
        {
            x = i * xStep;
            y = iAmp * Mathf.Sin(x / iFreq);
            positions[i] = new Vector3(
                iWorldAnchor.x + /*Utils.Remap(x, xStart, xFinish, 0f, 1f)*/ + x,
                iWorldAnchor.y + /*Utils.Remap(y, -iAmp, iAmp, -0.2f, 0.2f)*/ + y,
                -1f
            );
        }
        return positions;
    }
}
