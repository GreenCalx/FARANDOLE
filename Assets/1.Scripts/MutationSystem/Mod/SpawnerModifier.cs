using System;
using System.Collections;
using System.Collections.Generic;

public class SpawnerModifier : IMiniGameMod
{
    public float multiplier = 1.5f;

    public EMiniGameMods AssociatedTag()
        => EMiniGameMods.SPAWNER;

    public void Apply(MiniGameLoopSocket socket)
    {
        var spawner = socket.inst_miniGame.GetExtension<SpawnerExtension>();

        if (spawner == null)
            return;

        var rank = socket.inst_miniGame.MGM.MGLoop.rank;
        // Do stuff
    }
}
