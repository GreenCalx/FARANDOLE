using UnityEngine;

public interface IBallMod : IMiniGameMod
{
    public EMiniGameMods AssociatedTag() { return EMiniGameMods.BALL; }

}