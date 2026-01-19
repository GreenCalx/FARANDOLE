using UnityEngine;
public interface IMiniGameMod
{
    public EMiniGameMods AssociatedTag() { return EMiniGameMods.NONE; }
    public void Apply(MiniGameLoopSocket iMGSocket);
}