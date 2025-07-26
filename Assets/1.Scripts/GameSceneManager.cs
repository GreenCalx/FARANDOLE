using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour, IManager
{
    public List<Scene> createdScenes;

    #region IManager
    public void Init(GameManager iGameManager)
    {

    }
    public bool IsReady()
    {
        return true;
    }
    #endregion

    void OnDestroy()
    {
        foreach (Scene s in createdScenes)
        {
            SceneManager.UnloadSceneAsync(s);
        }
    }
}
