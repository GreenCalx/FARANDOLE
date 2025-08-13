using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class MemoryMiniGame : MiniGame
{
    [Header("MemoryMiniGame")]
    public GameObject[] prefabs_card;
    public GameObject prefab_pivot;

    private Pivot pivot;
    public Card[] cards;
    private Card selectedCard;
    private int cardsCount = 1;
    public int[] cardsCounts;
    public float[] rotsSpeeds;
    private int pairs = 0;

    public float distFromCenter = 1.5f;

    public override void Init()
    {
        cardsCount = cardsCounts[MGM.miniGamesDifficulty];
        cards = new Card[cardsCount];

        int[] randomPos = { 1, 2, 3, 4, 5, 6 };

        float stepAngle = Mathf.PI * 2f / cardsCount;
        float angle;
        Shuffle(randomPos);

        pivot = GOBuilder.Create(prefab_pivot)
                    .WithName("pivot")
                    .WithParent(transform)
                    .WithPosition(transform.position)
                    .Build().GetComponent<Pivot>();

        pivot.rotSpeed = rotsSpeeds[MGM.miniGamesDifficulty];

        for (int i = 0; i < cardsCount; i++)
        {
            angle = randomPos[i] * stepAngle;
            cards[i] = GOBuilder.Create(prefabs_card[i / 2])
                            .WithName("card" + i)
                            .WithParent(pivot.transform)
                            .WithPosition(transform.position + distFromCenter * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0))
                            .Build().GetComponent<Card>();

            cards[i].tapCB.AddListener(TurnCard);
            cards[i].index = i;
            PC.AddTapTracker(cards[i]);
        }
        pairs = 0;

    }
    public override void Play()
    {
        IsActiveMiniGame = true;
        IsInPostGame = false;
    }
    public override void Stop()
    {
        deleteCards();
        IsActiveMiniGame = false;
        IsInPostGame = false;
    }
    public override void Win()
    {
        deleteCards();
        MGM.WinMiniGame();
    }
    public override void Lose()
    {
        IsInPostGame = false;
    }
    public override bool SuccessCheck()
    {
        return pairs == cardsCount / 2;
    }

    public void TurnCard(int index)
    {
        if (selectedCard == null)
        {
            selectedCard = cards[index];
        }
        else if (selectedCard == cards[index])
        {
            return;
        }
        else
        {
            if (selectedCard.content == cards[index].content)
            {
                PairMatching(index);
            }
            else
            {
                PairNotMatching(index);
            }
        }
        cards[index].TapEffect();
    }

    public void PairMatching(int index)
    {
        pairs += 1;
        if (SuccessCheck())
        {
            Win();
        }
        PC.EnableTapTracker(selectedCard,false);
        PC.EnableTapTracker(cards[index],false);
        selectedCard.WinCard();
        cards[index].WinCard();
        selectedCard = null;
    }

    public void PairNotMatching(int index)
    {
        selectedCard.HideCard();
        cards[index].HideCard();
        selectedCard = null;
    }


    void Shuffle(int[] array)
    {
        System.Random rng = new System.Random();
        for (int i = cardsCount - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    private void deleteCards() {
    for (int i = 0; i < cardsCount; i++)
        {
            if (cards[i] != null)
            {
                PC.RemoveTapTracker(cards[i]);
                Destroy(cards[i].gameObject); 
            }

        }
    }
}