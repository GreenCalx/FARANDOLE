using UnityEngine;

public interface IArcadeMod : IMiniGameMod
{
    public EMiniGameTags AssociatedTag() { return EMiniGameTags.ARCADE; }

}