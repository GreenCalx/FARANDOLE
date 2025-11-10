using UnityEngine;

public interface ITheaterMod : IMiniGameMod
{
    public EMiniGameTags AssociatedTag() { return EMiniGameTags.THEATER; }

}