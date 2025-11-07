using UnityEngine;

public enum EUITag
{
    NONE = 0,
    MenuBtn = 1,
    MainBtn = 2,
    ActionBtn = 3,
    ThumbnailBtn =4
}

public class UITag : MonoBehaviour
{
    public EUITag tag;
}
