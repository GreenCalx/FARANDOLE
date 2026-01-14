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
        var spawner = socket.inst_miniGame
                  .GetExtension<IMiniGameExtension<int>>();

        if (spawner == null)
            return;

        var rank = socket.inst_miniGame.MGM.MGLoop.rank;

        // Apply multiplicative mutation
        foreach (var def in GetAllSpawnableDefs())
        {
            int baseValue = spawner.Get(def, rank);
            int modified = (int)Math.Floor(baseValue * multiplier);
            spawner.Set(def, rank, modified);
        }
    }

    IEnumerable<SpawnableDef> GetAllSpawnableDefs()
    {
        // from bank / registry / known list
        yield break;
    }
}
