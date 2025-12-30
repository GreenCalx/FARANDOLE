using UnityEngine;
using System.Collections.Generic;
public enum SCREEN_ORIENTATION
{
    PORTRAIT = 0,
    LANDSCAPE = 1,
    BOTH = 2
}

[CreateAssetMenu(fileName = "MiniGameSO", menuName = "Scriptable Objects/MiniGameSO")]
public class MiniGameSO : ScriptableObject
{
    public byte ID;
    public string name;
    public string goal;
    public Sprite thumbNailImg;
    public GameObject prefab_MiniGame;
    public EMiniGameFamily family;
    public List<EMiniGameMods> compatibleMods;
    public SCREEN_ORIENTATION orientationRequirement = SCREEN_ORIENTATION.PORTRAIT;
}
