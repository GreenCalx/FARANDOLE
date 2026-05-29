using UnityEngine;
using System.Collections.Generic;
public interface ISpawnerMod : IMiniGameMod
{
    public EMiniGameMods AssociatedTag() { return EMiniGameMods.SPAWNER; }
}