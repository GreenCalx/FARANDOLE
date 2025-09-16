using UnityEngine;

[CreateAssetMenu(fileName = "LoopRankSO", menuName = "Scriptable Objects/LoopRankSO")]
public class LoopRankSO : ScriptableObject
{
    public Sprite rankMedal_default;
    public Sprite rankMedal_I;
    public Sprite rankMedal_II;
    public Sprite rankMedal_III;
    public Sprite rankMedal_S;
    public Sprite rankMedal_M;

    public Sprite GetImageFromRank(LoopRank iRank)
    {
        switch (iRank)
        {
            case LoopRank.Z:
                return rankMedal_default;
            case LoopRank.I:
                return rankMedal_I;
            case LoopRank.II:
                return rankMedal_II;
            case LoopRank.III:
                return rankMedal_III;
            case LoopRank.S:
                return rankMedal_S;
            case LoopRank.M:
                return rankMedal_M;
            default:
                Debug.LogWarning("tryRankUp:: Unkown loop rank : " + iRank.ToString());
                return rankMedal_default;
        }
    }
}
