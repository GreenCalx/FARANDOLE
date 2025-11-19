using UnityEngine;
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
                        }
                        if (selectedImage!=null)
                        {
                            selectedImage.ShowLight(false);
                            selectedImage.HideInfoBubble();
                        }
                        selectedImage = newImg;
                        selectedGame = newImg.selfDesc; 
                        selectedImage.ShowLight(true);
                        }
                    );
                MGImages.Add(newImg);
            }

            //SelectGameValidationBtn.enabled      = selectedGame != null;
            SelectGameValidationBtn.interactable = selectedGame != null;
            SelectGameValidationBtn.onClick.AddListener(()=>{});
        }
    }

    void OnEnable()
    {
        Setup();
    }

    void OnDisable()
    {
        foreach(var mg in MGImages)
        {
            Destroy(mg.gameObject);
        }
        MGImages.Clear();
    }
}
