
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
    private InteractionObjectUI _interactionObjectUI;
    [SerializeField]
    private GameObject _usebleObject;
    [SerializeField]
    private CheckPlayer _buyArea;
    [SerializeField] 
    private GameObject _UIbuyArea;
    [SerializeField]
    private float _SpeedFill = 1;

    private int _price;
    private bool _isChangeMoney;

    public Action ChangeMoney;
    
    protected void OnEnable()
    {
        EventManager.ObjectPurshuased += ObjectPurchased;

        if (_buyArea)
        {
            _buyArea.OnTrigger += TryBuy;
        }
    }
    private void OnDisable()
    {
        if (_buyArea)
        {
            _buyArea.OnTrigger -= TryBuy;
        }
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
            EventManager.ObjectPurshuased?.Invoke();
        }
        /*if (_fillState < 1)
        {
            _fillState += Time.fixedDeltaTime * _SpeedFill;
        }
        else
        {
            _purchasedState = PurchasedState.Purchased;
            _fillState = 0;
        }*/
    }
    private void ChangeUIPrice(int price)
    {
        if (_interactionObjectUI.MoneyText != null)
        {
            _interactionObjectUI.MoneyText.text = AbbrevationUtility.AbbreviateNumber(price);
        }
        if (_interactionObjectUI.BuyTimeImage != null)
        {
            _interactionObjectUI.BuyTimeImage.fillAmount = 1 - price/(float)_firstPrice;
        }
    }
    private void ObjectPurchased()
    {
        _purchasedState = PurchasedState.Purchased;
        _usebleObject.SetActive(true);
        _isChangeMoney = false;
        _buyArea.gameObject.SetActive(false);
        _UIbuyArea.gameObject.SetActive(false);
    }
}
