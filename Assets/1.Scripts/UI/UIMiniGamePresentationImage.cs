using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class UIMiniGamePresentationImage : ManagedAnimation
{
    [Header("UIMiniGamePresentationImage")]
    public UICustomSquircle MiniGameThumbnail;
    
    
    public void SetFromMiniGameDesc(MiniGameSO iMGDesc)
    {
        MiniGameThumbnail.sprite = iMGDesc.thumbNailImg;
    }


}
