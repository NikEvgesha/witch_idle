using Sirenix.Utilities;
using System;
using Unity.Mathematics;
using UnityEngine;
using YG;

public class InteractionObject : MonoBehaviour
{
    [SerializeField]
    private string _uniqueName;
    [SerializeField]
    private PurchasedState _purchasedState;
    [SerializeField]
    private int _firstPrice;
    [SerializeField]
    private BuyStruct _buyStruct;
    [SerializeField]
    private WorkStruct _workStruct;
    [SerializeField]
    private LockStruct _lockStruct;
    [SerializeField]
    private float _SpeedFill = 1;
    [SerializeField]
    private string _stringGUID;
    [SerializeField]
    private int _levelToUnlock;

    private int _priceIn;
    private int _price
    {
        get
        {
            return _priceIn;
        }
        set
        {
            _priceIn = value;
            YandexGame.savesData.IOPrice[_saveIndex] = _price;
            ChangeUIPrice(_price);
        }
    }
    private bool _isChangeMoney;
    public Action ChangeMoney;

    private int _saveIndex;
    private bool _unlock = true;

    protected void OnEnable()
    {
        if (_uniqueName == "")
        {
            _uniqueName = this.gameObject.name;
        }

        //EventManager.ObjectPurshuased += ObjectPurchased;
        _buyStruct._buyArea.OnTrigger += TryBuy;
        EventManager.PlayerLevelChange += CheckLock;
        
    }
    private void CheckLock()
    {
        _lockStruct.LevelText.text = _levelToUnlock.ToString();
        _unlock = _levelToUnlock <= WitchPlayerController.Instanse.PlayerLevel;
        _purchasedState = _unlock ? PurchasedState.Unpurchased : PurchasedState.Locked;
    }
    private void CheckLock(int level)
    {
        if (_purchasedState != PurchasedState.Locked) { return; }
        _unlock = _levelToUnlock <= level;
        _purchasedState = _unlock ? PurchasedState.Unpurchased : PurchasedState.Locked;
        CheckState();
    }
    private void OnDisable()
    {
        _buyStruct._buyArea.OnTrigger -= TryBuy;
    }
    private void FixedUpdate()
    {
        if (_isChangeMoney)
        {
            BuyProcessing();
        }
    }
    public void Start()
    {
        if (!TryLoad(YandexGame.savesData.IOName))
        {
            CreateYGSaveSistem(ref YandexGame.savesData.IOName, ref YandexGame.savesData.IOPrice,ref YandexGame.savesData.IOBuild);
            CheckLock();
            CheckState();
            SaveIO();
        }
        else
        {
            LoadIO();
        }
        _SpeedFill /= 100;
    }
    public void CheckState()
    {
        switch (_purchasedState)
        {
            case PurchasedState.Locked:
                {
                    ObjectLocked();
                    break;
                }
            case PurchasedState.Unpurchased:
                {
                    ObjectUnPurchased();
                    break;
                }
            case PurchasedState.Purchased:
                {
                    ObjectPurchased();
                    break;
                }
            default:
                break;

        }
        
    }
    private bool TryLoad(string[] names)
    {
        bool check = false;
        if (names == null || names.Length == 0)
        {
            return check;
        }
        names.ForEach((name, index) => 
        {
            if (name == _uniqueName && check == false)
            {
                check = true;
                _saveIndex = index;

                return;
            }
        }
        );

        return check;
    }
    private void CreateYGSaveSistem(ref string[] names, ref int[] prices, ref bool[] builds)
    {
        if (names == null || names.Length == 0)
        {
            names = new string[1];
            prices = new int[1];
            builds = new bool[1];
        }
        else
        {
            string[] newArr = new string[names.Length + 1];
            int[] newArrPrice = new int[prices.Length + 1];
            bool[] newArrBuild = new bool[builds.Length + 1];
            for (int i = 0; i < names.Length; i++)
            {
                newArr[i] = names[i];
                newArrPrice[i] = prices[i];
                newArrBuild[i] = builds[i];
            }
            names = newArr;
            prices = newArrPrice;
            builds = newArrBuild;
        }
        _saveIndex = names.Length - 1;
        names[_saveIndex] = _uniqueName;
    }
    private void LoadIO()
    {
        if (YandexGame.savesData.IOBuild[_saveIndex]) 
        { 
            ObjectPurchased();
            return;
        }
        CheckLock();
        _price = YandexGame.savesData.IOPrice[_saveIndex];
    }
    private void SaveIO()
    {
        YandexGame.savesData.IOBuild[_saveIndex] = _purchasedState == PurchasedState.Purchased;
        YandexGame.savesData.IOPrice[_saveIndex] = _price;
    }
    private void TryBuy(bool inTrigger)
    {
        if (_purchasedState == PurchasedState.Purchased)
        {
            return;
        }
        if (!inTrigger)
        {
            YandexGame.SaveProgress();
        }
        _isChangeMoney = inTrigger;
    }
    private void BuyProcessing()
    {
        float rate = math.ceil(_SpeedFill * (float)_firstPrice);
        int rateInt = (int)rate;
        if (WitchPlayerController.Instanse.HaveMoney(rateInt) && _price > 0)
        {
            _price -= rateInt;
            WitchPlayerController.Instanse.Money -= rateInt;
        } 
        else if (_price <= 0)
        {
            //EventManager.ObjectPurshuased?.Invoke();
            ObjectPurchased();

        }
    }
    private void ChangeUIPrice(int price)
    {
        if (_buyStruct.MoneyText != null)
        {
            _buyStruct.MoneyText.text = price.ToString(); // Изменил, Нужно проверить DOTHIS
        }
        if (_buyStruct.BuyTimeImage != null)
        {
            _buyStruct.BuyTimeImage.fillAmount = 1 - price/(float)_firstPrice;
        }
    }
    private void ObjectPurchased()
    {
        _price = 0;
        _purchasedState = PurchasedState.Purchased;
        _workStruct.UIObject.SetActive(true);
        _isChangeMoney = false;
        _buyStruct._buyArea.gameObject.SetActive(false);
        _buyStruct.UIObject.gameObject.SetActive(false);
        _lockStruct.UIObject.gameObject.SetActive(false);
        SaveIO();
        YandexGame.SaveProgress();
    }
    private void ObjectUnPurchased()
    {

        _price = _firstPrice;
        _workStruct.UIObject.SetActive(false);
        _buyStruct._buyArea.gameObject.SetActive(true);
        _buyStruct.UIObject.gameObject.SetActive(true);
        _lockStruct.UIObject.gameObject.SetActive(false);
        SaveIO();
        YandexGame.SaveProgress();
    }
    private void ObjectLocked()
    {
        _workStruct.UIObject.SetActive(false);
        _buyStruct._buyArea.gameObject.SetActive(false);
        _buyStruct.UIObject.gameObject.SetActive(false);
        _lockStruct.UIObject.gameObject.SetActive(true);
    }
    public string GetName()
    {
        return _uniqueName;
    }
    public int GetIndex()
    {
        return _saveIndex;
    }
}
