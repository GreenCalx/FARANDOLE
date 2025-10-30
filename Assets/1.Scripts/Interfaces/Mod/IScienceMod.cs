using UnityEngine;

public interface IScienceMod : IMiniGameMod
{
    public EMiniGameTags AssociatedTag() { return EMiniGameTags.SCIENCE; }

}