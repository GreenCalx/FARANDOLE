using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MiniGameBankSO", menuName = "Scriptable Objects/MiniGameBankSO")]
public class MiniGameBankSO : ScriptableObject
{
    public List<GameObject> GameBank;
}
