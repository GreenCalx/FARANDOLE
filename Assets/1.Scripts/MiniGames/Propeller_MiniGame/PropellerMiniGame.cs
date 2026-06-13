using UnityEngine;
using DG.Tweening;

public class WindmillMinigame : MiniGame, IDogFamily
{
    [Header("Difficulty")]
    public float[] flourThresholds;
    // True = seul le sens horaire produit de la farine (AngularSpeed < 0 en convention Unity)
    // Sert de sens du vent initial quand la fleche est active.
    public bool clockwiseOnly = true;

    [Header("Wind")]
    // A rotation Y=0 la fleche doit representer le sens horaire (flip Y=180 = anti-horaire)
    public SpriteRenderer windArrow;
    public int windArrowMinDifficulty = 2;
    public int windChangeMinDifficulty = 5;
    [SerializeField] private float windChangePeriod = 3f;
    [SerializeField] private float windBlinkDuration = 0.8f;
    [SerializeField] private float windBlinkFrequency = 4f;
    [SerializeField] private float windFlipDuration = 0.4f;
    private bool windClockwise;
    private bool windIsBlinking;
    private float windTimer;

    public RotateToCursor inst_propeller;
    private float currentFlour;
    public Transform flourPile;
    private float initialLocalYFlour;
    private float initialLocalXFlour;

    private float flourThreshold;
    [SerializeField] private ParticleSystem flourParticles;
    public float[] maxYFlourOffsets;
    private float maxYFlourOffset;

    [Header("Shake")]
    [SerializeField] private float shakeMagnitude = 0.05f;
    [SerializeField] private float shakeFrequency = 20f;
    private float shakeIntensity;

    [Header("Particles")]
    [SerializeField] private float particleStopDelay = 0.2f;
    private float timeSinceLastValidRotation;

    [Header("Safran")]
    public Transform safran;
    public ParticleSystem safran_ps;
    public Transform brouette;
    [SerializeField] private float safranJumpHeight = 0.3f;
    [SerializeField] private float safranJumpDuration = 0.25f;
    [SerializeField] private float exitOffsetX = 5f;
    [SerializeField] private float exitDuration = 0.5f;
    private float initialLocalYSafran;
    private float initialLocalXSafran;
    private float initialLocalXBrouette;

    public override void Init()
    {
        base.Init();
        initialLocalYFlour = flourPile.localPosition.y;
        initialLocalXFlour = flourPile.localPosition.x;
        initialLocalYSafran = safran.localPosition.y;
        initialLocalXSafran = safran.localPosition.x;
        initialLocalXBrouette = brouette.localPosition.x;
    }

    public override void Reset()
    {
        currentFlour = 0f;
        shakeIntensity = 0f;
        timeSinceLastValidRotation = particleStopDelay;
        flourThreshold = flourThresholds[MGM.miniGamesDifficulty - 1];
        maxYFlourOffset = maxYFlourOffsets[MGM.miniGamesDifficulty - 1];
        PC.AddPositionTracker(inst_propeller);
        flourPile.localPosition = new Vector3(initialLocalXFlour, initialLocalYFlour, flourPile.localPosition.z);
        safran.DOKill();
        safran.localPosition = new Vector3(initialLocalXSafran, initialLocalYSafran, safran.localPosition.z);
        brouette.DOKill();
        brouette.localPosition = new Vector3(initialLocalXBrouette, brouette.localPosition.y, brouette.localPosition.z);
        ResetWind();
    }

    void ResetWind()
    {
        windClockwise = Random.value < 0.5f;
        windIsBlinking = false;
        windTimer = 0f;
        windArrow.transform.DOKill();
        windArrow.transform.localRotation = Quaternion.Euler(0f, windClockwise ? 0f : 180f, 0f);
        windArrow.enabled = true;
        windArrow.gameObject.SetActive(MGM.miniGamesDifficulty >= windArrowMinDifficulty);
    }

