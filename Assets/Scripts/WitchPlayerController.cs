
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
    public Transform moneyCanvas;
    public GameObject maxText;
    private void Awake()
    {
        Instanse = this;
    }
    private void Start()
    {
        Money = SaveControl.Instanse.TryGetMoney() ? SaveControl.Instanse.GetMoney() : _money;
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
