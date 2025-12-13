using UnityEngine;
using TMPro;
using System.Threading;
public class UIGameScore : MonoBehaviour
{
    public int minimumNumberOfFigures = 6;
    public TextMeshProUGUI tmpro_score;
    double previousScore = 0;
    public UIScoreDeltaAnim h_ScoreDeltaAnim;
    CancellationTokenSource cts;
    void Start()
    {
        previousScore = 0;
        string txt_val = "";
        for (int i=0; i < minimumNumberOfFigures; i++)
        {
            txt_val += "0";
        }
        tmpro_score.text = txt_val;
    }
    public void Refresh (double iScore)
    {
        double delta = iScore - previousScore;

        string s_val = iScore.ToString();
        string txt_val = "";
        if (s_val.Length < minimumNumberOfFigures)
        {
            for (int i=s_val.Length-1; i < minimumNumberOfFigures; i++)
            {
                txt_val += "0";
            }
        }

        txt_val += s_val;

        tmpro_score.text = txt_val;
        previousScore = iScore;

        h_ScoreDeltaAnim.PreShowCB.RemoveAllListeners();
        h_ScoreDeltaAnim.PreShowCB.AddListener(()=>h_ScoreDeltaAnim.tmpro_field.text = "+" + delta.ToString());

        cts = new CancellationTokenSource();
        h_ScoreDeltaAnim.DefaultShow(cts.Token);
    }
}
