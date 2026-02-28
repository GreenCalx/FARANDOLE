using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIArtefactImage : UISelectableImage
{
    [Header("UIArtefactImage")]
    public UICustomSquircle ArtefactThumbnail;
    public Sprite defaultSprite;

    [Header("Info Display")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public UIButton selectButton;

    [Header("Modifiers Display")]
    public RectTransform modifiersContainer;
    public GameObject modifierTagPrefab;

    [Header("Data")]
    public Artefact artefact;

    private List<GameObject> instantiatedModTags;

    protected override void Awake()
    {
        base.Awake();
        instantiatedModTags = new List<GameObject>();

        if (selectButton != null)
        {
            selectButton.clickCallback.AddListener(OnSelectButtonClicked);
        }
    }

    public void SetFromArtefact(Artefact iArtefact)
    {
        artefact = iArtefact;

        if (iArtefact?.data == null)
            return;

        if (ArtefactThumbnail != null)
        {
            if (iArtefact.data.icon != null)
                ArtefactThumbnail.sprite = iArtefact.data.icon;
            else if (defaultSprite != null)
                ArtefactThumbnail.sprite = defaultSprite;
        }

        if (nameText != null)
            nameText.text = iArtefact.data.name;

        if (descriptionText != null)
            descriptionText.text = iArtefact.data.descriptiion;

        RefreshModifierTags();
    }

    private void RefreshModifierTags()
    {
        ClearModifierTags();

        if (modifiersContainer == null || modifierTagPrefab == null || artefact == null)
            return;

        if (artefact.mods != null)
        {
            foreach (IMiniGameMod mod in artefact.mods)
            {
                GameObject tagObj = GOBuilder.Create(modifierTagPrefab)
                    .WithParent(modifiersContainer)
                    .Build();

                TextMeshProUGUI tagText = tagObj.GetComponent<TextMeshProUGUI>();
                if (tagText != null)
                {
                    tagText.text = $"[{mod.AssociatedTag()}]";
                }

                instantiatedModTags.Add(tagObj);
            }
        }
    }

    private void ClearModifierTags()
    {
        foreach (GameObject tag in instantiatedModTags)
        {
            if (tag != null)
                Destroy(tag);
        }
        instantiatedModTags.Clear();
    }

    private void OnSelectButtonClicked()
    {
        Select();
    }

    public void EnableSelection()
    {
        if (selectButton != null)
        {
            selectButton.interactable = true;
            selectButton.enabled = true;
        }
    }

    public void DisableSelection()
    {
        if (selectButton != null)
        {
            selectButton.interactable = false;
            selectButton.enabled = false;
        }
    }

    public override void Select()
    {
        base.Select();
        UpdateLightColor(GameData.GetUITheme.thumbnailSuccessLightColor);
    }

    public override void Deselect()
    {
        base.Deselect();
    }

    public Artefact GetArtefact()
    {
        return artefact;
    }

    private void OnDestroy()
    {
        ClearModifierTags();
    }
}
