using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SpawnerExtension : MiniGameExtension
{
    public override EMiniGameMods Tag => EMiniGameMods.SPAWNER;
    //
    [SerializeField]
    public SerializableDictionary<SpawnableDef,SpawnerExtensionData> DataOverSpawnables = new SerializableDictionary<SpawnableDef,SpawnerExtensionData>();
}