    public override void Play()
    {
        base.Play();
    }

    public override void Stop()
    {
        Clean();
        base.Stop();
    }

    public override void Win()
    {
        PlaySafranAnimation();
        PC.RemovePositionTracker(inst_propeller);
        MGM.WinMiniGame();
    }

    void PlaySafranAnimation()
    {
        DOTween.Sequence()
            .Append(safran.DOLocalMoveY(initialLocalYSafran + safranJumpHeight, safranJumpDuration).SetEase(Ease.OutQuad))
            .AppendCallback(() => safran_ps.Play())
            .Append(safran.DOLocalMoveY(initialLocalYSafran, safranJumpDuration).SetEase(Ease.InQuad))
            .AppendCallback(() =>
            {
                safran.DOLocalMoveX(initialLocalXSafran + exitOffsetX, exitDuration).SetEase(Ease.InQuad);
                brouette.DOLocalMoveX(initialLocalXBrouette + exitOffsetX, exitDuration).SetEase(Ease.InQuad);
            });
    }

    public override void Lose()
    {
        base.Lose();
    }

    public override bool SuccessCheck()
    {
        if (GameIsLost) return false;
        return currentFlour >= flourThreshold;
    }

    void Update()
    {
        if (!IsActiveMiniGame || IsInPostGame)
            return;

        UpdateWind();

        float speed = inst_propeller.AngularSpeed;
        bool validDirection = windClockwise ? speed < 0f : speed > 0f;
        if (validDirection)
        {
            currentFlour += Mathf.Abs(speed) * Time.deltaTime / 360;
            currentFlour = Mathf.Min(currentFlour, flourThreshold);
            shakeIntensity = Mathf.Lerp(shakeIntensity, shakeMagnitude, Time.deltaTime * 15f);
            timeSinceLastValidRotation = 0f;
            if (!flourParticles.isPlaying)
                flourParticles.Play();
        }
        else
        {
            timeSinceLastValidRotation += Time.deltaTime;
            shakeIntensity = Mathf.Lerp(shakeIntensity, 0f, Time.deltaTime * 10f);
            if (timeSinceLastValidRotation >= particleStopDelay && flourParticles.isPlaying)
                flourParticles.Stop();
        }

        float t = currentFlour / flourThreshold;
        float shakeX = Mathf.Sin(Time.time * shakeFrequency) * shakeIntensity;

        flourPile.localPosition = new Vector3(
            initialLocalXFlour + shakeX,
            initialLocalYFlour + t * maxYFlourOffset,
            flourPile.localPosition.z
        );

        if (SuccessCheck())
            Win();
    }

    // Au-dessus de windChangeMinDifficulty : clignote pour avertir, puis la fleche
    // se retourne et le sens valide s'inverse.
    void UpdateWind()
    {
        if (MGM.miniGamesDifficulty < windChangeMinDifficulty)
            return;

        windTimer += Time.deltaTime;
        if (!windIsBlinking)
        {
            if (windTimer >= windChangePeriod)
            {
                windTimer = 0f;
                windIsBlinking = true;
            }
        }
        else
        {
            windArrow.enabled = Mathf.FloorToInt(windTimer * windBlinkFrequency * 2f) % 2 == 0;
            if (windTimer >= windBlinkDuration)
            {
                windTimer = 0f;
                windIsBlinking = false;
                windArrow.enabled = true;
                FlipWind();
            }
        }
    }

    void FlipWind()
    {
        windClockwise = !windClockwise;
        windArrow.transform.DOKill();
        windArrow.transform.DOLocalRotate(new Vector3(0f, windClockwise ? 0f : 180f, 0f), windFlipDuration)
            .SetEase(Ease.OutBack);
    }

    void Clean()
    {
        if (inst_propeller != null)
            PC.RemovePositionTracker(inst_propeller);
        if (windArrow != null)
            windArrow.transform.DOKill();
    }
}
