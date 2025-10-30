using UnityEngine;
public interface IMiniGameMod
{
    public EMiniGameTags AssociatedTag() { return EMiniGameTags.NONE; }
    public void ApplyMod();
}