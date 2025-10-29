using UnityEngine;

public interface IDogMod : IMiniGameMod
{
    public EMiniGameTags AssociatedTag() { return EMiniGameTags.DOG; }

}