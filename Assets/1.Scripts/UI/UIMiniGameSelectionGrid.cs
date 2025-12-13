using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
public class UIMiniGameSelectionGrid : MonoBehaviour
{
    public bool LoadAll = true;

    public GameObject prefab_MGThumbnail;
    List<UIMiniGamePresentationImage> MGImages;
    public MiniGameSO selectedGame;
    public UIMiniGamePresentationImage selectedImage;
    public UIButton SelectGameValidationBtn;
    CancellationTokenSource cts;

    public void Setup(UICompletionBar iCompletionBar = null)
    {
        cts = new CancellationTokenSource();
        MGImages = new List<UIMiniGamePresentationImage>();
        if (LoadAll)
        {
            List<LoopHighScore> singlesHighScores = UserData.GetGameModeHighScores(GAME_MODE.SINGLES);
            foreach(MiniGameSO MGDesc in GameData.GetMGBank.GameBank)
            {
                UIMiniGamePresentationImage newImg = GOBuilder.Create(prefab_MGThumbnail)
                                                        .WithParent(transform)
                                                        .BuildAs<UIMiniGamePresentationImage>();
                newImg.SetFromMiniGameDesc(MGDesc);
                newImg.infoBubbleBtn.onClick.AddListener(
                    ()=> {
                        if (selectedImage == newImg)
                        {
                            selectedImage.ShowLight(false);
                            selectedImage.HideInfoBubble();
                            selectedGame = null;
                            selectedImage = null;
                            return;
                        }
                        else if (selectedImage!=null)
                        {
                            selectedImage.ShowLight(false);
                            selectedImage.HideInfoBubble();
                        }
                        selectedImage = newImg;
                        selectedGame = newImg.selfDesc; 
                        selectedImage.ShowLight(true);
                        }
                    );

                // Init From Savefile
                if (SinglesModeProgress.TryLoadLocalProgress(MGDesc.ID, out var sd))
                {
                    newImg.ShowRank(sd.maxRank);
                    if (sd.completed)
                    {
                        newImg.MASK.color = Color.yellow;
                        newImg.MASK.SetVerticesDirty();
                        if (iCompletionBar!=null)
                        {
                            iCompletionBar.Increment();
                        }
                    }
                } else
                {
                    newImg.ShowRank(LoopRank.I);
                }

                // Finalize
                MGImages.Add(newImg);
            }

            selectedGame = null;
            selectedImage = null;
            SelectGameValidationBtn.interactable = false;
            SelectGameValidationBtn.UpdateImage();


        }
    }

    public void ShowAll()
    {
        foreach(UIMiniGamePresentationImage img in MGImages )
        {
            img.DefaultShow(cts.Token);
        }
    }


    public void Clear()
    {
        foreach(var mg in MGImages)
        {
            Destroy(mg.gameObject);
        }
        MGImages.Clear();
        selectedImage = null;
        selectedGame = null;
    }

    void Update()
    {
        SelectGameValidationBtn.interactable = selectedGame != null;
        SelectGameValidationBtn.UpdateImage();
    }

    void OnEnable()
    {
        //Setup();
    }

    void OnDisable()
    {
        // CLear();
    }
}
