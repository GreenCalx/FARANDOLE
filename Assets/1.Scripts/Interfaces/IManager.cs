using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManager
{
    public void Init(GameManager iGameManager);
    public bool IsReady();
}
