using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UIMutationPanel : MonoBehaviour
{
    [Header("Replacement Mini Games")]
    public RectTransform h_ReplacementPool;
    public GameObject prefab_MiniGameImage;
    private List<UIMiniGamePresentationImage> replacementImages = new List<UIMiniGamePresentationImage>();
    private UIMiniGamePresentationImage selectedReplacement;

    [Header("Artefact Selection")]
    public RectTransform h_ArtefactPool;
    public GameObject prefab_ArtefactImage;
    private List<UIArtefactImage> artefactImages = new List<UIArtefactImage>();
    private UIArtefactImage selectedArtefact;

    [Header("UI Controls")]
    public UIButton validateMutationButton;
    public TextMeshProUGUI InfoTextField;
    public TextMeshProUGUI stepIndicatorText;

    [Header("Animation")]
    public int showDelayBetweenItemsMs = 80;

    [Header("Events")]
    public UnityEvent OnMutationValidated;

    private MiniGameLoop currentLoop;
    private UILoopPresentationAnim loopPresentation;
    private UIMiniGamePresentationImage selectedToReplace;
    private CancellationTokenSource animationCTS;

    private void Awake()
    {
        if (validateMutationButton != null)
        {
            validateMutationButton.clickCallback.AddListener(OnValidateMutationClicked);
            validateMutationButton.interactable = false;
        }

        OnMutationValidated ??= new UnityEvent();
    }

    public void Setup(MiniGameLoop iLoop, UILoopPresentationAnim iLoopPresentation, List<Artefact> iArtefactChoices)
    {
        currentLoop = iLoop;
        loopPresentation = iLoopPresentation;

        ClearAll();

        SetupLoopPresentationSelection();
        SetupReplacementChoices();
        SetupArtefactChoices(iArtefactChoices);

        UpdateValidateButton();
        UpdateStepIndicator();
    }

    private void SetupLoopPresentationSelection()
    {
        if (loopPresentation?.UIImages == null)
            return;

        loopPresentation.EnableThumbnailSelection();

        foreach (var img in loopPresentation.UIImages)
        {
            if (img != null)
            {
                img.onSelected.AddListener(OnCurrentLoopMiniGameSelected);
            }
        }
    }

    private void ClearLoopPresentationSelection()
    {
        if (loopPresentation?.UIImages == null)
            return;

        loopPresentation.DisableThumbnailSelection();

        foreach (var img in loopPresentation.UIImages)
        {
            if (img != null)
            {
                img.onSelected.RemoveListener(OnCurrentLoopMiniGameSelected);
                img.Deselect();
            }
        }
    }

    private void SetupReplacementChoices()
    {
        if (h_ReplacementPool == null || prefab_MiniGameImage == null || currentLoop == null)
            return;

        List<MiniGameSO> currentLoopDescriptors = new List<MiniGameSO>();
        for (int i = 0; i < currentLoop.count; i++)
        {
            MiniGame mg = currentLoop.At(i);
            if (mg?.descriptor != null)
                currentLoopDescriptors.Add(mg.descriptor);
        }

        List<MiniGameSO> availableGames = GameData.GetMGBank.GameBank
            .Where(mg => !currentLoopDescriptors.Contains(mg))
            .ToList();

        int count = Mathf.Min(3, availableGames.Count);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableGames.Count);
            MiniGameSO selected = availableGames[randomIndex];
            availableGames.RemoveAt(randomIndex);

            UIMiniGamePresentationImage img = GOBuilder.Create(prefab_MiniGameImage)
                .WithParent(h_ReplacementPool)
                .BuildAs<UIMiniGamePresentationImage>();

            img.SetFromMiniGameDesc(selected);
            img.onSelected.AddListener(OnReplacementMiniGameSelected);
            img.EnableSelection();
            img.gameObject.SetActive(true);
            replacementImages.Add(img);
        }
    }

    private void SetupArtefactChoices(List<Artefact> iArtefacts)
    {
        if (h_ArtefactPool == null || prefab_ArtefactImage == null)
            return;

        foreach (Artefact artefact in iArtefacts)
        {
            UIArtefactImage img = GOBuilder.Create(prefab_ArtefactImage)
                .WithParent(h_ArtefactPool)
                .BuildAs<UIArtefactImage>();

            img.SetFromArtefact(artefact);
            img.onSelected.AddListener(OnArtefactSelected);
            img.EnableSelection();
            img.gameObject.SetActive(true);
            artefactImages.Add(img);
        }
    }

    private async UniTaskVoid PlayShowAnimations()
    {
        animationCTS?.Cancel();
        animationCTS = new CancellationTokenSource();
        CancellationToken ct = animationCTS.Token;

        foreach (var img in replacementImages)
        {
            if (ct.IsCancellationRequested) return;
            if (img.m_Animator != null)
                img.DefaultShow(ct).Forget();
            await UniTask.Delay(showDelayBetweenItemsMs, cancellationToken: ct);
        }

        foreach (var img in artefactImages)
        {
            if (ct.IsCancellationRequested) return;
            if (img.m_Animator != null)
                img.DefaultShow(ct).Forget();
            await UniTask.Delay(showDelayBetweenItemsMs, cancellationToken: ct);
        }
    }

    private void OnCurrentLoopMiniGameSelected(UISelectableImage iSelected)
    {
        if (iSelected is not UIMiniGamePresentationImage selected)
            return;

        if (selectedToReplace != null && selectedToReplace != selected)
        {
            selectedToReplace.Deselect();
        }

        if (selectedToReplace == selected)
        {
            selectedToReplace.Deselect();
            selectedToReplace = null;
        }
        else
        {
            selectedToReplace = selected;
            selected.UpdateLightColor(GameData.GetUITheme.thumbnailFailLightColor);
            UpdateInfoText($"Replace: {selected.selfDesc?.name}");
        }

        UpdateValidateButton();
        UpdateStepIndicator();
    }

    private void OnReplacementMiniGameSelected(UISelectableImage iSelected)
    {
        if (iSelected is not UIMiniGamePresentationImage selected)
            return;

        if (selectedReplacement != null && selectedReplacement != selected)
        {
            selectedReplacement.Deselect();
        }

        if (selectedReplacement == selected)
        {
            selectedReplacement.Deselect();
            selectedReplacement = null;
        }
        else
        {
            selectedReplacement = selected;
            UpdateInfoText($"Replacement: {selected.selfDesc?.name}");
        }

        UpdateValidateButton();
        UpdateStepIndicator();
    }

    private void OnArtefactSelected(UISelectableImage iSelected)
    {
        if (iSelected is not UIArtefactImage selected)
            return;

        if (selectedArtefact != null && selectedArtefact != selected)
        {
            selectedArtefact.Deselect();
        }


        if (selectedArtefact == selected)
        {
            selectedArtefact.Deselect();
            selectedArtefact = null;
            
        }
        else
        {
            selectedArtefact = selected;  
        }


        if (selected.artefact?.data != null)
        {
            UpdateInfoText($"Artefact: {selected.artefact.data.name}");
        }

        UpdateValidateButton();
        UpdateStepIndicator();
    }

    private void UpdateInfoText(string iText)
    {
        if (InfoTextField != null)
        {
            InfoTextField.text = iText;
        }
    }

    private void UpdateStepIndicator()
    {
        if (stepIndicatorText == null)
            return;

        int steps = 0;
        if (selectedToReplace != null) steps++;
        if (selectedReplacement != null) steps++;
        if (selectedArtefact != null) steps++;

        if (steps>3)
            stepIndicatorText.text = "MUTATE";
        else
            stepIndicatorText.text = $"{steps}/3";
    }

    private void UpdateValidateButton()
    {
        if (validateMutationButton == null)
            return;

        bool allSelected = selectedToReplace != null
                        && selectedReplacement != null
                        && selectedArtefact != null;

        validateMutationButton.interactable = allSelected;
        validateMutationButton.UpdateImage();
    }

    private void OnValidateMutationClicked()
    {
        Debug.Log($"[MutationPanel] Validate clicked! toReplace={selectedToReplace != null}, replacement={selectedReplacement != null}, artefact={selectedArtefact != null}");
        if (selectedToReplace == null || selectedReplacement == null || selectedArtefact == null)
            return;

        ApplyMiniGameReplacement();
        ApplyArtefactGain();

        Debug.Log($"Mutation validated: Replaced {selectedToReplace.selfDesc?.name} with {selectedReplacement.selfDesc?.name}, gained {selectedArtefact.artefact?.data?.name}");

        OnMutationValidated?.Invoke();
        HidePanel();
    }

    private void ApplyMiniGameReplacement()
    {
        if (selectedToReplace?.selfDesc == null || selectedReplacement?.selfDesc == null)
            return;

        MiniGameSO toReplace = selectedToReplace.selfDesc;
        MiniGameSO replacement = selectedReplacement.selfDesc;

        for (int i = 0; i < currentLoop.count; i++)
        {
            if (currentLoop.At(i)?.descriptor == toReplace)
            {
                MiniGameLoopSocket socket = currentLoop.sockets[i];

                if (socket.inst_miniGame != null)
                {
                    Destroy(socket.inst_miniGame.gameObject);
                }

                GameObject newMG = GOBuilder.Create(replacement.prefab_MiniGame).Build();
                MiniGame asMG = newMG.GetComponent<MiniGame>();
                if (asMG != null)
                {
                    asMG.MGM = GameManager.Get.MGM;
                    asMG.PC = GameManager.Get.PC;
                    asMG.PG = GameManager.Get.PG;
                    socket.inst_miniGame = asMG;
                    asMG.Init();
                    newMG.SetActive(false);
                }

                Debug.Log($"Replaced mini-game at index {i}: {toReplace.name} -> {replacement.name}");
                break;
            }
        }
    }

    private void ApplyArtefactGain()
    {
        Artefact artefact = selectedArtefact?.GetArtefact();
        if (artefact?.data != null && GameManager.Get?.playerData != null)
        {
            GameManager.Get.playerData.GainArtefact(artefact.data);
            Debug.Log($"Artefact added to player bag: {artefact.data.name}");
        }
    }

    private void ClearAll()
    {
        ClearLoopPresentationSelection();

        foreach (var img in replacementImages)
        {
            if (img != null)
            {
                img.onSelected.RemoveListener(OnReplacementMiniGameSelected);
                Destroy(img.gameObject);
            }
        }
        replacementImages.Clear();

        foreach (var img in artefactImages)
        {
            if (img != null)
            {
                img.onSelected.RemoveListener(OnArtefactSelected);
                Destroy(img.gameObject);
            }
        }
        artefactImages.Clear();

        selectedToReplace = null;
        selectedReplacement = null;
        selectedArtefact = null;
    }

    public void HidePanel()
    {
        animationCTS?.Cancel();
        ClearLoopPresentationSelection();
        gameObject.SetActive(false);
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        PlayShowAnimations().Forget();
    }

    private void OnDestroy()
    {
        animationCTS?.Cancel();

        if (validateMutationButton != null)
        {
            validateMutationButton.clickCallback.RemoveListener(OnValidateMutationClicked);
        }

        ClearAll();
    }
}
