using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpawnableDef", menuName = "MiniGameExtensions/SpawnableDef")]
[Serializable]
public class SpawnableDef : ScriptableObject
{
    [Tooltip("Prefab that implements ISpawnable")]
    public GameObject prefab;
}
