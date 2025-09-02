using UnityEngine;
using System.Collections.Generic;
public enum SCREEN_ORIENTATION
{
    PORTRAIT = 0,
    LANDSCAPE = 1,
    BOTH = 2
}

public enum MINIGAME_TAGS
{
    ANY = 0,
    AGILITY = 1,
    REFLEX = 2,
    BRAIN = 3,
    SCIENCE = 4
}

[CreateAssetMenu(fileName = "MiniGameSO", menuName = "Scriptable Objects/MiniGameSO")]
public class MiniGameSO : ScriptableObject
{
    public byte ID;
    public string name;
    public string goal;
    public Sprite thumbNailImg;
    public GameObject prefab_MiniGame;
    public List<MINIGAME_TAGS> tags;
    public SCREEN_ORIENTATION orientationRequirement = SCREEN_ORIENTATION.PORTRAIT;
}
