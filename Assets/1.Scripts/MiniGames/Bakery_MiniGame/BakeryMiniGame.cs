using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
public class BakeryMiniGame : MiniGame, IDogFamily
{
    private readonly string kTargetCoins = "TargetCoins";
    public Slider progressBar; 
    public ShopOrderManager m_ShopOrderManager;
    public CustomerManager m_CustomerManager;
    public UIBakeryShop UIShopGame;
    public float[] customerSpawnIntervals;
    public GameObject[] spawnablePerRank;
    
    Coroutine customerSpawnCo;
    
    int currentCoins;
    int targetCoins;

    public override void Init()
    {
        base.Init();
        m_ShopOrderManager.Setup();
    }
    public override void Reset()
    {
        base.Reset();

        ITargetObjectiveExtension targetExt = GetExtension<ITargetObjectiveExtension>();
        ISpawnerExtension spawnerExt = GetExtension<ISpawnerExtension>();

        m_CustomerManager.Setup(spawnablePerRank[(int)MGM.MGLoop.rank]);
        //m_CustomerManager.Setup(spawnerExt?.Get());
        
        m_ShopOrderManager.Reset();
        m_ShopOrderManager.Setup();
        
        m_ShopOrderManager.OnOrderSuccessfullCB.AddListener(
            (coins)=>
            {
                currentCoins += coins;
                progressBar.value = currentCoins;
                if (SuccessCheck())
                    Win();
            }
            );
        

        currentCoins = 0;
        targetCoins = targetExt?.Get(kTargetCoins, MGM.MGLoop.rank) ?? 1;
        progressBar.minValue = 0f;
        progressBar.maxValue = targetCoins;
        progressBar.value = currentCoins;

        UIShopGame.gameObject.SetActive(false);
    }
    public override void Play()
    {
        base.Play();

        if (customerSpawnCo!=null)
        {
            StopCoroutine(customerSpawnCo);
            customerSpawnCo = null;
        }
        customerSpawnCo = StartCoroutine(SpawnCustomers());
        UIShopGame.gameObject.SetActive(true);
    }
    public override void Stop()
    {
        base.Stop();
        m_ShopOrderManager.Reset();
        m_CustomerManager.ClearCustomers();
        UIShopGame.gameObject.SetActive(false);
    }
    public override void Win()
    {
        base.Win();
        m_ShopOrderManager.CloseShop();
       // m_CustomerManager.AllLeaveShop();
    }
    public override void Lose()
    {
        base.Lose();
    }
    public override bool SuccessCheck()
    {
        return currentCoins >= targetCoins;
    }

    public override async UniTask IntroAnim(CancellationToken token)
    {
        return;
    }
    
    #region MODS
    public void ApplyMod()
    {

    }
    #endregion

    IEnumerator SpawnCustomers()
    {
        float elapsed = 99f;
        while (!IsInPostGame)
        {
            elapsed += Time.deltaTime;
            if (elapsed > customerSpawnIntervals[(int)MGM.MGLoop.rank])
            {
                if (m_CustomerManager.EnterShop())
                {
                    elapsed = 0f;
                }
            }
            yield return null;
        }
    }
}
