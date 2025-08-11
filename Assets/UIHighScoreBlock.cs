using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHighScoreBlock : MonoBehaviour
{
    public GameObject prefab_miniGameMiniature;
    public TextMeshProUGUI scoreValue;
    public TextMeshProUGUI gameModeValue;
    public Button challengeScoreBtn;
    public RectTransform handle_Miniatures;
    List<UIMiniGameMiniature> mgMiniatures;
    LoopHighScore LHS;
    [HideInInspector]
    public LoopHighScore associatedLHS
    {
        set
        {
            LHS = value;
            BuildFromLHS();
        }
        get
        {
            return LHS;
        }
    }

    void BuildFromLHS()
    {
        
        scoreValue.text = LHS.score.ToString();
        gameModeValue.text = LHS.gameMode.ToString();
        // Ids
        mgMiniatures = new List<UIMiniGameMiniature>(LHS.ids.Length);
        foreach (byte id in LHS.ids)
        {
            UIMiniGameMiniature mini = GOBuilder.Create(prefab_miniGameMiniature)
                                        .WithName("mini_" + id)
                                        .WithParent(handle_Miniatures)
                                        .Build().GetComponent<UIMiniGameMiniature>();
            mini.mgIDLabel.text = id.ToString();
            mgMiniatures.Add(mini);
        }
    }
}
