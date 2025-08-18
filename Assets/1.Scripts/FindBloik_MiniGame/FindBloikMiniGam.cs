using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class FindBloik_MiniGame : MiniGame
{
    [Header("FindBloikMiniGame")]
    public GameObject prefabs_unoStickers;
    public GameObject prefab_bloikSticker;

    public int[] numberOfDecoySticker;
    public BouncySticker[] unoStickers;
    public float[] maxStickerSpeed;
    public float[] minStickerSpeed;
    public BouncySticker bloikSticker;

    public int[] stickersHit;

    public float spawnMargin = 0.1f; //Avoid getting stuck
    int lastHitIndex = -1;

    public override void Init()
    {
        int n_spawns = numberOfDecoySticker[MGM.miniGamesDifficulty];
        unoStickers = new BouncySticker[n_spawns];

        bloikSticker = GOBuilder.Create(prefab_bloikSticker)
                    .WithName("bloik")
                    .WithParent(transform)
                    .WithPosition(new Vector2(
                        UnityEngine.Random.Range(PG.bounds.min.x + spawnMargin, PG.bounds.max.x- spawnMargin),
                        UnityEngine.Random.Range(PG.bounds.min.y + spawnMargin, PG.bounds.max.y - spawnMargin)))
                    .Build().GetComponent<BouncySticker>();
        bloikSticker.index = -1;
        bloikSticker.tapCB.AddListener(StickerHit);
        bloikSticker.speed = UnityEngine.Random.Range(minStickerSpeed[MGM.miniGamesDifficulty], maxStickerSpeed[MGM.miniGamesDifficulty]);

        PC.AddTapTracker(bloikSticker);

        for (int i = 0; i < n_spawns; i++)
        {
            unoStickers[i] = GOBuilder.Create(prefabs_unoStickers)
            .WithName("unoStickers" + i)
            .WithParent(transform)
            .WithPosition(new Vector2(
                UnityEngine.Random.Range(PG.bounds.min.x + spawnMargin, PG.bounds.max.x- spawnMargin),
                UnityEngine.Random.Range(PG.bounds.min.y + spawnMargin, PG.bounds.max.y- spawnMargin)))
            .Build().GetComponent<BouncySticker>();
            PC.AddTapTracker(unoStickers[i]);
            unoStickers[i].tapCB.AddListener(StickerHit);
            unoStickers[i].index = i;
            unoStickers[i].speed = UnityEngine.Random.Range(minStickerSpeed[MGM.miniGamesDifficulty], maxStickerSpeed[MGM.miniGamesDifficulty]);
        }
    }

    public override void Play()
    {
        IsActiveMiniGame = true;
        IsInPostGame = false;
    }
    public override void Stop()
    {
        deleteStickers();
        IsActiveMiniGame = false;
        IsInPostGame = false;
    }
    public override void Win()
    {
        foreach (var sticker in unoStickers)
        {
            sticker.gameObject.SetActive(false);
        }
        bloikSticker.Stop();
        MGM.WinMiniGame();
    }

    public override void Lose()
    {
        IsInPostGame = false;
    }
    public override bool SuccessCheck()
    {
        return false;
    }

    public void StickerHit(int index)
    {
        if (index == -1)
        {
            Win();
        }
        else
        {
            lastHitIndex = index;
        }
    }



    private void deleteStickers()
    {
        foreach (var sticker in unoStickers)
        {
            PC.RemoveTapTracker(sticker);
            Destroy(sticker.gameObject);
        }
        PC.RemoveTapTracker(bloikSticker);
        Destroy(bloikSticker.gameObject);
    }

    void Update()
    {
        if (lastHitIndex != -1)
        {
            //Fail
            lastHitIndex = -1;
        }
    }
}