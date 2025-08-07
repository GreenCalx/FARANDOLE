using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UITitle : MonoBehaviour
{
    public Button randomSeedBtn;
    public Button dailySeedBtn;
    public Button sprintBtn;
    public string GameScene = "Game";
    void Start()
    {
        randomSeedBtn.onClick.AddListener(() => StartRandomSeed());
        dailySeedBtn.onClick.AddListener(() => StartDailySeed());
        sprintBtn?.onClick.AddListener(() => StartSprint());
    }

    void StartRandomSeed()
    {
        GameData.Get.PickGameMode(GAME_MODE.RANDOM_SEED);
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
    }
    void StartDailySeed()
    {
        GameData.Get.PickGameMode(GAME_MODE.DAILY_SEED);
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
    }
    void StartSprint()
    {
        GameData.Get.PickGameMode(GAME_MODE.SPRINT);
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
    }
}
