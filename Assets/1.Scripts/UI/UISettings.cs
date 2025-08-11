using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISettings : MonoBehaviour
{
    public Button deleteScoresBtn;

    void Start()
    {
        deleteScoresBtn?.onClick.AddListener(() => DeleteSaveFile());
    }

    void DeleteSaveFile()
    {
        if (UserData.DeleteHighScoresData())
        {
            Debug.Log("High Scores deleted");
            UserData.userHighScores = new UserHighScores();
        }
    }
}
