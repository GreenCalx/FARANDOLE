using UnityEngine;

public interface IRegularMod : IMiniGameMod
{
    public EMiniGameTags AssociatedTag() { return EMiniGameTags.REGULAR; }

}