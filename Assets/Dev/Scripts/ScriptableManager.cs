using UnityEngine;

public class ScriptableManager : MonoBehaviour
{

    public MachineUpgradeData machineUpgradeData;
    public GameData gameData;
    public LevelData LevelData;
    public float timer;
    private void OnEnable()
    {
        EventManager.MoneyUpdated += MoneyUpdated;
        EventManager.GetLevelMachineData += () => LevelData;
        EventManager.GetMachineUpgradeData += () => machineUpgradeData;
        EventManager.GetGameData += () => gameData;

    }

    private void Update()
    {
        timer+= Time.deltaTime;
        if (timer>60)
        {
            //SundaySDK.Tracking.TrackLevelStart(SceneManager.GetActiveScene().buildIndex);
            timer = 0;
        }
    }

    private void MoneyUpdated()
    {
        gameData.Save();
    }

    private void OnDisable()
    {
        EventManager.MoneyUpdated -= MoneyUpdated;
        EventManager.GetGameData -= () => gameData;
        EventManager.GetLevelMachineData -= () => LevelData;
        EventManager.GetMachineUpgradeData = () => machineUpgradeData;
    }


    private void Start()
    {
        //SundaySDK.Tracking.TrackLevelStart(SceneManager.GetActiveScene().buildIndex);
        gameData.Load();
        LevelData.Load();
        EventManager.StartGame();
        //EventManager.SetGymMachines.Invoke();

    }
}
