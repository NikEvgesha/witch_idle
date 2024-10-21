using UnityEngine;
using YG;
public class WitchPlayerController : MonoBehaviour
{
    public static WitchPlayerController Instanse;
    [SerializeField]
    private int _money;
    [SerializeField]
    private int _experience;
    [SerializeField]
    private int _level;
    [SerializeField]
    private LevelSteps _levelSteps;

    public int Money
    {
        get { return _money; }
        set
        {
            _money = value;
            EventManager.MoneyChange(_money);
            YandexGame.savesData.money = _money;
            // Функция изменения UI
            // Функция сохранения значения
        }
    }
    public int Experience
    {
        get { return _experience; }
        set
        {
            _experience = value;
            CheckNextLevel();
            EventManager.ExperienceChange(_experience, _levelSteps.GetExperience(_level));
            YandexGame.savesData.Experience = _experience;
            // Функция изменения UI
            // Функция сохранения значения
        }
    }
    private void CheckNextLevel()
    {
        if (_experience >= _levelSteps.GetExperience(_level))
        {
            _experience -= _levelSteps.GetExperience(_level);
            PlayerLevel++;
            CheckNextLevel();
        }
    }
    public int PlayerLevel
    {
        get { return _level; }
        set
        {
            _level = value;
            EventManager.PlayerLevelChange(_level);
            YandexGame.savesData.PlayerLevel = _level;
            // Функция изменения UI
            // Функция сохранения значения
        }
    }
    public int money;
    public Transform moneyCanvas;
    public GameObject maxText;
    private void Awake()
    {
        YandexGame.GetDataEvent += LoadMoney;
        YandexGame.GetDataEvent += LoadLevel;
        Instanse = this;
    }
    private void LoadMoney()
    {
        Money = YandexGame.savesData.money;
    }
    private void LoadLevel()
    {
        Experience = YandexGame.savesData.Experience;
        PlayerLevel = YandexGame.savesData.PlayerLevel;
    }
    private void Start()
    {

        //Money = SaveControl.Instanse.TryGetMoney() ? SaveControl.Instanse.GetMoney() : _money;
    }
    public bool HaveMoney() 
    { 
        if (Money <= 0)
        {
            //Плашку нет денег
            return false;
        }

        return true;
    }
    public bool HaveMoney(int price)
    {
        if (Money < price)
        {
            //Плашку нет денег
            return false;
        }

        return true;
    }
}
