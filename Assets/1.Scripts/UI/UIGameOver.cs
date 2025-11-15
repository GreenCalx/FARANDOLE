using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Threading;
using TMPro;

public class UIGameOver : MonoBehaviour
{
    public UIRunOverview RunOverview;
    public GameOverEffect GameOverFX;
    public RectTransform scoreDisplayHandle;
    public RectTransform newHighScoreDisplayHandle;

    public UIButton TryAgainBtn;
    public UIButton MenuBtn;
    public TextMeshProUGUI scoreDisplayValue;

    Animator animator;
    const string GameOverSkipTrigger = "GameOverSkip";
    const string GameOverClosePlayfieldStateName = "GameOverClosePlayfield";
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    public void Setup(GameManager iGameManager)
    {
        RunOverview.Setup(iGameManager.playerData);
    }

    void Skip()
    {
        animator.SetTrigger(GameOverSkipTrigger);
    }

    void OnDisable()
    {
        GameOverFX?.StopFX();
    }
    
    void OnEnable()
    {
        GameOverFX?.StartFX();
    }
    public async Task Animate(bool iIsHighScore, PlaygroundManager iPG, CancellationToken iCT)
    {
        // CancellationTokenRegistration ctr = iCT.Register(() => Skip());

        

        // iPG.FinalClapClose();

        // await WaitClosePlayfield(1f, iCT); // full anim
        // if (iCT.IsCancellationRequested)
        // { return; }
    }

    async Task WaitAnimState(string iStateName, float iCompletionFrac, CancellationToken iCT)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(iStateName))
        {
            if (iCT.IsCancellationRequested)
                return;
            await Task.Yield();
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < iCompletionFrac)
        {
            if (iCT.IsCancellationRequested)
                return;
            await Task.Yield();
        }
    }

    async Task WaitClosePlayfield(float iCompletionFrac, CancellationToken iCT)
    {
        // while (!animator.GetCurrentAnimatorStateInfo(0).IsName(LoopDepthStateName))
        // { await Task.Yield(); }
        // while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < iCompletionFrac)
        // { await Task.Yield(); }

        await WaitAnimState(GameOverClosePlayfieldStateName, iCompletionFrac, iCT);
    }
}
