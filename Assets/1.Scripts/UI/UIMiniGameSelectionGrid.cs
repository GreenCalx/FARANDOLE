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
    public void Setup()
    {
        MGImages = new List<UIMiniGamePresentationImage>();
        if (LoadAll)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            List<LoopHighScore> singlesHighScores = UserData.GetGameModeHighScores(GAME_MODE.SINGLES);
            foreach(MiniGameSO MGDesc in GameData.GetMGBank.GameBank)
            {
                UIMiniGamePresentationImage newImg = GOBuilder.Create(prefab_MGThumbnail)
                                                        .WithParent(transform)
                                                        .BuildAs<UIMiniGamePresentationImage>();
                newImg.SetFromMiniGameDesc(MGDesc);
                newImg.DefaultShow(cts.Token);
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

                LoopHighScore gameHS = null;
                try
                {
                    gameHS = singlesHighScores.Single( e => e.ids[0] == MGDesc.ID);
                } catch (InvalidOperationException exc)
                {
                    gameHS = null;
                }
                
                if (gameHS==null)
                    newImg.ShowRank(LoopRank.I);
                else
                    newImg.ShowRank(gameHS.maxRank);

                MGImages.Add(newImg);
            }

            selectedGame = null;
            selectedImage = null;
            SelectGameValidationBtn.interactable = false;
            SelectGameValidationBtn.UpdateImage();

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
