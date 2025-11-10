using UnityEngine;

public interface IChessMod : IMiniGameMod
{
    public EMiniGameTags AssociatedTag() { return EMiniGameTags.CHESS; }

}