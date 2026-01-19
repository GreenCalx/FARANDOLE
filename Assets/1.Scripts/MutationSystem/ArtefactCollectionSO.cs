using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArtefactCollectionSO", menuName = "Scriptable Objects/ArtefactCollectionSO")]
public class ArtefactCollectionSO : ScriptableObject
{
    public List<ArtefactSO> artefacts;
}
