
using System;
using Unity.Mathematics;
using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    [SerializeField]
    private PurchasedState _purchasedState;
    [SerializeField]
    private int _firstPrice;
    [SerializeField]
    private BuyStruct _buyStruct;
    [SerializeField]
    private WorkStruct _workStruct;
    [SerializeField]
    private UpdateStruct _updateStruct;
    [SerializeField]
    private float _SpeedFill = 1;

    private int _price;
    private bool _isChangeMoney;

    public Action ChangeMoney;
    
    protected void OnEnable()
    {
        //EventManager.ObjectPurshuased += ObjectPurchased;

        _buyStruct._buyArea.OnTrigger += TryBuy;
        
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
        _price = _firstPrice;
        ChangeUIPrice(_price);
        _SpeedFill /= 100;
    }

    private void OnValidate()
    {
        ChangeUIPrice(_firstPrice);
    }
    private void TryBuy(bool inTrigger)
    {
        if (_purchasedState == PurchasedState.Purchased)
        {
            return;
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
            ChangeUIPrice(_price);
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
            _buyStruct.MoneyText.text = AbbrevationUtility.AbbreviateNumber(price);
        }
        if (_buyStruct.BuyTimeImage != null)
        {
            _buyStruct.BuyTimeImage.fillAmount = 1 - price/(float)_firstPrice;
        }
    }
    private void ObjectPurchased()
    {
        _purchasedState = PurchasedState.Purchased;
        _workStruct.UIObject.SetActive(true);
        _isChangeMoney = false;
        _buyStruct._buyArea.gameObject.SetActive(false);
        _buyStruct.UIObject.gameObject.SetActive(false);
    }
}
