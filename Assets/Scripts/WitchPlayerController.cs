
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WitchPlayerController : MonoBehaviour
{
    [SerializeField]
    private int _money;
    public int Money
    {
        get { return _money; }
        set
        {
            _money = value;
            EventManager.MoneyChange(_money);
            SaveControl.Instanse.SaveMoney(_money);
            // Функция изменения UI
            // Функция сохранения значения
        }
    }

    public static WitchPlayerController Instanse;



    public int money;
    //public GameObject moneyTextPrefab;
    public Transform moneyCanvas;
    public GameObject maxText;
    private void Awake()
    {
        Instanse = this;
    }
    private void Start()
    {
        Money = SaveControl.Instanse.TryGetMony() ? SaveControl.Instanse.GetMoney() : _money;
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

    private void OnEnable()
    {
        //EventManager.MoneyUpdated += MoneyUpdated;
        //EventManager.PlayerCollectedMoney += PlayerCollectedMoney;
    }

    private void MoneyUpdated()
    {
       // money = EventManager.GetGameData().totalMoneyAmount;
    }

    private void OnDisable()
    {
       // EventManager.MoneyUpdated -= MoneyUpdated;
        //EventManager.PlayerCollectedMoney -= PlayerCollectedMoney;
    }

    /*public void StackIsMax(bool activate)
    {
        maxText.SetActive(activate);
    }*/

    /*private void PlayerCollectedMoney(float amount)
    {
        var money = Instantiate(moneyTextPrefab, Vector3.zero, quaternion.identity, moneyCanvas);
        money.transform.localPosition = Vector3.zero;
        money.GetComponent<TextMeshProUGUI>().text = "$" + amount;
        money.transform.DOLocalMoveY(money.transform.localPosition.y + 2, .5f).OnComplete(() =>
        {
            Destroy(money.gameObject);
        });
    }*/

}
