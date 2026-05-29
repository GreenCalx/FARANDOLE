using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SpawnableDef", menuName = "Scriptable Objects/SpawnableDef")]
[Serializable]
public class SpawnableDef : ScriptableObject
{
    [Tooltip("Prefab that implements ISpawnable")]
    public GameObject prefab;
}
