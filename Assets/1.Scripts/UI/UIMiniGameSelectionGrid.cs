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
                MGImages.Add(newImg);
            }

            //SelectGameValidationBtn.enabled      = selectedGame != null;
            selectedGame = null;
            selectedImage = null;
            SelectGameValidationBtn.interactable = false;
            SelectGameValidationBtn.UpdateImage();
            //SelectGameValidationBtn.onClick.AddListener(()=>{});
        }
    }

    void Update()
    {
        bool mg_selected = selectedGame != null;
        if (mg_selected && !SelectGameValidationBtn.interactable)
        {
            SelectGameValidationBtn.interactable = true;
            SelectGameValidationBtn.UpdateImage();
        } else if ((!mg_selected) && SelectGameValidationBtn.interactable)
        {
            SelectGameValidationBtn.interactable = false;
            SelectGameValidationBtn.UpdateImage();
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
        selectedImage = null;
        selectedGame = null;
    }
}
