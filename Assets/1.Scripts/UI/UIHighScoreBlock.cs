using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

public class UIHighScoreBlock : MonoBehaviour
{
    public TextMeshProUGUI position;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI scoreValue;
    public UICustomSquircle Frame;
    public Color baseFrameColor;
    public Color localPlayerFrameColor;
    public void Setup(LeaderboardEntry iEntry)
    {
        position.text = "#"+(iEntry.Rank+1).ToString(); // rank start at 0
        playerName.text = iEntry.PlayerName;
        scoreValue.text = iEntry.Score.ToString();

        if (iEntry.PlayerName == SessionData.UserName)
        {
            Frame.color = localPlayerFrameColor;
        } else
        {
            Frame.color = baseFrameColor;
        }
    }
}
